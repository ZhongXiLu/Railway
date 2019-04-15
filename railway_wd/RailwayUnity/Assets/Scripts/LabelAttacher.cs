using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LabelAttacher : MonoBehaviour {
    
    public GameObject textPrefab;

    GameObject text;

    void Start() {
        text = Instantiate(textPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        text.transform.SetParent(GameObject.Find("Canvas").transform);

        text.name = "Label " + transform.parent.name;
        text.transform.GetChild(1).gameObject.GetComponent<Text>().text = transform.parent.name;
    }

    void Update() {
        Vector3 namePos = Camera.main.WorldToScreenPoint(this.transform.position);
        text.transform.position = namePos;
    }
}
