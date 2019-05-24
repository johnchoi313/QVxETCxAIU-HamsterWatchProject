using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

//---HOW IT WORKS:---//

public class ServerFinder : MonoBehaviour {

    public UDPSender udpSender;
    public UDPReceiver serverFinder;
    public float serverFindTime = 3.0f;
    private float serverFindTimer = 3.0f;
    private bool connectedToServer = false;

    void Update() {
        //Found a server broadcast
        string serverIP = serverFinder.getMessage();
        if(serverIP != null) {
            if(!connectedToServer) { 
                udpSender.IP = serverIP; 
                Debug.Log("Found server at " + serverIP + "!"); 
            }
            serverFindTimer = serverFindTime; 
            connectedToServer = true;
        }
        //If server broadcast is not found, assume server is disconnected
        if(serverFindTimer > 0) { serverFindTimer -= Time.deltaTime; } 
        else { 
            connectedToServer = false; 
            serverIP = null;
        }
    }


}
