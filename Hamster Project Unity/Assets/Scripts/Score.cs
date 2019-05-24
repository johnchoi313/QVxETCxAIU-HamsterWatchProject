using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

  public Text ScoreText;
  private int highScore;
  
  public void incrementScore() { highScore++; PlayerPrefs.SetInt("Score",highScore); }
	void Start () { highScore = PlayerPrefs.GetInt("Score", 0); }	
  void Update () { ScoreText.text = "Score:" + highScore; }
}
