using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour 
{
	public Camera m_Camera;
	public bool amActive =false;
	public bool autoInit =false;
    public bool autoInitminimap = false;
    GameObject myContainer;	
	
	void Awake()
	{
		if (autoInit == true)
		{
			m_Camera = Camera.main;
			amActive = true;
		}
        if (autoInitminimap == true)
        {
            m_Camera = GameObject.Find("Camera_Minimap").GetComponent<Camera>();
            amActive = true;
        }
    }
	
	
	void Update()
	{
		if(amActive==true)
		{
			transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back, m_Camera.transform.rotation * Vector3.up);
		}
	}

}
