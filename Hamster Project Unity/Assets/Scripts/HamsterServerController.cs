using UnityEngine;
using System.Collections;
using EasyWiFi.Core;
using System;

namespace EasyWiFi.ServerControls {
    public class HamsterServerController : MonoBehaviour, IServerController {

        //Constants
        public EasyWiFiConstants.PLAYER_NUMBER playerNum = EasyWiFiConstants.PLAYER_NUMBER.Player1;
        public EasyWiFiConstants.AXIS dPadHorizontal = EasyWiFiConstants.AXIS.XAxis;
        public EasyWiFiConstants.AXIS dPadVertical = EasyWiFiConstants.AXIS.YAxis;
        
        //Runtime variables
        DpadControllerType[] dPad = new DpadControllerType[EasyWiFiConstants.MAX_CONTROLLERS];
        ButtonControllerType[] button = new ButtonControllerType[EasyWiFiConstants.MAX_CONTROLLERS];
        StringBackchannelType[] stringController = new StringBackchannelType[EasyWiFiConstants.MAX_CONTROLLERS];
        string message = "";

        public HamsterController hamster;
        public HamsterTextures hamsterTextures;
        int currentNumberControllers = 0;

        void OnEnable() {
            EasyWiFiController.On_ConnectionsChanged += checkForNewConnections;
            if (dPad[0] == null && EasyWiFiController.lastConnectedPlayerNumber >= 0) {
                EasyWiFiUtilities.checkForClient("DPad1", (int)playerNum, ref dPad, ref currentNumberControllers);
            }
            if (button[0] == null && EasyWiFiController.lastConnectedPlayerNumber >= 0) {     
                EasyWiFiUtilities.checkForClient("Button1", (int)playerNum, ref button, ref currentNumberControllers);   
            }
            if (stringController[0] == null && EasyWiFiController.lastConnectedPlayerNumber >= 0) {
                EasyWiFiUtilities.checkForClient("StringData1", (int)playerNum, ref stringController, ref currentNumberControllers);
            }
        }
        void OnDestroy() {
            EasyWiFiController.On_ConnectionsChanged -= checkForNewConnections;
        }

        public void checkForNewConnections(bool isConnect, int playerNumber) {
            EasyWiFiUtilities.checkForClient("DPad1", (int)playerNum, ref dPad, ref currentNumberControllers);
            EasyWiFiUtilities.checkForClient("Button1", (int)playerNum, ref button, ref currentNumberControllers);   
            EasyWiFiUtilities.checkForClient("StringData1", (int)playerNum, ref stringController, ref currentNumberControllers);    
        }

        // Update is called once per frame
        void Update() {
            for (int i = 0; i < currentNumberControllers; i++) {
                if (dPad[i] != null && dPad[i].serverKey != null && dPad[i].logicalPlayerNumber != EasyWiFiConstants.PLAYERNUMBER_DISCONNECTED) {
                    mapDataStructureToAction(i);
                }
                //Get names
                if (stringController[i] != null && stringController[i].serverKey != null && stringController[i].logicalPlayerNumber != EasyWiFiConstants.PLAYERNUMBER_DISCONNECTED) {
                    if(message != stringController[i].STRING_VALUE && stringController[i].STRING_VALUE != null) {
                        message = stringController[i].STRING_VALUE;
                        hamster.setNameTag(message);
                    }
                }
            }
        }

        public void mapDataStructureToAction(int index) {
            //Jump
            if(button[index].BUTTON_STATE_IS_PRESSED) { hamster.jump(); }
            //left/right movement
            float horizontal = 0f;
            float vertical = 0f;
            if (dPad[index].DPAD_LEFT_PRESSED) { horizontal = -1f; }
            if (dPad[index].DPAD_RIGHT_PRESSED) { horizontal = 1f; }
            if (dPad[index].DPAD_DOWN_PRESSED) { vertical = -1f; }
            if (dPad[index].DPAD_UP_PRESSED) { vertical = 1f; }
            Vector3 actionVector3 = EasyWiFiUtilities.getControllerVector3(horizontal, vertical, dPadHorizontal, dPadVertical);
            hamster.move(actionVector3.x);      
        }
    }
}
