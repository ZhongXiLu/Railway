using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TrackData {
    public string id = "0";
    public string length = "5000";
    public string v_max = "100";    // check if you can configure this in AToMPM
}

/**
Track script attached to a Track GameObject.
*/
public class Track : MonoBehaviour {

    public TrackData track = new TrackData();
    public ParameterShower parameterShower;

    void OnMouseDown() {
        Dictionary<string, string> parameters = new Dictionary<string, string>() {
            {"length", track.length},
            {"v_max", track.v_max}
        };
        parameterShower.show(track.id, parameters);
    }   

}
