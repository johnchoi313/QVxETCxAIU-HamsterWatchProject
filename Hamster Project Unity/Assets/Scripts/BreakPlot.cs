using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakPlot : MonoBehaviour {

  public int day = 0;
  public List<BreakList> breakLists;

  /*
	// Use this for initialization
	void Start () { 
    day = PlayerPrefs.GetInt("Day", (int)System.DateTime.Now.Day); 
    breakLists = JsonUtility.FromJson<BreakList>(PlayerPrefs.GetString("BreakLists", new List<BreakList>()));
  }
	
  public void SaveCSVPlot() {
    //Update the day
    PlayerPrefs.SetInt("Day", (int)System.DateTime.Now.Day);
    day = (int)System.DateTime.Now.Day;
    //Collect list of plots
    List<int> Mon = new List<int>(); 
    List<int> Tue = new List<int>(); 
    List<int> Wed = new List<int>(); 
    List<int> Thu = new List<int>(); 
    List<int> Fri = new List<int>(); 
    foreach(BreakList breakList in breakLists) {
      foreach(int t in breakList.M) { Mon.Add(t); }
      foreach(int t in breakList.T) { Tue.Add(t); }
      foreach(int t in breakList.W) { Wed.Add(t); }
      foreach(int t in breakList.H) { Thu.Add(t); }
      foreach(int t in breakList.F) { Fri.Add(t); }
    }
    //Convert the list to a string
    string CSVString = "Monday;";
    foreach(int t in Mon) { CSVString += "" + t + ";"; }
    CSVString += "\r\nTuesday;";
    foreach(int t in Tue) { CSVString += "" + t + ";"; }
    CSVString += "\r\nWednesday;";
    foreach(int t in Wed) { CSVString += "" + t + ";"; }
    CSVString += "\r\nThursday;";
    foreach(int t in Thu) { CSVString += "" + t + ";"; }
    CSVString += "\r\nFriday;";
    foreach(int t in Fri) { CSVString += "" + t + ";"; }
    //Write the string to path
    path = Application.dataPath + "/" + System.DateTime.Now.ToString("MM-dd-yy");
    StreamWriter writer = new StreamWriter(path, false); //true to append, false to overwrite
    writer.WriteLine(CSVString); //Filename is palette name
    writer.Close();
    //Clear the list
    breakLists = new List<BreakList>();
    PlayerPrefs.SetString("BreakLists", JsonUtility.ToJson(breakLists));
  }

	// Update is called once per frame
	void Update () {
	    //If day is monday and the previous graph has not been saved, save it
      if(day != (int)System.DateTime.Now.Day) {}	
	}
  */

}

[System.Serializable]
public class BreakList{
  public List<int> M;
  public List<int> T;
  public List<int> W;
  public List<int> H;
  public List<int> F;
}