using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class mBot2 : MonoBehaviour
{
    public int speed = 0;
    public float smoothSpeed = 5f;
    public GameObject WheelLeft;
    public GameObject WheelRight;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int wheelSpeed = speed * 20;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime);
            WheelLeft.transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * wheelSpeed * Time.deltaTime);
            WheelRight.transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * wheelSpeed * Time.deltaTime);
        }


        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.Lerp(transform.eulerAngles, Vector3.down, smoothSpeed), Space.World);
            }
            else
            {
                transform.Rotate(Vector3.Lerp(transform.eulerAngles, Vector3.up, smoothSpeed), Space.World);
            }

            WheelLeft.transform.Rotate(Vector3.left * Input.GetAxis("Horizontal") * wheelSpeed * Time.deltaTime);
            WheelRight.transform.Rotate(Vector3.right * Input.GetAxis("Horizontal") * wheelSpeed * Time.deltaTime);
        }



    }
}
