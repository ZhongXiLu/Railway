using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/**
Creates a socket to send messages to the simulator.
Not to be confused with the other socket in the Main component.
*/
public class SocketManager : MonoBehaviour {

    private Socket listener;
    private Socket handler = null;

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
        }
    }

    /**
    Send a message to the simulator.
    @param message The message to be send.
    */
    public void send(string message) {
        if(handler != null) {
            byte[] msg = Encoding.UTF8.GetBytes(message);
            handler.Send(msg);
        }
    }
}
