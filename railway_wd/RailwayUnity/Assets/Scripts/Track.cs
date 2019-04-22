using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
Track script attached to a Track GameObject.
*/
public class Track : MonoBehaviour {

    public string id = "0";
    public string length = "5000";
    public string v_max = "100";    // check if you can configure this in AToMPM

    public ParameterShower parameterShower;

    void OnMouseDown() {
        Dictionary<string, string> parameters = new Dictionary<string, string>() {
            {"length", length},
            {"v_max", v_max}
        };
        parameterShower.show(id, parameters);
    }   

}
