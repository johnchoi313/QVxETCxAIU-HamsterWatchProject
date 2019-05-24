using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ActionButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

  public HamsterWatch hamsterWatch;

  public void OnPointerDown(PointerEventData data) { hamsterWatch.setAction(true); Debug.Log("Action Pressed"); }
  public void OnPointerUp(PointerEventData data) { hamsterWatch.setAction(false); Debug.Log("Action Released"); }

}
