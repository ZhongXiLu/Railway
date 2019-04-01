using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordLoader : MonoBehaviour {

    public TrackFactory trackFactory;

    void Start() {

        trackFactory.createGround(1000, 1000);

        trackFactory.createJunction("atompmId", 0, -80);
        trackFactory.createStraight("atompmId", 0, 0);
        trackFactory.createTurnout("atompmId", 0, 80);
        trackFactory.createStation("atompmId", 0, 160);
        trackFactory.createCrossing("atompmId", 30, 160);
    }
}
