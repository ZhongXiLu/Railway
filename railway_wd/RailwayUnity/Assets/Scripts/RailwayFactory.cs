using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
RailwayFactory: responsible for creating the different kinds of tracks and trains.

Note that the all the tracks are 80 units long and 30 units wide, except the crossing, which is twice its size.
*/
public class RailwayFactory : MonoBehaviour {

    public GameObject groundPrefab;
    public GameObject straightPrefab;
    public GameObject stationPrefab;
    public GameObject turnoutPrefab;
    public GameObject junctionPrefab;
    public GameObject crossingPrefab;
    public GameObject trainPrefab;

    public ParameterShower parameterShower;

    public GameObject createGround(int width, int height) {
        GameObject ground = Instantiate(groundPrefab, new Vector3(2.5f*width, 0, -2.5f*height), Quaternion.identity) as GameObject;
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(width, 1, height);
        return ground;
    }

    IEnumerator modifyLabel(string labelName, string text, bool overrideLabel) {
        yield return new WaitForSeconds(0.01f);
        GameObject label = GameObject.Find(labelName);
        if(overrideLabel) {
            label.transform.Find("Text").gameObject.GetComponent<Text>().text = text;
        } else {
            label.transform.Find("Text").gameObject.GetComponent<Text>().text += "\n" + text;
        }
    }

    IEnumerator updateEndStationLabel(string labelName) {
        yield return new WaitForSeconds(0.01f);
        GameObject label = GameObject.Find(labelName);
        if(label.transform.Find("Text").gameObject.GetComponent<Text>().text.Contains("\n")) {
            string text = label.transform.Find("Text").gameObject.GetComponent<Text>().text;
            int count = int.Parse(text.Substring(text.Length-1, 1)) + 1;
            text = text.Substring(0, text.Length-1);
            label.transform.Find("Text").gameObject.GetComponent<Text>().text = text + count;
        } else {
            label.transform.Find("Text").gameObject.GetComponent<Text>().text += "\n#Trains arrived: 1";
        }
    }

    public GameObject createTrain(string id, float x, float z, string start, string end, string schedule, string a_max, string startStation) {
        GameObject train = Instantiate(trainPrefab, new Vector3(x, 0, z-25), Quaternion.identity) as GameObject;
        train.name = "Train_" + id;
        // Wait for label to be createn first before modifying it
        StartCoroutine(modifyLabel("Label " + train.name, "From " + start + " to " + end, false));
        train.GetComponent<Train>().train.id = "Train_" + id;
        train.GetComponent<Train>().parameterShower = parameterShower;
        train.GetComponent<Train>().train.schedule = schedule;
        train.GetComponent<Train>().train.a_max = a_max;
        train.GetComponent<Train>().currentTrack = startStation;
        return train;
    }

    public void destroyTrain(string train, string endStation) {
        Destroy(GameObject.Find("Label Train " + train));
        GameObject trainObj = GameObject.Find("Train_" + train);
        if(trainObj != null) {
            Destroy(trainObj);
        }
        // TODO: show that train reached end station (text, effect, ...)?
        StartCoroutine(updateEndStationLabel("Label " + endStation));
    }

    public GameObject createStraight(string name, int x, int z) {
        GameObject straight = Instantiate(straightPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        straight.name = name;
        return straight;
    }

    public GameObject createStation(string name, int x, int z, string stationName) {
        GameObject station = Instantiate(stationPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        station.name = name;
        // Wait for label to be createn first before modifying it
        StartCoroutine(modifyLabel("Label " + name, "Station: " + stationName, true));
        return station;
    }

    public GameObject createTurnout(string name, int x, int z) {
        GameObject turnout = Instantiate(turnoutPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        turnout.name = name;
        return turnout;
    }

    public GameObject createJunction(string name, int x, int z) {
        GameObject junction = Instantiate(junctionPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        junction.name = name;
        return junction;
    }

    public GameObject createCrossing(string name, int x, int z) {
        GameObject crossing = Instantiate(crossingPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        crossing.name = name;
        return crossing;
    }
}
