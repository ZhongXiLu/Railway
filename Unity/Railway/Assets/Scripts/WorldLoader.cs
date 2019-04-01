using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
WorldLoader

Note that the all the tracks are 80 units long and 15 units wide.
*/
public class WorldLoader : MonoBehaviour {

    public GameObject groundPrefab;
    public GameObject straightPrefab;
    public GameObject stationPrefab;
    public GameObject turnoutPrefab;
    public GameObject junctionPrefab;
    public GameObject crossingPrefab;

    GameObject createGround(int width, int height) {
        GameObject ground = Instantiate(groundPrefab, new Vector3(2.5f*width, 0, -2.5f*height), Quaternion.identity) as GameObject;
        ground.transform.localScale = new Vector3(width, 1, height);
        return ground;
    }

    GameObject createStraight(string name, int x, int z) {
        GameObject straight = Instantiate(straightPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        straight.name = name;
        return straight;
    }

    GameObject createStation(string name, int x, int z) {
        GameObject station = Instantiate(stationPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        station.name = name;
        return station;
    }

    GameObject createTurnout(string name, int x, int z) {
        GameObject turnout = Instantiate(turnoutPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        turnout.name = name;
        return turnout;
    }

    GameObject createJunction(string name, int x, int z) {
        GameObject junction = Instantiate(junctionPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        junction.name = name;
        return junction;
    }

    GameObject createCrossing(string name, int x, int z) {
        GameObject crossing = Instantiate(crossingPrefab, new Vector3(x, 0, z), Quaternion.identity) as GameObject;
        crossing.name = name;
        return crossing;
    }

    // Start is called before the first frame update
    void Start() {
        createGround(1000, 1000);

        createJunction("atompmId", 0, -80);
        createStraight("atompmId", 0, 0);
        createTurnout("atompmId", 0, 80);
        createStation("atompmId", 0, 160);
        createCrossing("atompmId", 30, 160);
    }

    // Update is called once per frame
    void Update() {
        
    }
}
