using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/**
Class for creating parameter forms for game objects.
*/
public class ParameterShower : MonoBehaviour {

    public GameObject formPrefab;
    public GameObject inputPrefab;

    private Socket listener;
    private Socket handler = null;

    bool sent = false;

    void Start() {
        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 11001);
        // Debug.Log(String.Format("Listening on : {0}:{1}", ipAddress, 11001));

        listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        listener.Blocking = false;
        listener.Bind(remoteEndPoint);
        listener.Listen(1);
    }

    void Update() {
        if(handler == null) {
            try {
                handler = listener.Accept();
                handler.Blocking = false;
                // Debug.Log("Accepted a connection... again");
            } catch(SocketException) {
                // since we're not blocking, no connection has been made, just continue to next frame...
            }
        } else {
            // TODO: press button => send message to simulator
            if(!sent) {
                byte[] msg = Encoding.UTF8.GetBytes("AYAYA");
                handler.Send(msg);
                sent = true;
            }
        }
    }

    /**
    Show (parameters) form to user where they can live tweak some parameters.
    @param title        Title of the form (should be the ID of the object).
    @param parameters   A dictionary of all the parameters names with their value that needs to be shown.
    */
    public void show(string title, Dictionary<string, string> parameters) {

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
        form.transform.Find("Viewport/Content/Button").gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(5, -60*(index-1), 0); 

    }
}
