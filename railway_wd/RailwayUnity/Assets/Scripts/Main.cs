using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Main : MonoBehaviour {

    public float simulationTimeScaleFactor = 1.0f;
    public bool simulateLive = false;

    public GameObject canvas;
    public WorldLoader worldLoader;
    public TraceParser traceParser;

    // Credits to Simon Van Mierlo for providing the base code for the sockets :)
    private Socket listener;
    private Socket handler = null;
    private static int PORT = 11000;
    private string data = null;
    private byte[] bytes = new Byte[1024];

    void Start() {
        if(!simulateLive) {
            worldLoader.loadWorld("railway");
            traceParser.loadTrace("railway_log");
            
        } else {
            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the
            // host running the application.
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, PORT);
            Debug.Log(String.Format("Listening on : {0}:{1}", ipAddress, PORT));

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Blocking = false;
            listener.Bind(remoteEndPoint);
            listener.Listen(1);
        }
    }

    void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            canvas.GetComponent<Canvas>().enabled = !canvas.GetComponent<Canvas>().enabled;
        }

        if(simulateLive) {
            if(handler == null) {
                try {
                    handler = listener.Accept();
                    handler.Blocking = false;
                    Debug.Log("Accepted a connection...");
                } catch(SocketException) {
                    // since we're not blocking, no connection has been made, just continue to next frame...
                }
            } else {
                while(handler.Available > 0) {
                    data = null;
                    while(true) {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if(data.IndexOf("<EOF>") > -1) {
                            break;
                        }
                    }

                    string[] stringSeparator = new string[]{"<EOF>"};
                    string[] commands = data.Split(stringSeparator, StringSplitOptions.None);

                    foreach(string command in commands) {
                        if(command == "Hello World!") {
                            worldLoader.loadWorld("railway");
                        } else {
                            traceParser.simulateCommand(command, 1f);
                        }
                    }
                }
            }

        } else {
            // Running simulation from tracefile
            while(traceParser.nextCommand()) {
                float nextTimestamp = traceParser.nextTimestamp()/simulationTimeScaleFactor;
                if(nextTimestamp <= Time.realtimeSinceStartup) {
                    traceParser.simulateNextCommand(simulationTimeScaleFactor);
                } else {
                    // next command in not in time (wait for next frames...)
                    break;
                }
            }
        }
    }
}
