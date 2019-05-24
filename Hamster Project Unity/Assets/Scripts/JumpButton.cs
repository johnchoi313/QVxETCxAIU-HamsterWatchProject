using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class JumpButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

  public HamsterWatch hamsterWatch;

  public void OnPointerDown(PointerEventData data) { hamsterWatch.setJump(true); Debug.Log("Jump Pressed"); }
  public void OnPointerUp(PointerEventData data) { hamsterWatch.setJump(false); Debug.Log("Jump Released"); }

}
