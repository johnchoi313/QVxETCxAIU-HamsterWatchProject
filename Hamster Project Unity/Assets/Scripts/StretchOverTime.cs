using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchOverTime : MonoBehaviour {

  public Vector3 stretchFactor;
  public Vector3 stretchOffset;
  public Vector3 stretchTime;
	
	void Update () {
    float x = stretchFactor.x * Mathf.Sin(stretchTime.x * Time.time + stretchOffset.x) + 1;
    float y = stretchFactor.y * Mathf.Sin(stretchTime.y * Time.time + stretchOffset.y) + 1;
    float z = stretchFactor.z * Mathf.Sin(stretchTime.z * Time.time + stretchOffset.z) + 1;
	  transform.localScale = new Vector3(x,y,z); 
	}

}
