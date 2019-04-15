using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public float simulationTimeScaleFactor = 1.0f;

    public GameObject canvas;
    public WorldLoader worldLoader;
    public TraceParser traceParser;

    IEnumerator simulate() {
        float currentTime = 0.0f;
        while(traceParser.nextCommand()) {
            float nextTimestamp = traceParser.nextTimestamp()/simulationTimeScaleFactor;
            if(currentTime != nextTimestamp) {
                yield return new WaitForSeconds(nextTimestamp - currentTime);
                currentTime = nextTimestamp;
            }
            traceParser.simulateNextCommand(simulationTimeScaleFactor);
        }
    }

    void Start() {
        worldLoader.loadWorld("railway");

        traceParser.loadTrace("railway_log");
        StartCoroutine(simulate());
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            canvas.GetComponent<Canvas>().enabled = !canvas.GetComponent<Canvas>().enabled;
        }
    }
}
