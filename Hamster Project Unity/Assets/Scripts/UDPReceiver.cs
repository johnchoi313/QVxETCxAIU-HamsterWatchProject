﻿using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net;

public class UDPReceiver : MonoBehaviour {
   
    private string receiverMessage = null;
    public int receivePort;
    public bool autoStart = false;

    UdpClient receiver;
    Thread receiveThread;
    public int sleep = 50;
    public bool running = false;

    private float timer = 0;
    public float resetDelay = 10;
    public InputField PortField;
    public Text ConnectButton;

    public void startReceiveThread() {
        if(!running) {
            stopReceiverThread();
            Invoke("startReceiveThreadInvoke", 1.0f);
        }
    }    
    public void startReceiveThreadInvoke() {
        if(receivePort > 0) {
            running = true;
            if (receiveThread != null && receiveThread.IsAlive) { receiveThread.Abort(); }
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log("Receiver Started!");
            if(ConnectButton != null) { ConnectButton.text = "Restart UDP"; }
        }
    }
    public void stopReceiverThread() {
        if (receiver != null) { receiver.Close(); }
        running = false;
    }
    public string getMessage() {
        if (receiverMessage != null) {
            string message = receiverMessage;
            receiverMessage = null;
            return message;
        } else {
            return null;
        }
    }
    public void setPort(string port) { 
        if(!int.TryParse(port, out receivePort)) { receivePort = 0; } 
        PlayerPrefs.SetInt("ReceivePort", receivePort);
    }

    private void ReceiveData() {
        while (running) {
            receiver = new UdpClient(receivePort);
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = receiver.Receive(ref anyIP);
            receiverMessage = Encoding.UTF8.GetString(data);
            Debug.Log(receiverMessage);
            receiver.Close();
            Thread.Sleep(sleep);
        }
    }
    
    void Start() {
        //receivePort = PlayerPrefs.GetInt("ReceivePort", 9000);
        if(PortField != null) { PortField.text = "" + receivePort; }
        if(autoStart && receivePort > 0) { startReceiveThread(); }
    }
    void Update() {
        if (timer > 0) { timer -= Time.deltaTime; }
        else { timer = resetDelay; }
    }

    void OnApplicationQuit() {
        Debug.Log("Closed application.");
        stopReceiverThread();
    }
}
 