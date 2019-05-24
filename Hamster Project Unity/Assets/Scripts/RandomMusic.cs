using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusic : MonoBehaviour {

  public List<AudioSource> allMusic;
  private int currentSongIndex = 0;
  public AudioSource previousSong;

  public float newSongTime = 5;
  private float newSongTimer = 0;

	void Start () { StartNewSong(true); }
	
  void Update() {
    if(!previousSong.isPlaying) {
      if(newSongTimer > 0) { newSongTimer -= Time.deltaTime; }
      else { StartNewSong(); }
    }
  }

  void StartNewSong(bool firstTime = false) {
    //Get a new song from the current playlist
    int newSongIndex = Random.Range(0,allMusic.Count);
    //Play this random song.
    allMusic[newSongIndex].Play();
    //If more than one song...
    if(allMusic.Count > 1) {
      if(!firstTime) { 
        //Re add the previous song back to the list.
        allMusic.Add(previousSong);
      }
      //Assign this selected song.
      previousSong = allMusic[newSongIndex]; 
      //Remove the current song for the next random song selection.
      allMusic.RemoveAt(newSongIndex);
    }
    //Reset the new song timer
    newSongTimer = newSongTime;
  }

}
