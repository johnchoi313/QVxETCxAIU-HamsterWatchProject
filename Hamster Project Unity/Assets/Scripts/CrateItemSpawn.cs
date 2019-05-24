using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateItemSpawn : MonoBehaviour {

  public GameObject[] itemList;
  public GameObject crateSplode;

  private bool destroyed = false;
  public float timer = 5.0f;
  
	// Update is called once per frame
	void Update () {
    if(timer > 0) { timer -= Time.deltaTime; } 
    else if(!destroyed) {
      //Instantiate Crate Explosion
      if(crateSplode != null) { Destroy(Instantiate(crateSplode, transform.position, Quaternion.identity), 2); }
      //Play sound
      gameObject.GetComponent<AudioSource>().Play();
      //Instantiate item
      Instantiate(itemList[Random.Range(0,itemList.Length)], transform.position - new Vector3(0,transform.position.y - 0.3f,0),Quaternion.identity);  
      //Destroy this game object
      Destroy(gameObject, 0.2f); destroyed = true;
    }
  }
}
