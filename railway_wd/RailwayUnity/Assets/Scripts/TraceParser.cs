using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
TraceParser: reads data from the RailwayDEVS trace file.
Also sends out the right call to the appropriate object.
*/
public class TraceParser : MonoBehaviour {

    public Railway railway;
    public TrainMover trainMover;
    public RailwayFactory railwayFactory;

    List<string> commands;

    /**
    Loads a trace file from the RailwayDEVS.

    @param traceFile The trace file.
    */
    public void loadTrace(string traceFile) {
        TextAsset textAsset = (TextAsset)Resources.Load(traceFile);
        commands = new List<string>(textAsset.text.Split('\n'));
    }

    /**
    Check if there is still a command that hasn't been run yet.
    @return True if there still exists a command.
    */
    public bool nextCommand() {
        if(commands.Count > 0) {
            string line = commands[0];
            return (line.Contains(":"));
        } else {
            return false;
        }
    }

    /**
    Returns the timestamp of the next command.
    @return The next timestamp (-1f if there is no next command).
    */
    public float nextTimestamp() {
        if(commands.Count > 0) {
            string line = commands[0];
            if(line.Contains(":")) {
                string time = line.Split(':')[0];
                return float.Parse(time);
            }
        }
        return -1f;
    }

    /**
    Parse the train part from a line from the tracefile.

    @param line     Complete line from the tracefile.
    @return         Dictionary with the parsed results.
    */
    Dictionary<string, string> parseTrain(string line) {
        Dictionary<string, string> train = new Dictionary<string, string>();
        string[] trace = line.Split(':')[1].Split(' ');

        foreach(string text in trace) {
            if(text.Contains("id")) {
                train.Add("id", text.Substring(3, text.Length-3));
            } else if(text.Contains("schedule")) {
                train.Add("schedule", text.Substring(10, text.Length-11));
            } else if(text.Contains("a_max")) {
                train.Add("a_max", text.Substring(6, text.Length-6));
            }
        }
        return train;
    }


    /**
    Simulate a command.
    @param command         The command.
    @param timeScaleFactor Time scale factor of the simulation (e.g. '2' means twice as fast).
    */
    public void simulateCommand(string command, float timeScaleFactor) {
        if(command.Contains(":")) {
            Debug.Log(command);

            string[] trace = command.Split(':')[1].Split(' ');
            Dictionary<string, string> train = parseTrain(command);

            int traceOffset = 5;
            // New train at start station
            if(trace[traceOffset] == "to" && trace[traceOffset+1] == "StartStation") {
                GameObject track = GameObject.Find(trace[traceOffset+2]);
                railwayFactory.createTrain(train["id"], track.transform.position.x, track.transform.position.z+80, trace[traceOffset+4], trace[traceOffset+6], train["schedule"], train["a_max"]);

            // Train reaches end station
            } else if(trace[traceOffset] == "to" && trace[traceOffset+1] == "EndStation") {
                railwayFactory.destroyTrain(train["id"], trace[traceOffset+2]);

            // Train moves to new track
            } else if(trace[traceOffset] == "to") {
                trainMover.moveTrainToTrack(train["id"], trace[traceOffset+2]);

            // Train moves to 1km mark on track
            } else if(trace[traceOffset] == "reaches" && trace[traceOffset+1] == "1km") {
                trainMover.moveTrainTo1KmMark(train["id"], train["schedule"], trace[traceOffset+7], trace[traceOffset+6], float.Parse(trace[traceOffset+4].Substring(0, trace[traceOffset+4].Length - 1))/timeScaleFactor);

            // Train accelerates last part of track
            } else if(trace[traceOffset] == "accelerates") {
                trainMover.moveTrainToEnd(train["id"], trace[traceOffset+7], float.Parse(trace[traceOffset+4].Substring(0, trace[traceOffset+4].Length - 1))/timeScaleFactor);
            }
        }
    }
     
    /**
    Simulate the next command from the trace.
    @param timeScaleFactor Time scale factor of the simulation (e.g. '2' means twice as fast).
    */
    public void simulateNextCommand(float timeScaleFactor) {

        if(commands.Count > 0) {
            string line = commands[0];
            commands.RemoveAt(0);
            
            simulateCommand(line, timeScaleFactor);
        }
    }
}
