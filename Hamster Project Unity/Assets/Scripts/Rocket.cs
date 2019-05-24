using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	public float rocketSpeed = 5;
  public float rocketLife = 5;
  
  public float radius = 1.0F;
  public float power = 10.0F;

  public GameObject explosion;
  private bool exploded = false;

	// Update is called once per frame
	void Update () {
	  gameObject.GetComponent<Rigidbody>().AddRelativeForce(0,0,rocketSpeed);

    if(rocketLife > 0) { rocketLife -= Time.deltaTime; } else { Explode(); }

	}

  public void Explode() {
    if(!exploded) {
      
      Vector3 explosionPos = transform.position;

      Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
      foreach (Collider hit in colliders) {
          Rigidbody rb = hit.GetComponent<Rigidbody>();
          if (rb != null) { rb.AddExplosionForce(power, explosionPos, radius, 3.0F); }
      }

      Destroy(Instantiate(explosion,explosionPos,Quaternion.identity),5);
      Destroy(gameObject);
      //Destroy(gameObject, 0.5f);
      exploded = true;
    }
  }

  public void OnCollisionEnter(Collision hit) {
    Explode();
  }
  

}
