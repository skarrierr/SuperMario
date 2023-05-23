using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Transform cameraFollow;
    private Rigidbody rb;
    public float MoveSpeed;
    public float jumpForce;
    private bool jumpBool = false;
    bool space = false;
    private bool Grounded = true;
    private Animator anim;
    public int coins;
    private bool Crouching = false;

    Vector3 rotation;


    public Transform WallRayDetector;
    private bool WallJumpBool = false;
    public GameObject pillow;
    public GameObject pillowspawn;

    void Start()
    {
        cameraFollow = GameObject.FindGameObjectWithTag("Camera").transform;
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        move();
        jump();
        WallJump();
    }

    private void Update()
    {
        Cappy();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            space = true;
            jumpBool = true;

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            space = false;
        }

    }

    private void move() {

     Vector3 realVelocity = rb.velocity;

     Vector3 inputVector;

     float x = Input.GetAxis("Horizontal");
     float y = Input.GetAxis("Vertical");

        Vector3 XMOVE = Camera.main.transform.right * x;
        Vector3 YMOVE = Camera.main.transform.forward * y;
        inputVector = XMOVE + YMOVE;      
        inputVector *= MoveSpeed;

        inputVector.y = rb.velocity.y; 

        if (!WallJumpBool)
            rb.velocity = inputVector;





        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !WallJumpBool)
        {
            rotation = new Vector3(inputVector.x, 0, inputVector.z);
            transform.rotation = Quaternion.LookRotation(rotation);
        }

        if (x != 0 || y != 0)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }

        if (Input.GetKey(KeyCode.V) && Grounded && !WallJumpBool)
        {
            Crouching = true;
            rb.velocity = inputVector / 3;    //This overrides the previous velocity assignment
            anim.SetBool("Crouch", true);
           // sphCol.enabled = true;
           // capCol.enabled = false;
            rb.AddForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            Crouching = false;
            anim.SetBool("Crouch", false);
           // sphCol.enabled = false;
           // capCol.enabled = true;
        }

    }


    private void jump()
    {
        if (jumpBool && Grounded)
        {
            rb.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
            jumpBool = false;
            Grounded = false;
            anim.SetBool("Jump", true);
        }
        else
        {
            jumpBool = false;
        }
    }

    void Cappy()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Instantiate(pillow, pillowspawn.transform.position, new Quaternion(0,0,0,0));
        }
    }

    void WallJump()
    {

        //used to detect if player facing wall
        Ray wall = new Ray(WallRayDetector.transform.position, WallRayDetector.transform.forward); //raycasr
        RaycastHit hit;

        //used to see if player is off the ground, before walljumping
        RaycastHit hitdown; //declare a raycast hit detector
        Ray downRay = new Ray(transform.position, -Vector3.up); //shoot a raycast downward
        Physics.Raycast(downRay, out hitdown); //tells unity if the downray hit something, and transfers result into hitdown.

        bool offground = false;
        offground = hitdown.distance > 0.2f;

        if (Physics.Raycast(wall, out hit, 0.7f) && hit.normal.y < 0.05 && offground && rb.velocity.y <= 0) //use a layer mask if you have triggers around the course
        {
            rb.drag = 5;
            WallJumpBool = true;
            anim.SetBool("WallJump", true);


            anim.SetBool("Jump", false);



            if (!Grounded && Physics.Raycast(wall, out hit, 0.7f) && hit.normal.y < 0.2 && space)
            {
                space = false;
                WallJumpBool = true;
                transform.eulerAngles += new Vector3(0, 180, 0);//face the opposite direction of the wall
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);//set to 0 and then change on next line, so we always stabilize the vlelocity increase
                rb.velocity = new Vector3(hit.normal.x * 8, rb.velocity.y + 20, hit.normal.z * 7); //bounce off to direction of normal


                anim.SetBool("WallJump", false);
                anim.SetBool("Jump", true);

            }
        }
        else
        {
            anim.SetBool("WallJump", false);
            rb.drag = 0;
        }




    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.5 && !Input.GetKey(KeyCode.Space))
        {
            Grounded = true;
            WallJumpBool = false;
            anim.SetBool("Jump", false);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (!Crouching)
        {
            Grounded = false;
        }
    }


}