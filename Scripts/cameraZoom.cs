using UnityEngine;
using System.Collections;

public class cameraZoom : MonoBehaviour
{

    public float Y = 0.2f; // set this to 0.2
    public float B = 0.2f; // set this to -0.2

    public Camera cam;

    // Use this for initialization
    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(cam.fieldOfView > 50)
        {
            cam.fieldOfView = 50;
        }
        if (cam.fieldOfView < 30)
        {
            cam.fieldOfView =30;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(cam.fieldOfView <= 50 && cam.fieldOfView >= 30)
            {
                cam.fieldOfView -= Y;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cam.fieldOfView <= 50 && cam.fieldOfView >= 30)
            {
                cam.fieldOfView += B;
            }
        }


    }
}
