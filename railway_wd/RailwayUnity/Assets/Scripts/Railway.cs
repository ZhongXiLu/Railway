using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Railway: represents the complete railway network.
Note that this is only used for generating the objects.
*/
public class Railway : MonoBehaviour {

    /**
    Track: represents a generic track.
    */
    public class Track {
        public string type;
        public int id;
        public string name = "";
        public int length = 100;
        public Dictionary<string, int> ports = new Dictionary<string, int>(); ///< Dictonary of port name to track id.
    }

    public Dictionary<int, Track> tracks = new Dictionary<int, Track>(); ///< Dictonary of id to track.

    /**
    Get a random start station from the railway network.

    @return The track that contains a start station.
    */
    public Track getStartStation() {
        foreach(Track track in tracks.Values) {
            int inCount = 0;
            foreach(string port in track.ports.Keys) {
                if(port == "in" || port == "in2") {
                    inCount++;
                }
            }
            if(inCount == 0) {
                return track;
            }
        }
        return null;    // no StartStation
    }

}
