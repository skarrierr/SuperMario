using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody rb;

    public float MoveSpeed;
    public float jumpForce;
    private bool jumpBool = false;
    bool space = false;
    private bool Grounded = true;
    private Animator anim;

    private bool Crouching = false;

    Vector3 rotation;

    private bool WallJumpBool = false;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        anim = transform.GetChild(0).GetComponent<Animator>();

    }


    void FixedUpdate()
    {
        move();
        jump();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBool = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            space = true;
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