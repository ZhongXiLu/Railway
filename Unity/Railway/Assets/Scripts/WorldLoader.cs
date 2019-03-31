using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldLoader : MonoBehaviour {

    public GameObject groundPrefab;
    public GameObject straightPrefab;
    
    // Start is called before the first frame update
    void Start() {
        // Ground
        // TODO: make rectangle instead of square?
        // GameObject ground = Instantiate(groundPrefab, new Vector3(2500, 0, -2500), Quaternion.identity) as GameObject;
        // ground.transform.localScale = new Vector3(1000, 0, 1000);

        // Straights
        for(int i = 0; i < 3; i++) {
            GameObject straight = Instantiate(straightPrefab, new Vector3(0, 0, 10*i), Quaternion.identity) as GameObject;
            straight.gameObject.name = "AtompmId" + i;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
