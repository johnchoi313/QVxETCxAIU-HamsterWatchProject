using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {
  public Vector3 gravity = new Vector3(0, -9.81f, 0);
  void Awake () { Physics.gravity = gravity; }
}
