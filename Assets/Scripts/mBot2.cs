using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mBot2 : MonoBehaviour
{
    public int speed;
    public GameObject WheelLeft;
    public GameObject WheelRight;
    //public GameObject WheelMiddle;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Input.GetAxis("Vertical") * speed * Time.deltaTime);
        WheelLeft.transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * speed * Time.deltaTime, Space.World);
        WheelRight.transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * speed * Time.deltaTime, Space.World);
        //WheelMiddle.transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * speed * Time.deltaTime, Space.World);

    }
}
