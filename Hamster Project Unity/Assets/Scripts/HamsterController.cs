using UnityEngine;
using UnityEngine.UI;
using EasyWiFi.Core;
using System.Collections;
using System.Collections.Generic;

// The GameObject is made to bounce using the space key.
// Also the GameOject can be moved forward/backward and left/right.
// Add a Quad to the scene so this GameObject can collider with a floor.
public class HamsterController : MonoBehaviour {

  public bool available = true;

  public float speed = 6.0f;
  public float jumpSpeed = 8.0f;
  public float gravity = 20.0f;
  public float pushPower = 2.0f;
  public float bounce = 20.0f;

  public float lookAngle = 60f;
  public float lookSpeed = 5f;

  public ParticleSystem plusOne;
  public int score = 0;

  public AudioSource jumpSound;
  public AudioSource itemSound;
  public AudioSource eatSound;
  
  public TextMesh nameTag;
  public Score gameScore;

  private Vector3 moveDirection = Vector3.zero;
  private CharacterController controller;
  private Animator animator;

  public Transform arm;
  public Transform back;
  public Transform root;
  private GameObject item;

  public UDPReceiver udpReceiver;
  
  public enum ControlMode { UDP, WASD, Arrows };
  public ControlMode controlMode;

  private bool previousJump = false;
  private bool previousAction = false;

  private HamsterData hamsterData;
  
  void Start() {
    controller = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();
    hamsterData = new HamsterData();
    hamsterData.n = nameTag.text;
  }

  public void setAvailable(bool isAvailable) {
    available = isAvailable;
    gameObject.SetActive(!isAvailable);
  }

  public void setHamsterData(string message) { 
    try {
      setHamsterData(JsonUtility.FromJson<HamsterData>(message)); 
    } catch {
      Debug.LogWarning("Failed to parse JSON hamster message!");
    }
  }
  public void setHamsterData(HamsterData data) { 
    hamsterData = data; 
    setNameTag(hamsterData.n);
  }
  void Update() {
    //Get Controls from UDP Sender
    //string message = udpReceiver.getMessage();
    //if(message != null) { hamsterData = JsonUtility.FromJson<HamsterData>(message); }

    //Jetpack
    if (getJumpButton() && item != null) { 
      if(item.GetComponent<Item>().pickupType == Item.PickupType.JETPACK) { item.GetComponent<Item>().activate(transform); }
    }
    //Jump
    if (getJumpButtonDown()) { jump(); }
    //Move
    move(getMoveButton());
    //All item powerups
    if (item != null) { 
      switch(item.GetComponent<Item>().pickupType) {
        case(Item.PickupType.FLASHLIGHT):
          if (getActionButtonDown()) { item.GetComponent<Item>().activate(transform); }
        break;
        
        case(Item.PickupType.ROCKET):
          if (getActionButtonDown()) { item.GetComponent<Item>().activate(transform); }
        break;
        
        case(Item.PickupType.SPRINKLER):
          if (getActionButtonDown()) { item.GetComponent<Item>().activate(transform); }
        break;
        
        case(Item.PickupType.BUBBLER):
          if (getActionButton()) { item.GetComponent<Item>().activate(transform); }
          else { item.GetComponent<Item>().deactivate(); }
        break;

        case(Item.PickupType.JETPACK):
          if (getJumpButton()) {
            item.GetComponent<Item>().activate(transform);
            moveDirection.y += gravity*0.8f * Time.deltaTime;
          } 
          else { item.GetComponent<Item>().deactivate(); }
        break;
      }
    }
    //Update button actions
    previousAction = hamsterData.a==1?true:false;
    previousJump = hamsterData.j==1?true:false;
    //Update the hamster
    updateHamster();
  }
  
  //Button Mappings
  public Vector2 getMoveButton() {
    return new Vector2((controlMode != ControlMode.UDP) ? ((controlMode == ControlMode.WASD)?Input.GetAxis("Horizontal1"):Input.GetAxis("Horizontal2")) : hamsterData.x * 0.001f, 
                       (controlMode != ControlMode.UDP) ? ((controlMode == ControlMode.WASD)?Input.GetAxis("Vertical1"):Input.GetAxis("Vertical2")) : hamsterData.y * 0.001f);
  }
  public bool getJumpButton() {
    return((controlMode != ControlMode.UDP) ? ((controlMode == ControlMode.WASD)?Input.GetButton("Jump1"):Input.GetButton("Jump2")) : hamsterData.j==1?true:false);
  }
  public bool getJumpButtonDown() {
    return((controlMode != ControlMode.UDP) ? ((controlMode == ControlMode.WASD)?Input.GetButtonDown("Jump1"):Input.GetButtonDown("Jump2")) : (hamsterData.j==1?true:false && previousJump == false));
  }
  public bool getActionButton() {
    return((controlMode != ControlMode.UDP) ? ((controlMode == ControlMode.WASD)?Input.GetButton("Fire1"):Input.GetButton("Fire2")) : hamsterData.a==1?true:false);
  }
  public bool getActionButtonDown() {
    return((controlMode != ControlMode.UDP) ? ((controlMode == ControlMode.WASD)?Input.GetButtonDown("Fire1"):Input.GetButtonDown("Fire2")) : (hamsterData.a==1?true:false && previousAction == false));
  }
  
  public void updateHamster() {
    // Apply gravity
    if(!controller.isGrounded) { moveDirection.y = moveDirection.y - (gravity * Time.deltaTime); }
    // Move the controller
    controller.Move(moveDirection * Time.deltaTime);
    //Point name tag at camera
    nameTag.gameObject.transform.eulerAngles = new Vector3(45,180,0);
  }
  public void move(Vector2 direction) { move(direction.x, direction.y); }
  public void move(float leftRight, float frontBack = 0) {
    animator.SetBool("walking",Mathf.Abs(leftRight) + Mathf.Abs(frontBack) > 0.15);
    float spd = speed * ((item != null && item.GetComponent<Item>().pickupType == Item.PickupType.CAR) ? 1.5f : 1); 
    if (controller.isGrounded || (item != null && item.GetComponent<Item>().pickupType == Item.PickupType.JETPACK)) {
      moveDirection = new Vector3(-leftRight * spd, moveDirection.y, -frontBack * spd);
    }
    //Look Left or right based on button
    transform.LookAt( transform.position + new Vector3(-leftRight, 0, -frontBack) );
  }
  public void jump() {
    if (controller.isGrounded) {
      moveDirection.y = jumpSpeed; 
      animator.SetTrigger("jump");  
      jumpSound.Play();
    }
  }
  public void setNameTag(string name) { setNameTag(name, score); }
  public void setNameTag(int score)  { setNameTag(hamsterData.n, score); }
  public void setNameTag(string name, int score) { nameTag.text = name + " (" + score + ")"; }

  // this script pushes all rigidbodies that the character touches
  void OnControllerColliderHit(ControllerColliderHit hit) {
    //Hit a mushroom
    if(hit.gameObject.tag == "Bounce") {

      if(Mathf.Abs(hit.transform.position.x - transform.position.x) < 0.45f && Mathf.Abs(hit.transform.position.z - transform.position.z) < 0.45f) {
        hit.gameObject.GetComponent<Animator>().SetTrigger("Bounce");
        moveDirection.y = bounce; 
        animator.SetTrigger("jump");  
        jumpSound.Play();
      }
      return;
    }
    //Hit other rigidbody
    Rigidbody body = hit.collider.attachedRigidbody;
    // no rigidbody
    if (body == null || body.isKinematic) { return; }
    // We dont want to push objects below us
    if (hit.moveDirection.y < -0.3) { return; }
    // Calculate push direction from move direction,
    // we only push objects to the sides never up and down
    Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
    // If you know how fast your character is trying to move,
    // then you can also multiply the push velocity by that.
    // Apply the push
    body.velocity = pushDir * pushPower;
  }
 
  void OnTriggerEnter(Collider hit) {
    if (hit.tag == "Fruit") {
      //collision.collider.gameObject.layer == LayerMask.NameToLayer("LAYER_NAME")  
      Destroy(hit.GetComponent<Collider>().gameObject);
      gameScore.incrementScore();
      score++; setNameTag(score);
      eatSound.Play();
      plusOne.Play();
    }
    if (hit.tag == "Item") {
      if(item != null) { Destroy(item); }
      //collision.collider.gameObject.layer == LayerMask.NameToLayer("LAYER_NAME")  
      hit.GetComponent<Collider>().enabled = false;
      item = hit.GetComponent<Collider>().gameObject;
      transform.localScale = new Vector3(1,1,1);
      switch(item.GetComponent<Item>().pickupType) {
        case(Item.PickupType.ROCKET): item.GetComponent<Item>().attach(arm); break;
        case(Item.PickupType.FLASHLIGHT): item.GetComponent<Item>().attach(arm); break;
        case(Item.PickupType.JETPACK): item.GetComponent<Item>().attach(back); break;
        case(Item.PickupType.CAR): item.GetComponent<Item>().attach(root); break;
        case(Item.PickupType.COLA): transform.localScale = new Vector3(3,3,3); item.GetComponent<Item>().attach(arm); break;
        case(Item.PickupType.SPRINKLER): item.GetComponent<Item>().attach(arm); break;
        case(Item.PickupType.BUBBLER): item.GetComponent<Item>().attach(arm); break;
      }
      itemSound.Play();  
    }
  }
}

[System.Serializable]
public class HamsterData {
    public string IP = ""; //IP
    public int k;          //key
    public string n = "";  //name
    public int f = 0;      //face
    public int b = 0;      //body
    public int s = 0;      //score
    
    public int x = 0;  //xMotion
    public int y = 0;  //yMotion

    public int a = 0; //action
    public int j = 0; //jump

    public List<BreakData> bd = null;
}