using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController       : MonoBehaviour
{
    public Vector3 offset;
    private Transform player;

    public float smoothTime = 7;
    public float rotateSpeed;

    private Vector3 none = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, player.position + offset, ref none, smoothTime * Time.deltaTime);
        rotate();

    }

    void rotate()
    {
        float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
        transform.Rotate(0, horizontal * Time.deltaTime, 0);  
    }
}
