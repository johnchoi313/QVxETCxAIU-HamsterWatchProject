using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour {

  public Borders borders;
  public GameObject fruitCrate;
  public GameObject itemCrate;

  [Header("Airplane Spawning")]
  public float airplaneTimeMin = 10;
  public float airplaneTimeMax = 30;
  public float airplaneTimer = 0;
  
  [Header("Plane Moving/Tilting")]
  public float moveSpeed = 1;
  public float swivelSpeed = 0;
  public float swivelAmount = 0;
  private float swivelAngle = 0;
  private float initialPosY;

  [Header("Crate Spawning")]
  public float spawnDelayMin = 1;
  public float spawnDelayMax = 3;
  public float spawnTime = 0.8f;
  public float spawnTimer = 0;
  
  public float spawnNumber = 10;
  private float spawnCount = 10;
  public int fruitChance = 5;

  private bool inBounds = false;
  private bool wasInBounds = true;
  private bool airplaneReorient = false;

  void Start () {
    initialPosY = transform.position.y;
    airplaneTimer = Random.Range(airplaneTimeMin, airplaneTimeMax); 
  }

	void Update () {
    if(airplaneTimer > 0) { airplaneTimer -= Time.deltaTime; }
    else if(airplaneReorient) {
      airplaneReorient = false;
      gameObject.GetComponent<AudioSource>().Play();
      airplaneTimer = Random.Range(airplaneTimeMin, airplaneTimeMax); 
      transform.position = new Vector3(Random.Range(borders.getMinX(),borders.getMaxX()) * 0.3f,initialPosY,Random.Range(borders.getMinZ(),borders.getMaxZ()) * 0.3f);
      transform.Rotate(0, Random.Range(0,360), 0);
      transform.Translate(0,0,-10f);
      spawnCount = spawnNumber;

    }

    //Check whether plane is in bounds 
    Debug.Log("MinX: " + borders.getMinX());
    Debug.Log("MaxX: " + borders.getMaxX());
    Debug.Log("MinZ: " + borders.getMinZ());
    Debug.Log("MaxZ: " + borders.getMaxZ());
    inBounds = (transform.position.x > borders.getMinX()*1.1f && transform.position.x < borders.getMaxX()*1.1f &&
                transform.position.z > borders.getMinZ()*1.1f && transform.position.z < borders.getMaxZ()*1.1f);
    if(wasInBounds != inBounds) { 
      //If going from outside of bounds to inside of bounds, start spawn delay
      if(inBounds) { spawnTimer = Random.Range(spawnDelayMin,spawnDelayMax); } 
      //If otherwise, "respawn" and reorient plane;
      else { airplaneReorient = true; }
    }
    wasInBounds = inBounds;
    //Instantiate Crates
    if(spawnTimer > 0) { spawnTimer -= Time.deltaTime; }
    else if(spawnCount > 0 && inBounds) {
      if(Random.Range(0, fruitChance) == 0) { Instantiate(itemCrate, transform.position-new Vector3(0,0.3f,0), Random.rotation); } 
      else { Instantiate(fruitCrate, transform.position-new Vector3(0,0.3f,0), Random.rotation); }
      Debug.Log("Airdropped Crate!");
      spawnTimer = spawnTime;      
      spawnCount--;
    }
    //Rock the aircraft
    swivelAngle = Mathf.Sin(Time.time * swivelSpeed) * swivelAmount;
    transform.localEulerAngles = new Vector3(0,transform.localEulerAngles.y,swivelAngle);
    //Move the aircraft
    transform.Translate(0,0,moveSpeed * Time.deltaTime);
    transform.position = new Vector3(transform.position.x,initialPosY,transform.position.z);
  }
}