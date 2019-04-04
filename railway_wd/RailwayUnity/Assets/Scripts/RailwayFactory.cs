using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject createGround(int width, int height) {
        GameObject ground = Instantiate(groundPrefab, new Vector3(2.5f*width, 0, -2.5f*height), Quaternion.identity) as GameObject;
        ground.transform.localScale = new Vector3(width, 1, height);
        return ground;
    }

    public GameObject createTrain(string name, int x, int z) {
        GameObject train = Instantiate(trainPrefab, new Vector3(x, 0, z+5), Quaternion.identity) as GameObject;
        train.name = name;
        return train;
    }

    public GameObject createStraight(string name, int x, int z, int length) {
        GameObject straight = new GameObject();
        straight.name = name;
        for(int i = 0; i <= Mathf.RoundToInt(length/100); i++) {
            GameObject straightPart = Instantiate(straightPrefab, new Vector3(x, 0, z+80*i), Quaternion.identity) as GameObject;
            straightPart.transform.parent = straight.transform;
        }
        return straight;
    }

    public GameObject createStation(string name, int x, int z) {
        GameObject station = Instantiate(stationPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        station.name = name;
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
