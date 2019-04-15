using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

/**
WorldLoader: loads a railway network from a given file (Assets/Resources/railway.xml).
*/
public class WorldLoader : MonoBehaviour {

    public Railway railway;
    public RailwayFactory railwayFactory;

    /**
    Fill the railway object, i.e. create all the Track objects.

    @param xmldoc   The xml file that contains the railway network.
    @param railway  The (empty) railway object that needs to be populated.
    */
    void populateRailway(XmlDocument xmldoc, Railway railway) {
        foreach(XmlNode track in xmldoc.GetElementsByTagName("Railway")[0].ChildNodes) {
            Railway.Track newTrack = new Railway.Track();
            int id = -1;
            newTrack.type = track.Name;
            foreach(XmlNode data in track.ChildNodes) {
                if(data.Name == "id") {
                    id = int.Parse(data.InnerText);
                    newTrack.id = id;
                } else if(data.Name == "length") {
                    newTrack.length = int.Parse(data.InnerText);
                } else if(data.Name == "name") {
                    newTrack.name = data.InnerText;
                } else if(data.Name == "ports") {
                    foreach(XmlNode port in data.ChildNodes) {
                        newTrack.ports[port.Name] = int.Parse(port.InnerText);
                    }
                }
            }
            railway.tracks.Add(id, newTrack);
        }
    }

    /**
    Create a track on a specific position.

    @param track    The new track that will be placed.
    @param position The position of the track.
    */
    void createTrack(Railway.Track track, Vector2 position) {
        if(track.type == "Junction") {
            railwayFactory.createJunction(track.id.ToString(), Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(track.type == "Turnout") {
            railwayFactory.createTurnout(track.id.ToString(), Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(track.type == "Straight") {
            railwayFactory.createStraight(track.id.ToString(), Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
        } else if(track.type == "Station") {
            railwayFactory.createStation(track.id.ToString(), Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), track.name);
        } else if(track.type == "Crossing") {
            railwayFactory.createCrossing(track.id.ToString(), Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
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
    Vector2 adjustPosition(Railway.Track originalTrack, Railway.Track newTrack, string port, Dictionary<int, Vector2> positions) {
        Vector2 newPosition = positions[originalTrack.id];
        // Position relative to other track
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

    public void loadWorld(string railwayfile) {

        // Ground
        railwayFactory.createGround(10000, 10000);

        // Load railway xml file
        TextAsset textAsset = (TextAsset)Resources.Load(railwayfile);  
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.LoadXml(textAsset.text);

        // Populate railway
        populateRailway(xmldoc, railway);

        // Place tracks in BFS order, starting from a start station
        Dictionary<int, bool> visited = new Dictionary<int, bool>();
        Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();

        Railway.Track startStation = railway.getStartStation();
        Vector2 position = new Vector2(0, 0);
        createTrack(startStation, position);
        positions[startStation.id] = position;

        Queue<Railway.Track> queue = new Queue<Railway.Track>();
        queue.Enqueue(startStation);

        while(queue.Count != 0) {
            Railway.Track track = queue.Dequeue();
            foreach(KeyValuePair<string, int> port in track.ports) {
                Railway.Track newTrack = railway.tracks[port.Value];
                if(!visited.ContainsKey(newTrack.id) || !visited[newTrack.id]) {                        
                    Vector2 newPosition = adjustPosition(track, newTrack, port.Key, positions);
                    positions[newTrack.id] = newPosition;
                    createTrack(newTrack, newPosition);
                    queue.Enqueue(newTrack);
                }
            }

            visited[track.id] = true;
        }
    }
}
