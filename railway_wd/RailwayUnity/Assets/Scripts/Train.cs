using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
Train script attached to a Train GameObject.
*/
public class Train : MonoBehaviour {

    public string id = "0";
    public string a_max = "0.7";
    public string schedule = "[]";

    public ParameterShower parameterShower;

    void OnMouseDown() {
        Dictionary<string, string> parameters = new Dictionary<string, string>() {
            {"a_max", a_max},
            {"schedule", schedule}
        };
        parameterShower.show(id, parameters);
    }   

}
