using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    public Text wintext; 
    
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
            other.GetComponent<Player>().coins++;
            if (other.GetComponent<Player>().coins >= 3)
            {
                wintext.text = "You Win !";
                Invoke("Close", 3);
            }

            Destroy(gameObject);
        }
    }
    void Close()
    {
        Application.Quit();
    }
}
