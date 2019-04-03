using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
Basic script to move around the camera.
*/
public class CameraMovement : MonoBehaviour {

    public float speed = 5.0f;


    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        transform.Translate(speed * new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")));
    }
}
