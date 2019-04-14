﻿using System.Collections;
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
    Simulate the next command from the trace.
    */
    public void simulateNextCommand(float timeScaleFactor) {

        if(commands.Count > 0) {
            string line = commands[0];
            Debug.Log(line);
            commands.RemoveAt(0);
            
            if(line.Contains(":")) {
                string[] trace = line.Split(':')[1].Split(' ');

                // New train at start station
                if(trace[3] == "to" && trace[4] == "StartStation") {
                    GameObject track = GameObject.Find(trace[5]);
                    railwayFactory.createTrain("Train " + trace[2], track.transform.position.x, track.transform.position.z+80);

                // Train moves to new track
                } else if(trace[3] == "to" && trace[4] == "Track") {
                    trainMover.moveTrainToTrack(trace[2], trace[5]);

                // Train moves to 1km mark on track
                } else if(trace[3] == "reaches" && trace[4] == "1km") {
                    trainMover.moveTrainTo1KmMark(trace[2], trace[10], float.Parse(trace[7].Substring(0, trace[7].Length - 1))/timeScaleFactor);

                // Train accelerates last part of track
                } else if(trace[3] == "accelerates") {
                    trainMover.moveTrainToEnd(trace[2], trace[10], float.Parse(trace[7].Substring(0, trace[7].Length - 1))/timeScaleFactor);
                }
            }
        }
    }
}