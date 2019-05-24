using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class ContinuousButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
  public bool isPressed = false;
  public void OnPointerUp(PointerEventData eventdata) { isPressed = false; }       
  public void OnPointerDown(PointerEventData eventdata) { isPressed = true; } 
}
