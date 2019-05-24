using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tree : MonoBehaviour {

  public float ForceThreshold;
  
  private float finalSize = 0.25f;
  public float growSpeed = 1f;
  public bool growUp = false;

  private bool fallenOver = false;
  private float fallAngle = 0;
  public float fallRotateSpeed = 1f;
  public float fallSpeed = 1f;

  public ParticleSystem treeDie;
  public ParticleSystem treeSpawn;

	// Use this for initialization
	void Start () {
    finalSize = transform.localScale.y;
    if(growUp) { 
      transform.localScale = new Vector3(0,0,0); 
      treeSpawn.Play();
    }
    transform.Rotate(0,Random.Range(0,360),0,Space.World);
	}
	
	// Update is called once per frame
	void Update () {
    if(growUp) {
      if(transform.localScale.x < finalSize - 0.0001f) {
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(finalSize,finalSize,finalSize), Time.deltaTime * growSpeed);
      }
    }
    if(fallenOver) {
      if(fallAngle < 90) {
        fallAngle += Time.deltaTime * fallRotateSpeed;
        transform.localEulerAngles = new Vector3(fallAngle, transform.localEulerAngles.y, transform.localEulerAngles.z);
      }
      transform.position = transform.position - new Vector3(0,fallSpeed * Time.deltaTime,0);
    }
	}

  public void FallOver(Vector3 direction) { 
    if(!fallenOver) {
      fallenOver = true; 
      transform.LookAt(transform.position + new Vector3(direction.x, 0, direction.z));
      gameObject.GetComponent<AudioSource>().Play();
      Destroy(gameObject, 25);
      treeDie.Play();
    }
  }

  void OnCollisionEnter(Collision col) {
    if (col.contacts.Length > 0) {
      Rigidbody _impactBody = col.rigidbody;
      float _impactMass = (col.rigidbody != null) ? col.rigidbody.mass : 1.0f;
      Vector3 _impactVelocity = col.relativeVelocity;

      Rigidbody rb = GetComponent<Rigidbody>();
      // Always have the impact velocity point in our moving direction
      if (rb != null) { _impactVelocity *= Mathf.Sign(Vector3.Dot(rb.velocity, _impactVelocity)); }

      float mag = _impactVelocity.magnitude;
      Vector3 force = 0.5f * _impactMass * _impactVelocity * mag;

      if ((ForceThreshold * ForceThreshold) <= force.sqrMagnitude) {
        //Fall Over in direction of force
        FallOver(force);
      }
    }
  }


}
