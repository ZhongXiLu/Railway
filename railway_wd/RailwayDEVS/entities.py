
ID_counter = 0

class Train:

    def __init__(self, a_max, departure_time=0, schedule=[]):
        global ID_counter
        self.ID = ID_counter
        ID_counter += 1
        self.schedule = schedule

        self.a_max = a_max
        self.departure_time = departure_time
        self.v = 0
        self.remaining_x = 0

    def __str__(self):
        return "Train: id={}, departure_time={}".format(self.ID, self.departure_time)

class Query:

    def __init__(self):
        pass

    def __str__(self):
        return "Query"

class QueryAck:

    def __init__(self, trafficLight):
        self.trafficLight = trafficLight

    def __str__(self):
        return "Query Ack: {}".format(self.trafficLight)

