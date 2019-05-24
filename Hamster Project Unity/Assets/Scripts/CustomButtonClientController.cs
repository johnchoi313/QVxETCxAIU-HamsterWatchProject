using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using EasyWiFi.Core;

namespace EasyWiFi.ClientControls {
    [AddComponentMenu("EasyWiFiController/Client/UserControls/Button")]
    public class CustomButtonClientController : MonoBehaviour, IClientController {

        public string controlName = "Button1";
        public ContinuousButton continuousButton;

        ButtonControllerType button;
        string buttonKey;

        // Use this for initialization
        void Awake() {
            buttonKey = EasyWiFiController.registerControl(EasyWiFiConstants.CONTROLLERTYPE_BUTTON, controlName);
            button = (ButtonControllerType)EasyWiFiController.controllerDataDictionary[buttonKey];            
        }
        void Update() { mapInputToDataStream(); }
        
        public void mapInputToDataStream() {
            button.BUTTON_STATE_IS_PRESSED = (continuousButton) ? continuousButton.isPressed : false;
        }
    }
}