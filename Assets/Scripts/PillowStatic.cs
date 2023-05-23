using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillowStatic : MonoBehaviour
{
   
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.GetComponent<Rigidbody>().AddForce(new Vector3(100, 50, 0), ForceMode.Impulse);

            //Destroy(this.gameObject);
        }
    }
}
