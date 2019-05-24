using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ToMainMenu : MonoBehaviour {

  public void backToLevelSelect() {
    SceneManager.LoadScene("Title");
  }
 
}
