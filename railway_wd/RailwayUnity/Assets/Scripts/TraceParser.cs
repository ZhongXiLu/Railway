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
            }
        }
        return train;
    }
     
    /**
    Simulate the next command from the trace.
    @param timeScaleFactor Time scale factor of the simulation (e.g. '2' means twice as fast).
    */
    public void simulateNextCommand(float timeScaleFactor) {

        if(commands.Count > 0) {
            string line = commands[0];
            Debug.Log(line);
            commands.RemoveAt(0);
            
            if(line.Contains(":")) {
                string[] trace = line.Split(':')[1].Split(' ');
                Dictionary<string, string> train = parseTrain(line);

                // New train at start station
                if(trace[4] == "to" && trace[5] == "StartStation") {
                    GameObject track = GameObject.Find(trace[6]);
                    railwayFactory.createTrain("Train " + train["id"], track.transform.position.x, track.transform.position.z+80, trace[8], trace[10]);

                // Train reaches end station
                } else if(trace[4] == "to" && trace[5] == "EndStation") {
                    railwayFactory.destroyTrain(train["id"], trace[6]);

                // Train moves to new track
                } else if(trace[4] == "to") {
                    trainMover.moveTrainToTrack(train["id"], trace[6]);

                // Train moves to 1km mark on track
                } else if(trace[4] == "reaches" && trace[5] == "1km") {
                    trainMover.moveTrainTo1KmMark(train["id"], train["schedule"], trace[11], trace[10], float.Parse(trace[8].Substring(0, trace[8].Length - 1))/timeScaleFactor);

                // Train accelerates last part of track
                } else if(trace[4] == "accelerates") {
                    trainMover.moveTrainToEnd(train["id"], trace[11], float.Parse(trace[8].Substring(0, trace[8].Length - 1))/timeScaleFactor);
                }
            }
        }
    }
}
