using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyWiFi.Core;

namespace EasyWiFi.ClientControls {

    [AddComponentMenu("EasyWiFiController/Client/Connection/ConnectionWidget")]
    public class CustomConnectionClientController : MonoBehaviour {
        
        private bool justStateChange;

        public Text connectText;

        void Update() {
            if (EasyWiFiController.clientState == EasyWiFiConstants.CURRENT_CLIENT_STATE.NotConnected) {
                if(connectText != null) { connectText.text = "Connect"; }
            } else if (EasyWiFiController.clientState == EasyWiFiConstants.CURRENT_CLIENT_STATE.SendingControllerData) {
                if(connectText != null) { connectText.text = "Disconnect"; }
            } else {
                if(connectText != null) { connectText.text = "Connecting..."; }
            }
        }

        public bool isConnected() {
            return (EasyWiFiController.clientState == EasyWiFiConstants.CURRENT_CLIENT_STATE.SendingControllerData);
        }
        public void connectOrDisconnect() {
            connect();
            disconnect();
        }
        public void connect() {
            if (!justStateChange && EasyWiFiController.clientState == EasyWiFiConstants.CURRENT_CLIENT_STATE.NotConnected) {
                //not connected currently so try to connect
                justStateChange = true;
                EasyWiFiController.checkForServer();
                Invoke("resetDisconnection", 2f);
            }
        }
        public void disconnect() {
            if (!justStateChange && EasyWiFiController.clientState == EasyWiFiConstants.CURRENT_CLIENT_STATE.SendingControllerData) {
                //connected currently so disconnect
                justStateChange = true;
                EasyWiFiController.sendDisconnect(EasyWiFiController.createDisconnectMessage());
                Invoke("resetDisconnection", 2f);
            }
        }
        void resetDisconnection() { justStateChange = false; }
    }
}