using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ConnectionApp : MonoBehaviour {

	//private AndroidJavaObject plugin;

	[Header("Panels")]
	public PanelGroup[] panelgroups;
	private int panelIndex = 0;

	[Header("Text Assignments")]
	public Text accelerometerText;
	public Text gyroscopeText;
	public Text heartrateText;
	public Text breakTimerText;
	public Text breakButtonText;

	[Header("Acceleration Settings")]
	public float accThreshold = 0;
	private Vector3 oldA;	
	public float spacedOutTime;
	private float spacedOutTimer;

	[Header("Line Graph Settings")] 
	public LineRenderer xLine;
	public LineRenderer yLine;
	public LineRenderer zLine;
	public float lineMult;
	private List<Vector3> aQueue;
	private float queueTimer;
	public float queueTime;
	public int queueCount;

	[Header("Volume Checker")]
	public Transform volumeBox;
	public float volumeBoxScale = 1;
	public float loudnessThreshold;
	//public MicControlC mic;

	[Header("Break Timer")]
	private float breakTimer = 0;
	public float breakTime;
	public AudioSource breakStartSound;
	public AudioSource breakStopSound;	

	[Header("Hamster Settings")]
	public Animator hamster;
	public Material faceMat;
	public Texture[] faceTextures;
	private int faceInt;
	public Material bodyMat;
	public Texture[] bodyTextures;
	private int bodyInt;

	public void prevPanel() { setPanel(panelIndex-1); }
	public void nextPanel() { setPanel(panelIndex+1); }
	public void setPanel(int index) {
		panelIndex = (index) % panelgroups.Length;
		foreach(PanelGroup panelgroup in panelgroups) { 
			foreach(GameObject panel in panelgroup.panels) { panel.SetActive(false); }
		}
		foreach(GameObject panel in panelgroups[panelIndex].panels) { panel.SetActive(true); }
	}

	public void prevFaceTexture() { setFaceTexture(faceInt - 1); }
	public void nextFaceTexture() { setFaceTexture(faceInt + 1); }
	private void setFaceTexture(int index) {
		if(faceMat != null && faceTextures != null) {
			faceInt = index % faceTextures.Length;
			faceMat.mainTexture = faceTextures[faceInt];
			PlayerPrefs.SetInt("Face", faceInt);
		}
	}
	public void prevBodyTexture() { setBodyTexture(bodyInt - 1); }
	public void nextBodyTexture() { setBodyTexture(bodyInt + 1); }
	private void setBodyTexture(int index) {
		if(bodyMat != null && bodyTextures != null) {
			bodyInt = index % bodyTextures.Length;
			bodyMat.mainTexture = bodyTextures[bodyInt];
			PlayerPrefs.SetInt("Body", bodyInt);		
		}
	}

	public void startStopTimer() {
		if(breakTimer > 0) {
			breakButtonText.text = "Start Timer";
			breakStopSound.Play();
			breakTimer = 0;
		}
		else {
			breakButtonText.text = "Stop Timer";
			breakStartSound.Play();
			breakTimer = breakTime;
		}		
	}

	public void Start() {
		setFaceTexture(PlayerPrefs.GetInt("Face", 0));
		setBodyTexture(PlayerPrefs.GetInt("Body", 0));

		Input.gyro.enabled = true;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		spacedOutTimer = spacedOutTime;
		aQueue = new List<Vector3>();
		for(int i = 0; i < queueCount; i++) { aQueue.Add(new Vector3(0,0,0)); }
		/* HEART RATE DOES NOT WORK
		#if UNITY_ANDROID
				plugin = new AndroidJavaClass("jp.kshoji.unity.sensor.UnitySensorPlugin").CallStatic<AndroidJavaObject>("getInstance");
				plugin.Call("setSamplingPeriod", 100 * 1000); // refresh sensor 100 mSec each
				plugin.Call("startSensorListening", "heartrate");
		#endif
		*/
	}

	public void Update() {
		
		//TICKLE THE HAMSTER
		hamster.SetBool("tickle", ((Input.touchCount > 0) || (Input.GetMouseButton(0))));
		
		//Show UI
		Vector3 a = Input.acceleration;
		Quaternion g = Input.gyro.attitude;
		accelerometerText.text = "x: " + System.Math.Round(a.x,2) + 
		                         "\ny: " + System.Math.Round(a.y,2) +
		                         "\nz: " + System.Math.Round(a.z,2);
		gyroscopeText.text = "x: " + System.Math.Round(g.x,2) + 
		                     "\ny: " + System.Math.Round(g.y,2) + 
		                     "\nz: " + System.Math.Round(g.z,2) + 
		                     "\nw: " + System.Math.Round(g.w,2);
		
		//Show Graph
		if(queueTimer > 0) {
			queueTimer -= Time.deltaTime;
		} else {
			aQueue.RemoveAt(0);
			aQueue.Add(a);
			/*DEBUG MOUSE GRAPH
			aQueue.Add(new Vector3((Input.mousePosition.x-200f)/400f,
							   	   (Input.mousePosition.y-200f)/400f,
								   (Input.mousePosition.x-200f)/800f));*/
			queueTimer = queueTime;	
			UpdateLines();
		}                   

		//BUZZ IF TOO LOUD
		/* volumeBox.localScale = new Vector3(mic.loudness * volumeBoxScale,1,1);
		if(mic.loudness > loudnessThreshold) {
			Vibrate();
			Debug.Log("Loudness Threshold passed!");
		} */

		//BUZZ if no movement after ten minutes
		if(oldA != null) {
			//Check Movement, reset spaced out timer if movement detected
			if(Mathf.Abs(oldA.x - a.x) > accThreshold || Mathf.Abs(oldA.y - a.y) > accThreshold || Mathf.Abs(oldA.z - a.z) > accThreshold) {
				spacedOutTimer = spacedOutTime;
			}
		}
		oldA = a;
		if(spacedOutTimer > 0) {
			spacedOutTimer -= Time.deltaTime;
		} else {
			Vibrate();
			spacedOutTimer = spacedOutTime;	
			Debug.Log("Spaced Out Timer passed!");		
		}

		//Update timer
		if(breakTimer > 0) {
			breakTimer -= Time.deltaTime; 
			int cs = (int)(breakTimer*100f) % 100;
			int s = (int)(breakTimer) % 60;
			int m = (int)(breakTimer/60f) % 60;
			breakTimerText.text = "" + m.ToString().PadLeft(2,'0') + ":" + s.ToString().PadLeft(2,'0') + ":" + cs.ToString().PadLeft(2,'0');
		} else {
			breakTimer = 0;
			if (!breakTimerText.text.Equals("00:00:00")) {
				breakTimerText.text = "00:00:00";
				breakStopSound.Play();
			}
		}

		/* HEART RATE DOES NOT WORK
		#if UNITY_ANDROID
		if (plugin != null) {
			float[] sensorValue = plugin.Call<float[]>("getSensorValues", "heartrate");
			if (sensorValue != null) {
				Debug.Log("sensorValue:" + sensorValue[0]);
				heartrateText.text = "h: " + System.Math.Round(sensorValue[0],2);
			}
		}
		#endif
		*/
	}	

	public void UpdateLines() {
		xLine.SetVertexCount(queueCount);
		yLine.SetVertexCount(queueCount);
		zLine.SetVertexCount(queueCount);

		Vector3[] xs = new Vector3[queueCount];
		Vector3[] ys = new Vector3[queueCount];
		Vector3[] zs = new Vector3[queueCount];
		for(int i = 0; i < queueCount; i++) {
			xs[i] = new Vector3((1f/(float)queueCount)*i,aQueue[i].x*lineMult,0);
			ys[i] = new Vector3((1f/(float)queueCount)*i,aQueue[i].y*lineMult,0);
			zs[i] = new Vector3((1f/(float)queueCount)*i,aQueue[i].z*lineMult,0);
		}
		xLine.SetPositions(xs);
		yLine.SetPositions(ys);
		zLine.SetPositions(zs);
	}

	public void Vibrate() { 
#if UNITY_ANDROID
			Handheld.Vibrate(); 
#endif
	}

	void OnApplicationQuit () {
		/* #if UNITY_ANDROID
		if (plugin != null) {
			plugin.Call("terminate");
			plugin = null;
		}
		#endif */
	}
}