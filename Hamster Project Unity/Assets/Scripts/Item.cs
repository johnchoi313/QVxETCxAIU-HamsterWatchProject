using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

  public enum PickupType { FRUIT, ROCKET, FLASHLIGHT, JETPACK, CAR, COLA, SPRINKLER, BUBBLER };
  public PickupType pickupType;
  public GameObject itemSparks;

  [Header("Floating Attributes")]
  private Vector3 initialPosition;
  private float bobSeed = 0;
  public float rotateSpeed = 0.5f;
  public float bobAmount = 0.5f;
  public float bobSpeed = 0.5f;

  [Header("Item Attributes")]
  public Vector3 attachedPosition;
  public Vector3 attachedRotation;
  bool isAttached = false;

  private float activateTimer = 0f;
  public float activateTime = 0.5f;
  public GameObject activateObject;
  
	// Use this for initialization
	void Start () {
    if(pickupType != PickupType.FRUIT && itemSparks != null) {
      Destroy(Instantiate(itemSparks, transform.position, transform.rotation), 2);
    }
    bobSeed = Random.Range(0,10);
		initialPosition = transform.position;
	  transform.Rotate(0,Random.Range(0,360),0,Space.World);
  }
	
	// Update is called once per frame
	void Update () {
	  if(!isAttached) {
      transform.Rotate(0,rotateSpeed * Time.deltaTime,0,Space.World);
      float bob = initialPosition.y + Mathf.Sin(bobSeed + Time.time * bobSpeed) *  bobAmount;
      transform.position = new Vector3(initialPosition.x, bob, initialPosition.z);
	  }

    if(activateTimer >= 0) { activateTimer -= Time.deltaTime; } 
  }

  public void attach(Transform hamster) {
    isAttached = true;
    transform.parent = hamster;
    transform.localPosition = attachedPosition;
    transform.localEulerAngles = attachedRotation;
  }

  public void activate(Transform direction) {
    if(isAttached && activateTimer < 0) {
      activateTimer = activateTime;

      switch(pickupType) {
        case(PickupType.ROCKET): 
        Instantiate(activateObject, direction.position, direction.rotation).transform.Translate(0,0.3f,1);
        break;
        
        case(PickupType.SPRINKLER): 
        Instantiate(activateObject, direction.position, direction.rotation).transform.Translate(0,-direction.position.y,1);
        break;

        case(PickupType.JETPACK): 
        activateObject.GetComponent<ParticleSystem>().Play();
        activateObject.GetComponent<AudioSource>().volume = 1;
        activateObject.GetComponent<Light>().enabled = true;
        break;

        case(PickupType.BUBBLER): 
        activateObject.GetComponent<ParticleSystem>().Play();
        activateObject.GetComponent<AudioSource>().volume = 1;
        break;

        case(PickupType.FLASHLIGHT): 
        gameObject.GetComponent<AudioSource>().Play();
        activateObject.SetActive(!activateObject.activeSelf);
        break;
      }
    }
  }

  public void deactivate() {
    switch(pickupType) {
      case(PickupType.JETPACK): 
      activateObject.GetComponent<AudioSource>().volume = 0;
      activateObject.GetComponent<Light>().enabled = false;
      activateObject.GetComponent<ParticleSystem>().Stop();
      break;

      case(PickupType.BUBBLER): 
      activateObject.GetComponent<AudioSource>().volume = 0;
      break;
    }
  }
  
}
