using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borders : MonoBehaviour {

  public Transform left, right, front, back;
  private RaycastHit leftEdge, rightEdge, frontEdge, backEdge;

	// Use this for initialization
	void Update () {

    int w = Screen.width;
    int h = Screen.height;
    
    if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(0,h/2)), out leftEdge)) {
      if(leftEdge.collider.gameObject.name == "Floor") { left.position = leftEdge.point; }
    }
    if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(w,h/2)), out rightEdge)) {
      if(rightEdge.collider.gameObject.name == "Floor") { right.position = rightEdge.point; }
    }
  
    if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(w/2,1)), out frontEdge)) {
      if(frontEdge.collider.gameObject.name == "Floor") { front.position = frontEdge.point; }
    }
    if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(w/2,h-1)), out backEdge)) {
      if(backEdge.collider.gameObject.name == "Floor") { back.position = backEdge.point; }
    }
    
  }

  public float getMinX() { return right.position.x; }
  public float getMinZ() { return back.position.z; }
  public float getMaxX() { return left.position.x; }
  public float getMaxZ() { return front.position.z; }
}