using UnityEngine;
using System.Collections;

// PLEASE NOTE! THIS SCRIPT IS FOR DEMO PURPOSES ONLY :) //

public class RotateCamera : MonoBehaviour {

  public Vector3 spin;

	void Update () {
				transform.Rotate(spin * Time.deltaTime);
	}
}

// PLEASE NOTE! THIS SCRIPT IS FOR DEMO PURPOSES ONLY :) //