using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

/**
WorldLoader: loads a railway network from a given file (Assets/Resources/railway.xml).
*/
public class WordLoader : MonoBehaviour {

    public TrackFactory trackFactory;

    public class Track {
        public string type;
        public int name;
        public Dictionary<string, int> ports = new Dictionary<string, int>();
    }

    public class Railway {

        public Dictionary<int, Track> tracks = new Dictionary<int, Track>();

    }

    /**
    Get a random start station from the railway network.

    @param railway  The railway network object.
    @return         The tracks that contains a start station.
    */
    Track getStartStation(Railway railway) {
        foreach(Track track in railway.tracks.Values) {
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

    /**
    Fill the railway object, i.e. create all the Track objects.

    @param xmldoc   The xml file that contains the railway network.
    @param railway  The (empty) railway object that needs to be populated.
    */
    void populateRailway(XmlDocument xmldoc, Railway railway) {
        foreach(XmlNode track in xmldoc.GetElementsByTagName("Railway")[0].ChildNodes) {
            Track newTrack = new Track();
            int name = -1;
            newTrack.type = track.Name;
            foreach(XmlNode data in track.ChildNodes) {
                if(data.Name == "name") {
                    name = int.Parse(data.InnerText);
                    newTrack.name = name;
                } else if(data.Name == "ports") {
                    foreach(XmlNode port in data.ChildNodes) {
                        newTrack.ports[port.Name] = int.Parse(port.InnerText);
                    }
                }
            }
            railway.tracks.Add(name, newTrack);
        }
    }

    /**
    Create a track on a specific position.

    @param type     The type of the track.
    @param name     The name of the track.
    @param position The position of the track.
    */
    void createTrack(string type, string name, Vector2 position) {
        if(type == "Junction") {
            trackFactory.createJunction(name, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(type == "Turnout") {
            trackFactory.createTurnout(name, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(type == "Straight") {
            trackFactory.createStraight(name, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(type == "Station") {
            trackFactory.createStation(name, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(type == "Crossing") {
            trackFactory.createCrossing(name, Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        }
    }

    /**
    Calculate a track's position based on another track and make sure they are well connected.

    @param originalTrack    The track the new track will be placed around.
    @param newTrack         The new track that will be placed. 
    @param port             The port at which the new track will be placed (e.g. out, out2, ...).
    @param positions        Dictionary that contains the positions of already placed tracks.
    @return                 The position of the new track.
    */
    Vector2 adjustPosition(Track originalTrack, Track newTrack, string port, Dictionary<int, Vector2> positions) {
        Vector2 newPosition = positions[originalTrack.name];
        if(port == "out") {
            newPosition.y += 80;
        } else if(port == "in") {
            newPosition.y -= 80;
        } else if(port == "out2") {
            if(newTrack.type == "Junction") {
                newPosition += new Vector2(-30, 80);
            } else {
                newPosition += new Vector2(30, 80);
            }
        } else if(port == "in2") {
            if(newTrack.type == "Junction") {
                newPosition += new Vector2(-30, -80);
            } else {
                newPosition += new Vector2(30, -80);
            }
        }
        // Crossing is twice its size
        if(originalTrack.type == "Crossing") {
            if(port == "out") {
                newPosition += new Vector2(30, 80);
            } else if(port == "out2") {
                if(newTrack.type == "Junction") {
                    newPosition += new Vector2(-30, 80);
                } else {
                    newPosition += new Vector2(30, 80);
                }
            }
        }
        return newPosition;
    }

    void Start() {

        // Ground
        trackFactory.createGround(1000, 1000);

        // Load railway xml file
        TextAsset textAsset = (TextAsset)Resources.Load("railway");  
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(textAsset.text);

        // Populate railway
        Railway railway = new Railway();
        populateRailway(xmldoc, railway);

        // Place tracks in BFS order, starting from a start station
        Dictionary<int, bool> visited = new Dictionary<int, bool>();
        Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();

        Track startStation = getStartStation(railway);
        Vector2 position = new Vector2(0, 0);
        trackFactory.createStation(startStation.name.ToString(), Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        positions[startStation.name] = position;

        Queue<Track> queue = new Queue<Track>();
        queue.Enqueue(startStation);

        while(queue.Count != 0) {
            Track track = queue.Dequeue();
            foreach(KeyValuePair<string, int> port in track.ports) {
                Track newTrack = railway.tracks[port.Value];
                if(!visited.ContainsKey(newTrack.name) || !visited[newTrack.name]) {                        
                    Vector2 newPosition = adjustPosition(track, newTrack, port.Key, positions);
                    positions[newTrack.name] = newPosition;
                    createTrack(railway.tracks[port.Value].type, newTrack.name.ToString(), newPosition);
                    queue.Enqueue(newTrack);
                }
            }

            visited[track.name] = true;
        }
    }
}
