using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillow : MonoBehaviour
{
   
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, 50, 0), ForceMode.Impulse);

            Destroy(this.gameObject);
        }
    }
}
