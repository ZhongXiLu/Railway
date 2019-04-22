
ID_counter = 0


class Train:

    def __init__(self, a_max, departure_time=0, schedule=[], start="Start", end="End"):
        global ID_counter
        self.ID = ID_counter
        ID_counter += 1
        self.schedule = schedule
        self.start = start
        self.end = end

        self.a_max = a_max
        self.departure_time = departure_time
        self.v = 0
        self.remaining_x = 0

    def __str__(self):
        return "Train id={} schedule=[{}] a_max={}".format(self.ID, ','.join(map(str, [str(step) for step in self.schedule])), self.a_max)


class Query:

    def __init__(self, train_id):
        self.train_id = train_id

    def __str__(self):
        return "Query for {}".format(self.train_id)


class QueryAck:

    def __init__(self, train_id, trafficLight):
        self.train_id = train_id
        self.trafficLight = trafficLight

    def __str__(self):
        return "Query Ack for {}: {}".format(self.train_id, self.trafficLight)

