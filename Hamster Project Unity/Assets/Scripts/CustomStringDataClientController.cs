using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyWiFi.Core;
using UnityEngine;
using UnityEngine.UI;

namespace EasyWiFi.ClientControls {
    [AddComponentMenu("EasyWiFiController/Client/DataControls/String")]
    public class CustomStringDataClientController : MonoBehaviour, IClientController {

        public string controlName = "StringData1";
        public InputField nameField;

        //we reuse the backchannel data types even though this is a forward channel
        StringBackchannelType stringData;
        string stringKey;

        //variable other script will modify via setValue to be sent across the network
        string value;

        // Use this for initialization
        void Awake() {
            stringKey = EasyWiFiController.registerControl(EasyWiFiConstants.CONTROLLERTYPE_STRING, controlName);
            stringData = (StringBackchannelType)EasyWiFiController.controllerDataDictionary[stringKey];
        }

        void Start() {
            nameField.text = PlayerPrefs.GetString("Name", "Hamster");
        }

        //here we grab the input and map it to the data list
        void Update() { mapInputToDataStream(); }

        //for properties DO NOT reset to default values becasue there isn't a default
        public void mapInputToDataStream() { 
            stringData.STRING_VALUE = value;
        }

        public void setName(string name) { 
            value = name; 
            PlayerPrefs.SetString("Name", name); 
        }
    }
}