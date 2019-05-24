using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.SimpleAndroidNotifications.Data;
using Assets.SimpleAndroidNotifications.Enums;
using Assets.SimpleAndroidNotifications.Helpers;

public class HamsterWatch : MonoBehaviour {

	[Header("Networking")]
  public UDPSender udpSender;
  public HamsterData hamsterData;
  public ServerFinder serverFinder;

	[Header("Controller")]
  public SimpleInputNamespace.Dpad dpad;
	private bool actionPressed = false;  
  private bool jumpPressed = false;
 
	[Header("Panels")]
	public PanelGroup[] panelgroups;
	private int panelIndex = 0;

  [Header("Swiping")]
  public float swipeDistance = 150;
  private float swipeX1, swipeX2;

	[Header("UI Assignments")]
  public Text breakTimerText;
	public Text breakButtonText;
  public InputField nameField;
  public Slider buzzerSlider;
  public Text buzzerText;
  
	[Header("Break Timer")]
  public int breakReset;
	public int breakTime;
  public int breaks;
  public int day = 0;
  public bool breakActive = false;
  private List<BreakData> breakDatas;
  private System.DateTime breakStartedTime;
  public Assets.SimpleAndroidNotifications.HamsterNotifier notifier;

	public AudioSource breakStartSound;
	public AudioSource breakStopSound;	
  public AudioSource equipSound;
  public AudioSource menuSound;

	[Header("Hamster Settings")]
	public Animator hamster;
	public HamsterTextures hamsterTextures;
	private int faceInt, bodyInt = 0;

  //---SWITCHING WINDOWS---//
	public void prevPanel() { setPanel(panelIndex-1); }
	public void nextPanel() { setPanel(panelIndex+1); }
	public void setPanel(int index) {
    menuSound.Play();
		panelIndex = (index) % panelgroups.Length;
		foreach(PanelGroup panelgroup in panelgroups) { 
			foreach(GameObject panel in panelgroup.panels) { panel.SetActive(false); }
		}
		foreach(GameObject panel in panelgroups[panelIndex].panels) { panel.SetActive(true); }
	}
  //---SETTING FACE TEXTURES---//
	public void prevFaceTexture() { 
		faceInt = (faceInt - 1 + hamsterTextures.faceTextures.Length) % hamsterTextures.faceTextures.Length; 
    PlayerPrefs.SetInt("Face", faceInt);
  	hamsterTextures.setFaceTexture(0, faceInt); 
		hamsterData.f = faceInt;
    equipSound.Play();
	}
  public void nextFaceTexture() { 
		faceInt = (faceInt + 1 + hamsterTextures.faceTextures.Length) % hamsterTextures.faceTextures.Length;
    PlayerPrefs.SetInt("Face", faceInt);
  	hamsterTextures.setFaceTexture(0, faceInt); 
  	hamsterData.f = faceInt;
    equipSound.Play();
  }
  //---SETTING BODY TEXTURES---//
  public void prevBodyTexture() { 
  	bodyInt = (bodyInt - 1 + hamsterTextures.faceTextures.Length) % hamsterTextures.bodyTextures.Length;
    PlayerPrefs.SetInt("Body", bodyInt);                
  	hamsterTextures.setBodyTexture(0, bodyInt); 
		hamsterData.b = bodyInt;
    equipSound.Play();
  }
  public void nextBodyTexture() {
  	bodyInt = (bodyInt + 1 + hamsterTextures.faceTextures.Length) % hamsterTextures.bodyTextures.Length;
    PlayerPrefs.SetInt("Body", bodyInt);        
  	hamsterTextures.setBodyTexture(0, bodyInt); 
  	hamsterData.b = bodyInt;
    equipSound.Play();
  }
  //---MISCELLANEOUS HELPER FUNCTIONS---//
	public void Vibrate() { 
		#if UNITY_ANDROID
		Handheld.Vibrate(); 
		#endif
	}
  //---SETTING BREAK TIMER---//
  public void useBreak() {
    if(!breakActive) {
      if(breaks > 0) { 
        breaks--;
        PlayerPrefs.SetInt("Breaks", breaks);
        breakStartedTime = System.DateTime.Now;
        notifier.ScheduleBreakTimer(breakTime);
        breakStartSound.Play();
        breakActive = true;
        //Add break to used list if not duplicate.
        int d = (int)System.DateTime.Now.DayOfWeek;
        int h = (int)System.DateTime.Now.Hour;
        int m = (int)System.DateTime.Now.Minute;
        bool duplicate = false;
        foreach(BreakData breakData in breakDatas) {
          if(breakData.d == d && breakData.h == h && breakData.m == m) { duplicate = true; break; }
        }
        if(!duplicate) { breakDatas.Add(new BreakData(d, h, m)); }
      }
    } else {
      notifier.CancelAll();
      breakTimerText.text = "00:00:00";
      breakStopSound.Play();
      breakActive = false;
    }
  } 
  public void resetBreaks() {
    PlayerPrefs.SetInt("Day", (int)System.DateTime.Now.Day);
    day = (int)System.DateTime.Now.Day;
    PlayerPrefs.SetInt("Breaks", breakReset);
    breaks = breakReset;
    breakDatas = new List<BreakData>();
  }
  //Public Void
  public void startBuzzer() { 
    notifier.ScheduleBuzzer((int)buzzerSlider.value);
  }
  public void stopBuzzer() { 
    notifier.CancelAll(); 
  }

	//---SETTING CONTROLS---//
  public void setAction(bool pressed) { hamsterData.a = pressed?1:0; if(pressed) { actionPressed = true; } }
  public void setJump(bool pressed) { hamsterData.j = pressed?1:0; if(pressed) { jumpPressed = true; } }
  public void setName(string name) { hamsterData.n = name; PlayerPrefs.SetString("Name", name); }

  public void OnApplicationPause(bool pauseStatus) {
      if(!pauseStatus) {
        day = PlayerPrefs.GetInt("Day", (int)System.DateTime.Now.Day);
        if(day != (int)System.DateTime.Now.Day) { resetBreaks(); }
      }
  }

	public void Start() {
    //Update break information
    day = PlayerPrefs.GetInt("Day", (int)System.DateTime.Now.Day);
    breaks = PlayerPrefs.GetInt("Breaks", breakReset);
    breakDatas = new List<BreakData>();
    //Initialize Hamster data
    hamsterData = new HamsterData(); 
    hamsterData.k = UnityEngine.Random.Range(0, 1000000);
		hamsterData.f = PlayerPrefs.GetInt("Face", 0);
		hamsterData.b = PlayerPrefs.GetInt("Body", 0);
		hamsterData.n = PlayerPrefs.GetString("Name", "Player");
    //Update Hamster
		nameField.text = hamsterData.n;
    hamsterTextures.setFaceTexture(0,PlayerPrefs.GetInt("Face", 0));
    hamsterTextures.setBodyTexture(0,PlayerPrefs.GetInt("Body", 0));
  }
	public void Update() {
    //update swiping
    checkSwiping();

		//TICKLE THE HAMSTER
		hamster.SetBool("tickle", ((Input.touchCount > 0) || (Input.GetMouseButton(0))));
		
    //If next day, reset breaks
    if(day != (int)System.DateTime.Now.Day) { resetBreaks(); }
    //Hotkey Reset Breaks (Ctrl + R or Shift + R)
    if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) { resetBreaks(); }
    if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) { resetBreaks(); }
    
    //Update break timer
    System.TimeSpan breakTimer = (breakActive) ? System.DateTime.Now.Subtract(breakStartedTime) : TimeSpan.FromSeconds(breakTime);
    if(breakTimer.TotalSeconds < breakTime) {
      breakButtonText.text = "Stop Break\n(" + breaks + " left)";
      int cs = (int)(TimeSpan.FromSeconds(breakTime).Subtract(breakTimer).Milliseconds)/10 % 100;
			int s = (int)(TimeSpan.FromSeconds(breakTime).Subtract(breakTimer).Seconds) % 60;
			int m = (int)(TimeSpan.FromSeconds(breakTime).Subtract(breakTimer).Minutes) % 60;
    	breakTimerText.text = "" + m.ToString().PadLeft(2,'0') + ":" + s.ToString().PadLeft(2,'0') + ":" + cs.ToString().PadLeft(2,'0');
		} else {
      if (breakActive) {
        breakTimerText.text = "00:00:00";
        breakStopSound.Play();
      }
      breakActive = false;
      breakButtonText.text = "Start Break\n(" + breaks + " left)";
		}
    buzzerText.text = "" + (int)buzzerSlider.value + " min";

    //Send hamster control message
    hamsterData.x = (int)(dpad.xAxis.value * 1000);
    hamsterData.y = (int)(dpad.yAxis.value * 1000);
    if(actionPressed) { hamsterData.a = 1; }
    if(jumpPressed) { hamsterData.j = 1; }
    hamsterData.IP = udpSender.IP;
    hamsterData.bd = breakDatas;

    string message = JsonUtility.ToJson(hamsterData);
    udpSender.sendMessage(message);
    
    jumpPressed = false;
    actionPressed = false;
	}
  
  private void checkSwiping() {
    if(Input.GetMouseButtonDown(0)) { swipeX1 = Input.mousePosition.x; }
    if(Input.GetMouseButtonUp(0))  { 
      swipeX2 = Input.mousePosition.x;
      Debug.Log("Swipe Distance: " + (swipeX2 - swipeX1).ToString() );
      if(swipeX1 - swipeX2 > swipeDistance) { Debug.Log("Swiped Left!"); nextPanel(); }
    }
  }	

}

[System.Serializable]
public class PanelGroup { public GameObject[] panels; }	

[System.Serializable]
public class BreakData {
  public int d = 0; //Day (Mon-Fri)
  public int h = 0; //Hour
  public int m = 0; //Min

  public BreakData(int D, int H, int M) { d = D; h = H; m = M; }
}
