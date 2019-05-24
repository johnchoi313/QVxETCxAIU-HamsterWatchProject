using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemSpawn : MonoBehaviour {

  public Borders borders;

  public GameObject[] fruitList;
  public float minFruitTime;
  public float maxFruitTime;
  private float fruitTimer;
  
  public GameObject[] itemList;
  public float minItemTime;
  public float maxItemTime;
  private float itemTimer;
  
	// Update is called once per frame
	void Update () {
	  //Instantiate Items
    if(itemTimer > 0) { itemTimer -= Time.deltaTime; }
    else {
      itemTimer = Random.Range(minItemTime, maxItemTime);
      Instantiate(itemList[Random.Range(0,itemList.Length)], new Vector3(Random.Range(borders.getMinX(),borders.getMaxX()),0.3f,Random.Range(borders.getMinZ(),borders.getMaxZ())),Quaternion.identity);

    }
    //Instantiate Fruits
    if(fruitTimer > 0) { fruitTimer -= Time.deltaTime; }
    else {
      fruitTimer = Random.Range(minFruitTime, maxFruitTime);
      Instantiate(fruitList[Random.Range(0,fruitList.Length)], new Vector3(Random.Range(borders.getMinX(),borders.getMaxX()),0.3f,Random.Range(borders.getMinZ(),borders.getMaxZ())),Quaternion.identity);
    }
	}
}
