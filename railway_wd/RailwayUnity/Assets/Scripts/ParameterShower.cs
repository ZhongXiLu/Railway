using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
Class for creating parameter forms for game objects.
*/
public class ParameterShower : MonoBehaviour {

    public GameObject formPrefab;
    public GameObject inputPrefab;
    string idObj = "0";    // the id of the gameobject of which it is showing the parameters

    SocketManager socketManager;

    void Start() {
        socketManager = GameObject.Find("SocketManager").GetComponent<SocketManager>();
    }

    /**
    Update the game object by
        (1) TODO: updating the attributes of the gameobject in unity
        (2) updating the attributes of the models in DEVS
    */
    void updateObject() {

        // Parse form data
        GameObject form = GameObject.Find("Parameters Form");
        GameObject content = form.transform.Find("Viewport/Content").gameObject;

        // Loop over all elements in form
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        foreach(Transform child in content.transform) {
            // Loop to get input value
            foreach(Transform child2 in child) {
                if(child2.name == "InputField") {
                    parameters.Add(child.name, child2.gameObject.GetComponent<InputField>().text);
                }
            }
        }

        // TODO: for other data objects
        GameObject gameObj = GameObject.Find(idObj);
        if(gameObj.GetComponent<Train>() != null) {
            Train train = gameObj.GetComponent<Train>();
            TrainData trainData = train.train;

            foreach(KeyValuePair<string, string> parameter in parameters) {
                trainData.GetType().GetField(parameter.Key).SetValue(trainData, parameter.Value);
            }
            // Debug.Log(JsonUtility.ToJson(trainData));
            socketManager.send(string.Format("UPDATE_{0} {1}", train.currentTrack, JsonUtility.ToJson(trainData)));
        }
        

    }

    /**
    Show (parameters) form to user where they can live tweak some parameters.
    @param title        Title of the form (should be the ID of the object).
    @param parameters   A dictionary of all the parameters names with their value that needs to be shown.
    */
    public void show(string title, Dictionary<string, string> parameters) {
        idObj = title;

        // Remove the previous form if there is any
        GameObject oldForm = GameObject.Find("Parameters Form");
        if(oldForm != null) {
            Destroy(oldForm);
        }

        // Create form object
        GameObject form = Instantiate(formPrefab) as GameObject;
        form.transform.SetParent(GameObject.Find("Canvas").transform);
        form.name = "Parameters Form";
        form.GetComponent<RectTransform>().anchoredPosition = new Vector3(490, -190, 0);

        // Set title form
        form.transform.Find("Viewport/Content/Title").gameObject.GetComponent<Text>().text = title;

        // Set input field for each parameter
        GameObject content = form.transform.Find("Viewport/Content").gameObject;
        int index = 0;  // manually keep index
        foreach(KeyValuePair<string, string> parameter in parameters) {
            GameObject inputField = Instantiate(inputPrefab) as GameObject;
            inputField.transform.SetParent(content.transform);
            inputField.name = parameter.Key;
            inputField.GetComponent<RectTransform>().anchoredPosition = new Vector3(5, -60*index, 0);

            inputField.transform.Find("Text").gameObject.GetComponent<Text>().text = parameter.Key;
            inputField.transform.Find("InputField").GetComponent<InputField>().text = parameter.Value;

            index++;
        }

        // Fix position of 'Save' button
        GameObject saveButton = form.transform.Find("Viewport/Content/Button").gameObject;
        saveButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(5, -60*(index-1), 0); 
        saveButton.GetComponent<Button>().onClick.AddListener(updateObject);
    }
}
