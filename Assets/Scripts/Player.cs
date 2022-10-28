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
    private bool groundpound;

    Vector3 rotation;

    private Animator anim;
    private Animator camAnim;

    //crouch
    private CapsuleCollider capCol;
    private SphereCollider sphCol;
    private bool Crouching = false;

    public Transform WallRayDetector;
    private bool WallJumpBool = false;

    public GameObject fireball;
    public Transform fireball_spawn_loc;
    public Vector3 fireballVel;


    // Start is called before the first frame update
    void Start()
    {
        cameraFollow = GameObject.FindGameObjectWithTag("Camera").transform;
        camAnim = GameObject.FindGameObjectWithTag("Camera").transform.GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        anim = transform.GetChild(0).GetComponent<Animator>();

        capCol = GetComponent<CapsuleCollider>();
        sphCol = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        jump();
        move();
        WallJump();
    }
    private void Update()
    {
       

        //jump - I have found that the getKeyDown works better in update, however, the physics obviously works better in fixedupdate, so I combined the detection in update, and the movement in fixedupdate
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBool = true;
        }

        //space key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            space = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            space = false;
        }

        if (!Grounded && !groundpound && Input.GetKeyDown(KeyCode.V))
        {
            StartCoroutine(GroundPound());
        }
    }

    private void move()
    {
        Vector3 realVelocity = rb.velocity;

        Vector3 inputVector;

        //inputs
        float x = Input.GetAxis("Horizontal");  // A and D or Left and Right arrow
        float y = Input.GetAxis("Vertical");

        //camera relative directions based on horizontal and vertical
        Vector3 XMOVE = Camera.main.transform.right * x;
        Vector3 YMOVE = Camera.main.transform.forward * y;

        inputVector = XMOVE + YMOVE;         //create a single movement vector from the inputs
        inputVector *= MoveSpeed;            //multiply it by speed

        inputVector.y = rb.velocity.y; //make sure you do not affect the y velocity (gravity)
        if (!groundpound && !WallJumpBool)
            rb.velocity = inputVector;    //assign

        //rotation
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && !WallJumpBool) // this makes sure the rotation does not reset to 0 if the player is not pressing any key
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

        if (Input.GetKey(KeyCode.V) && Grounded && !groundpound && !WallJumpBool)
        {
            Crouching = true;
            rb.velocity = inputVector / 3;    //This overrides the previous velocity assignment
            anim.SetBool("Crouch", true);
            sphCol.enabled = true;
            capCol.enabled = false;
            rb.AddForce(Vector3.down * 15000 * Time.deltaTime, ForceMode.Acceleration);
        }
        else
        {
            Crouching = false;
            anim.SetBool("Crouch", false);
            sphCol.enabled = false;
            capCol.enabled = true;
        }


    }
    private void jump()
    {
        if (jumpBool && Grounded && !groundpound && !Crouching)
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
    IEnumerator GroundPound()
    {
        RaycastHit hit; //declare a raycast hit detector
        Ray downRay = new Ray(transform.position, -Vector3.up); //shoot a raycast downward

        Physics.Raycast(downRay, out hit); //tells unity if the downray hit something, and transfers result into hit.
        {
            Grounded = false;
            groundpound = true;
            rb.drag = 0;

            anim.SetBool("GroundPound", true);
            anim.SetBool("Jump", false);

            rb.velocity = new Vector3(0, 0, 0); //freeze
            rb.useGravity = false; //no external forces
            yield return new WaitForSeconds(0.5f);
            rb.velocity = new Vector3(0, -30, 0);
            rb.mass = 100; //so mario doesnt move randomly when groundpounding
            while (!Grounded)
            {
                yield return new WaitForSeconds(0.001f); //a way to pause the function
            }


            camAnim.SetTrigger("Shake"); //cam shake animation
            yield return new WaitForSeconds(0.1f);

            rb.useGravity = true;
            rb.isKinematic = true;
            yield return new WaitForSeconds(0.4f);
            groundpound = false; //reset all modifications
            anim.SetBool("GroundPound", false);
            rb.isKinematic = false;
            rb.mass = 5;
        }
        //right,forward negative contact point normal
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (rb.velocity.y > -1)
            {
                Vector3 direction = (transform.position - collision.transform.position).normalized;
                rb.AddForce(direction * 15000 * Time.deltaTime, ForceMode.Impulse);

            }
        }
    }

  

   




}
