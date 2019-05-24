using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flower : MonoBehaviour {

  public float sizeMin = 3;
  public float sizeMax = 6;
  public float anglemax = 15;

	// Use this for initialization
	void Start () {	
    float size = Random.Range(sizeMin,sizeMax);
    transform.localScale = new Vector3(size, size, size); 
    transform.Rotate(Random.Range(0,15),Random.Range(0,360),Random.Range(0,15),Space.World);
	}
	
}
