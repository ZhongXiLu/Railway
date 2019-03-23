
import random

from entities import *
from formulas import *
from pypdevs.simulator import Simulator
from pypdevs.DEVS import AtomicDEVS, CoupledDEVS

class Collector(AtomicDEVS):
    def __init__(self):
        AtomicDEVS.__init__(self, "Collector")
        self.train_in = self.addInPort("train_in")
        self.Q_recv = self.addInPort("Q_recv")
        self.Q_sack = self.addOutPort("Q_sack")

        self.elapsed = 0.0
        self.current_time = 0.0
        self.state = "neutral"
        self.query_id = -1

        self.transit_times = []

    def timeAdvance(self):
        return {
            "neutral": float("inf"),
            "send_ack": 0,
            "new_train": 0
        }[self.state]

    def outputFnc(self):
        if self.state == "send_ack":
            return {self.Q_sack: QueryAck(self.query_id, "Green")}     # always green
        elif self.state == "new_train":
            pass
            # print("Average transit time: {}s".format(sum(self.transit_times)/float(len(self.transit_times))))
        return {}

    def intTransition(self):
        self.current_time += self.timeAdvance()
        return {
            "send_ack": "neutral",
            "new_train": "neutral",
            "neutral": "neutral"
        }[self.state]

    def extTransition(self, inputs):
        self.current_time += self.elapsed

        # Received request
        if self.Q_recv in inputs:
            self.query_id = inputs[self.Q_recv].train_id
            return "send_ack"

        # Train arrives on segment
        elif self.train_in in inputs:
            train = inputs[self.train_in]

            # Calculate transit time
            transit_time = self.current_time - train.departure_time
            self.transit_times.append(transit_time)

            return "new_train"

        return self.state

class RailwaySegment(AtomicDEVS):
    def __init__(self, L, v_max=100):
        AtomicDEVS.__init__(self, "RailwaySegment")
        self.train_in = self.addInPort("train_in")
        self.train_out = self.addOutPort("train_out")
        self.Q_recv = self.addInPort("Q_recv")
        self.Q_sack = self.addOutPort("Q_sack")
        self.Q_rack = self.addInPort("Q_rack")
        self.Q_send = self.addOutPort("Q_send")

        self.elapsed = 0.0
        self.state = ("neutral", "neutral")

        self.L = L
        self.v_max = v_max
        self.v_end = 0      # velocity of train when it reaches the end
        self.x_end = 0      # position of train when it reaches the end
        self.train = None
        self.timeArrival = 0
        self.duration = 0
        self.isLightInSight = False
        self.query_id = -1

        self.current_time = 0.0
        self.train_arrival_time = 0.0
        self.transit_times = []
        self.uptime = 0

    def timeAdvance(self):
        firstState = 0
        if self.state[0] == "new_train":

            if self.train.remaining_x > 1000:
                # Next light is NOT in sight
                distance_till_light = self.train.remaining_x - 1000     # distance till we can see the light
                self.train.remaining_x = 1000
                self.v_end, t = acceleration_formula(self.train.v, self.v_max, distance_till_light, self.train.a_max)
                firstState = 0
                self.duration = t
                self.timeArrival = self.time_last[0]
            else:
                # Next light is in sight => send query
                self.x_end = self.L
                self.v_end = self.train.v
                firstState = 0

        elif self.state[0] == "driving":
            remaining = self.time_last[0] - self.timeArrival
            self.duration = self.duration - remaining
            if self.duration <= 0:
                self.duration = 0
                self.isLightInSight = True
            firstState = self.duration
            self.timeArrival = self.time_last[0]
            # print("duration till lights for train {}: {}".format(self.train, self.duration))

        elif self.state[0] == "accelerating":
            self.train.v = self.v_end
            # self.train.remaining_x = 1000
            self.train.v, t = acceleration_formula(self.train.v, self.v_max, self.train.remaining_x, self.train.a_max)
            self.train.remaining_x = 0
            firstState = t
            self.state = ("leave_train", self.state[1])
        elif self.state[0] == "brake_train":
            self.train.v = self.v_end
            self.train.remaining_x = self.x_end
            self.v_end, self.x_end = brake_formula(self.train.v, 1, self.train.remaining_x)
            firstState = 1
        else:
            firstState = {
                "neutral": float("inf"),
                "leave_train": 0,
                "send_query": 1,
                "check_light": 0
            }[self.state[0]]
        secondState = {
            "neutral": float("inf"),
            "send_ack": 0
        }[self.state[1]]
        return min(firstState, secondState)

    def outputFnc(self):
        out = {}
        if self.state[0] == "send_query" or self.state[0] == "brake_train":
            out[self.Q_send] = Query(self.train.ID)
        elif self.state[0] == "leave_train":
            self.train.v = self.v_end
            out[self.train_out] = self.train
            self.train = None
            self.transit_times.append(self.current_time - self.train_arrival_time)

        if self.state[1] == "send_ack":
            trafficLight = "Green" if self.train is None else "Red"
            out[self.Q_sack] = QueryAck(self.query_id, trafficLight)

        return out

    def intTransition(self):
        self.current_time += self.timeAdvance()
        if self.train is not None:
            self.uptime += self.timeAdvance()
        firstState = None
        if self.isLightInSight:
            self.isLightInSight = False
            if self.L > 1000:
                # Next light is NOT in sight
                firstState = "check_light"
            else:
                # Next light is in sight
                firstState = "send_query"
        else:
            firstState = {
                "check_light": "send_query",
                "send_query": "send_query",
                "accelerating": "leave_train",
                "new_train": "driving",
                "driving": "driving",
                "leave_train": "neutral",
                "brake_train": "brake_train",
                "neutral": "neutral"
            }[self.state[0]]
        secondState = {
            "send_ack": "neutral",
            "neutral": "neutral",
        }[self.state[1]]
        return (firstState, secondState)

    def extTransition(self, inputs):
        self.current_time += self.elapsed
        if self.train is not None:
            self.uptime += self.elapsed
        # Received ack
        if self.Q_rack in inputs:
            if (self.train is not None) and inputs[self.Q_rack].train_id == self.train.ID:
                if inputs[self.Q_rack].trafficLight == "Green":
                    return ("accelerating", self.state[1])
                else:
                    return ("brake_train", self.state[1])

        # Received request
        elif self.Q_recv in inputs:
            if self.train is None:
                self.query_id = inputs[self.Q_recv].train_id
                return (self.state[0], "send_ack")

        # Train arrives on segment
        elif self.train_in in inputs:
            self.train_arrival_time = self.current_time
            self.train = inputs[self.train_in]
            self.train.remaining_x = self.L
            return ("new_train", self.state[1])

        return self.state

class Join(RailwaySegment):
    def __init__(self, L, v_max=100):
        RailwaySegment.__init__(self, L, v_max)
        self.sent_ack = False

    def outputFnc(self):
        out = {}
        if self.state[0] == "send_query" or self.state[0] == "brake_train":
            out[self.Q_send] = Query(self.train.ID)
        elif self.state[0] == "leave_train":
            self.train.v = self.v_end
            out[self.train_out] = self.train
            self.train = None
            self.transit_times.append(self.current_time - self.train_arrival_time)

        if self.state[1] == "send_ack" and not self.sent_ack:
            trafficLight = "Green" if self.train is None else "Red"
            out[self.Q_sack] = QueryAck(self.query_id, trafficLight)
            self.sent_ack = True

        return out

    def extTransition(self, inputs):
        self.current_time += self.elapsed
        if self.train is not None:
            self.uptime += self.elapsed
        # Received ack
        if self.Q_rack in inputs:
            if inputs[self.Q_rack].train_id == self.train.ID:
                if inputs[self.Q_rack].trafficLight == "Green":
                    return ("accelerating", self.state[1])
                else:
                    return ("brake_train", self.state[1])

        # Received request
        elif self.Q_recv in inputs:
            if self.train is None:
                self.query_id = inputs[self.Q_recv].train_id
                return (self.state[0], "send_ack")

        # Train arrives on segment
        elif self.train_in in inputs:
            self.train_arrival_time = self.current_time
            self.sent_ack = False
            self.train = inputs[self.train_in]
            self.train.remaining_x = self.L
            return ("new_train", self.state[1])

        return self.state

class Split(RailwaySegment):
    def __init__(self, L, v_max=100):
        RailwaySegment.__init__(self, L, v_max)
        self.train_out2 = self.addOutPort("train_out2")
        self.Q_send2 = self.addOutPort("Q_send2")

    def outputFnc(self):
        out = {}
        if self.state[0] == "send_query" or self.state[0] == "brake_train":
            if self.train.schedule[0] == "STRAIGHT":
                out[self.Q_send] = Query(self.train.ID)
            else:
                out[self.Q_send2] = Query(self.train.ID)
        elif self.state[0] == "leave_train":
            self.train.v = self.v_end
            if self.train.schedule.pop(0) == "STRAIGHT":
                out[self.train_out] = self.train
            else:
                out[self.train_out2] = self.train
            self.train = None
            self.transit_times.append(self.current_time - self.train_arrival_time)

        if self.state[1] == "send_ack":
            trafficLight = "Green" if self.train is None else "Red"
            out[self.Q_sack] = QueryAck(self.query_id, trafficLight)
            self.sent_ack = True

        return out

class Crossing(RailwaySegment):
    def __init__(self, L, v_max=100):
        RailwaySegment.__init__(self, L, v_max)
        self.train_out2 = self.addOutPort("train_out2")
        self.Q_send2 = self.addOutPort("Q_send2")
        self.sent_ack = False

    def outputFnc(self):
        out = {}
        if self.state[0] == "send_query" or self.state[0] == "brake_train":
            if self.train.schedule[0] == "STRAIGHT":
                out[self.Q_send] = Query(self.train.ID)
            else:
                out[self.Q_send2] = Query(self.train.ID)
        elif self.state[0] == "leave_train":
            self.train.v = self.v_end
            if self.train.schedule.pop(0) == "STRAIGHT":
                out[self.train_out] = self.train
            else:
                out[self.train_out2] = self.train
            self.train = None
            self.transit_times.append(self.current_time - self.train_arrival_time)

        if self.state[1] == "send_ack" and not self.sent_ack:
            trafficLight = "Green" if self.train is None else "Red"
            out[self.Q_sack] = QueryAck(self.query_id, trafficLight)
            self.sent_ack = True

        return out

    def extTransition(self, inputs):
        self.current_time += self.elapsed
        if self.train is not None:
            self.uptime += self.elapsed
        # Received ack
        if self.Q_rack in inputs:
            if inputs[self.Q_rack].train_id == self.train.ID:
                if inputs[self.Q_rack].trafficLight == "Green":
                    return ("accelerating", self.state[1])
                else:
                    return ("brake_train", self.state[1])

        # Received request
        elif self.Q_recv in inputs:
            if self.train is None:
                self.query_id = inputs[self.Q_recv].train_id
                return (self.state[0], "send_ack")

        # Train arrives on segment
        elif self.train_in in inputs:
            self.train_arrival_time = self.current_time
            self.sent_ack = False
            self.train = inputs[self.train_in]
            self.train.remaining_x = self.L
            return ("new_train", self.state[1])

        return self.state

class PollQueue(AtomicDEVS):
    def __init__(self):
        AtomicDEVS.__init__(self, "PollQueue")
        self.state = "neutral"
        self.train_in = self.addInPort("train_in")
        self.train_out = self.addOutPort("train_out")
        self.Q_rack = self.addInPort("Q_rack")
        self.Q_send = self.addOutPort("Q_send")

        self.queue = []

    def timeAdvance(self):
        timeAdvance = 1000 -self.time_next[0]*1000%1000      # schedule on the exact second
        return {
            "neutral": timeAdvance/1000.0,
            "send_query": 0,
            "send_train": 0,
            "send_query_again": 1
        }[self.state]

    def outputFnc(self):
        if self.state is "send_query":
            return {self.Q_send: Query(self.queue[0])}
        elif self.state is "send_train":
            return {self.train_out: self.queue.pop(0)}
        return {}

    def extTransition(self, inputs):
        # Send train to output segment
        if self.Q_rack in inputs:
            if inputs[self.Q_rack].trafficLight == "Green" and len(self.queue) > 0:
                return "send_train"
            else:
                return "send_query_again"

        # Put new train in queue
        elif self.train_in in inputs:
            self.queue.append(inputs[self.train_in])
            return "send_query"

        return self.state

    def intTransition(self):
        return {
            "send_query": "neutral",
            "send_query_again": "send_query",
            "send_train": "neutral",
            "neutral": "neutral"
        }[self.state]

class Generator(AtomicDEVS):
    def __init__(self, IAT_min, IAT_max, a_min, a_max, schedule=[]):
        AtomicDEVS.__init__(self, "Generator")
        self.train_out = self.addOutPort("train_out")
        self.elapsed = 0.0
        self.state = "Generator"
        self.schedule = schedule

        self.IAT_min = IAT_min
        self.IAT_max = IAT_max
        self.a_min = a_min
        self.a_max = a_max

        self.trains_generated = 0

        random.seed(123)

    def timeAdvance(self):
        return random.random()*(self.IAT_max-self.IAT_min)+self.IAT_min

    def outputFnc(self):
        a = random.random()*(self.a_max-self.a_min)+self.a_min
        departure_time = self.time_next[0]
        self.trains_generated += 1
        return {self.train_out: Train(a, departure_time, self.schedule)}

    def intTransition(self):
        return self.state

