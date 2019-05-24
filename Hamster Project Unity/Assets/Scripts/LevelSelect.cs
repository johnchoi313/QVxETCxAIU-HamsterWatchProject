using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour {

  public float distance = 8;

  public int levelIndex = 0;

  public Transform[] levels;
  public Text levelTitle;
  public float sizeMult = 1.1f;
  private Vector3 oldSize;
  private Vector3 newSize;

  public float sizeSpeed = 1;
  public float newZ = 1;

  public Transform cam;
  public float camSpeed = 1;

  public AudioSource menu;
  public AudioSource select;

  public GameObject loading;

	// Use this for initialization
	void Start () {
		oldSize = levels[0].localScale;
    newSize = levels[0].localScale * sizeMult;
	}
	
	// Update is called once per frame
	void Update () {
		
    //Keys to navigate levels
    if(Input.GetKeyDown(KeyCode.Return)) { selectLevel(); }
    if(Input.GetKeyDown(KeyCode.LeftArrow)) { prevLevel(); }
    if(Input.GetKeyDown(KeyCode.RightArrow)) { nextLevel(); }
    
    //Update cam position
    cam.position = Vector3.Lerp(cam.position, new Vector3(levelIndex*distance,cam.position.y,cam.position.z), Time.deltaTime * camSpeed);

    //Update level size
    for(int l = 0; l < levels.Length; l++) {
      if(l == levelIndex) {
        levels[l].localScale = Vector3.Lerp(levels[l].localScale, newSize, Time.deltaTime * sizeSpeed);
        levels[l].position = Vector3.Lerp(levels[l].position, new Vector3(levels[l].position.x,levels[l].position.y,newZ), Time.deltaTime * sizeSpeed);
      } else {
        levels[l].localScale = Vector3.Lerp(levels[l].localScale, oldSize, Time.deltaTime * sizeSpeed);
        levels[l].position = Vector3.Lerp(levels[l].position, new Vector3(levels[l].position.x,levels[l].position.y,0), Time.deltaTime * sizeSpeed);
      }
    }

	}

  public void nextLevel() {
    menu.Play();
    levelIndex++; if(levelIndex >= levels.Length) { levelIndex = 0; }
    levelTitle.text = levels[levelIndex].name;
  }

  public void prevLevel() {
    menu.Play();
    levelIndex--; if(levelIndex < 0) { levelIndex = levels.Length-1; }
    levelTitle.text = levels[levelIndex].name;
  }

  public void selectLevel() {
    select.Play();
    loading.SetActive(true);
    Invoke("loadLevel", 0.1f);
  }

  public void loadLevel() {
    SceneManager.LoadScene(levels[levelIndex].name);
  }
 
  public void quit() {
    Debug.Log("Quit!");
    Application.Quit();
  }

}
