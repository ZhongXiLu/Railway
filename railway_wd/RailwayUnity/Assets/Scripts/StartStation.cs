using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartStationData {
    public string id = "0";
    public string schedule = "";
    public string IAT_min = "10";
    public string IAT_max = "30";
    public string a_min = "0.2";
    public string a_max = "0.7";
}

/**
StartStation script attached to a Track GameObject that is a start station.
*/
public class StartStation : MonoBehaviour {

    public StartStationData startStation = new StartStationData();
    public ParameterShower parameterShower;

    void OnMouseDown() {
        Dictionary<string, string> parameters = new Dictionary<string, string>() {
            {"schedule", startStation.schedule},
            {"IAT_min", startStation.IAT_min},
            {"IAT_max", startStation.IAT_max},
            {"a_min", startStation.a_min},
            {"a_max", startStation.a_max},
        };
        parameterShower.show(startStation.id, parameters);
    }   

}
