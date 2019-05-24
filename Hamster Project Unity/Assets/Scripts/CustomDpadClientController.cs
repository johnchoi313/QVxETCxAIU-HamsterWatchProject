using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyWiFi.Core;

namespace EasyWiFi.ClientControls {
    [AddComponentMenu("EasyWiFiController/Client/UserControls/Dpad")]
    public class CustomDpadClientController : MonoBehaviour, IClientController {

        public string controlName = "DPad1";
        public ContinuousButton up;
        public ContinuousButton down;
        public ContinuousButton left;
        public ContinuousButton right;

        DpadControllerType dPad;
        string dPadKey;
        
        // Use this for initialization
        void Awake() {
            dPadKey = EasyWiFiController.registerControl(EasyWiFiConstants.CONTROLLERTYPE_DPAD, controlName);
            dPad = (DpadControllerType)EasyWiFiController.controllerDataDictionary[dPadKey];
        }
        void Update() { mapInputToDataStream(); }

        public void mapInputToDataStream() {
            dPad.DPAD_UP_PRESSED = (up) ? up.isPressed : false;
            dPad.DPAD_DOWN_PRESSED = (down) ? down.isPressed : false;
            dPad.DPAD_LEFT_PRESSED = (left) ? left.isPressed : false;
            dPad.DPAD_RIGHT_PRESSED = (right) ? right.isPressed : false;
        }
    }
}