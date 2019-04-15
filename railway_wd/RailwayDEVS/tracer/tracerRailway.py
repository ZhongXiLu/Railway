
import sys

from RailwayDEVS.models import *

class TracerRailway(object):

    def __init__(self, uid, server, filename=None):
        """
        Both uid and server can be ignored, as these are only required for distributed simulation
        filename contains the name of the file in which we should write the trace
        """
        self.filename = filename

    def startTracer(self, recover):
        """
        Recover is a boolean representing whether or not this is a recovered call (e.g., should the file be overwritten or appended to?)
        """
        if self.filename is None:
            self.verb_file = sys.stdout
        elif recover:
            self.verb_file = open(self.filename, 'a+')
        else:
            self.verb_file = open(self.filename, 'w')

    def stopTracer(self):
        """
        Stops the tracer (e.g., flush the file)
        """
        self.verb_file.flush()

    def trace(self, time, text):
        message = "{} : {}\n".format(time[0], text)
        try:
            self.verb_file.write(message)
        except TypeError:
            self.verb_file.write(message.encode())

    def traceInternal(self, aDEVS):
        """
        Called for each atomic DEVS model that does an internal transition.
        """

        # Train generated at a start station
        if type(aDEVS.model) is PollQueue:
            if aDEVS.state == "send_query":
                if aDEVS.model.train is not None:
                    self.trace(aDEVS.time_last, "{} to StartStation {} from {} to {}".format(aDEVS.train, aDEVS.getModelName()[1:], aDEVS.train.start, aDEVS.train.end))

        elif isinstance(aDEVS.model, RailwaySegment):
            # Train driving on segment (until 1km mark = next light within sight)
            if aDEVS.state[0] == "driving":
                self.trace(aDEVS.time_last, "{} reaches 1km mark in {}s on {} {}".format(aDEVS.train, aDEVS.duration, aDEVS.model.__class__.__name__, aDEVS.getModelName()[1:]))

            # Train accelerates to end of segment
            elif aDEVS.state[0] == "accelerating":
                self.trace(aDEVS.time_last, "{} accelerates to end in {}s on {} {}".format(aDEVS.train, aDEVS.duration, aDEVS.model.__class__.__name__, aDEVS.getModelName()[1:]))

            # Train brakes on last part of segment
            elif aDEVS.state[0] == "brake_train":
                self.trace(aDEVS.time_last, "{} brakes on end for {}s on {} {}".format(aDEVS.train, aDEVS.duration, aDEVS.model.__class__.__name__, aDEVS.getModelName()[1:]))

    def traceExternal(self, aDEVS):
        """
        Called for each atomic DEVS model that does an external transition.
        """

        text = None
        for I in range(len(aDEVS.IPorts)):

            if aDEVS.IPorts[I].getPortName() == "train_in":
                # Train arrived at end station
                if type(aDEVS.model) is Collector:
                    for train in aDEVS.my_input.get(aDEVS.IPorts[I], []):
                        text = "{} to EndStation {}".format(train, aDEVS.getModelName()[1:])

                # Train moved from one track to another
                elif type(aDEVS.model) is not PollQueue:
                    for train in aDEVS.my_input.get(aDEVS.IPorts[I], []):
                        text = "{} to {} {}".format(train, aDEVS.model.__class__.__name__, aDEVS.getModelName()[1:])

        if isinstance(aDEVS.model, RailwaySegment):
            if aDEVS.state[0] == "accelerating":
                self.trace(aDEVS.time_last, "{} accelerates to end in {}s on {} {}".format(aDEVS.train, aDEVS.duration, aDEVS.model.__class__.__name__, aDEVS.getModelName()[1:]))

        if text:
            self.trace(aDEVS.time_last, text)

    def traceConfluent(self, aDEVS):
        """
        Called for each atomic DEVS model that does a confluent transition.
        """
        pass

    def traceInit(self, aDEVS, t):
        """
        Called upon initialization of a model.
        The parameter *t* contains the time at which the model commences (likely 0).
        """
        pass

    def traceUser(self, time, aDEVS, variable, value):
        """
        Called upon so called *god events* during debuggin, where a user manually alters the state of an atomic DEVS instance.
        """
        pass