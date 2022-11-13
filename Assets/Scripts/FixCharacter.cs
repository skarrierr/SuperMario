using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixCharacter : MonoBehaviour
{
    public GameObject klk;
    public float offset;
    public Quaternion rotacion;

    // Start is called before the first frame update
    void Start()
    {
        rotacion = klk.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        rotacion = klk.transform.rotation;
        transform.position = new Vector3(klk.transform.position.x, klk.transform.position.y - offset, klk.transform.position.z);
        this.transform.rotation = new Quaternion(transform.rotation.x , rotacion.y ,transform.rotation.z *1, transform.rotation.w);
        
    }
}
