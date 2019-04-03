using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
TrackFactory: responsible for creating the different kinds of tracks.

Note that the all the tracks are 80 units long and 30 units wide, except the crossing, which is twice its size.
*/
public class TrackFactory : MonoBehaviour {

    public GameObject groundPrefab;
    public GameObject straightPrefab;
    public GameObject stationPrefab;
    public GameObject turnoutPrefab;
    public GameObject junctionPrefab;
    public GameObject crossingPrefab;

    public GameObject createGround(int width, int height) {
        GameObject ground = Instantiate(groundPrefab, new Vector3(2.5f*width, 0, -2.5f*height), Quaternion.identity) as GameObject;
        ground.transform.localScale = new Vector3(width, 1, height);
        return ground;
    }

    public GameObject createStraight(string name, int x, int z) {
        GameObject straight = Instantiate(straightPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        straight.name = name;
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
