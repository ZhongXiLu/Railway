
import json
from RailwayDEVS.models import *
from pypdevs.simulator import Simulator
from pypdevs.DEVS import AtomicDEVS, CoupledDEVS

class Railway(CoupledDEVS):
    def __init__(self):

        CoupledDEVS.__init__(self, "Railway")

sim = Simulator(Railway())
sim.setTerminationTime(0)
sim.setClassicDEVS()
sim.simulate()

results = {}

with open('railway_wd/railway_results.json', 'w') as outfile:
    json.dump(results, outfile, indent=4)
