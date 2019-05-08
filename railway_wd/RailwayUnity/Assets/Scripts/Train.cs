using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrainData {
    public string id = "Train_0"; // name of gameobject
    public string a_max = "0.7";
    public string schedule = "";
}

/**
Train script attached to a Train GameObject.
*/
public class Train : MonoBehaviour {

    public TrainData train = new TrainData();
    public ParameterShower parameterShower;
    public string currentTrack = "0";

    void OnMouseDown() {
        Dictionary<string, string> parameters = new Dictionary<string, string>() {
            {"a_max", train.a_max},
            {"schedule", train.schedule}
        };
        parameterShower.show(train.id, parameters);
    }   

}
