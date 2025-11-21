using UnityEngine;
using System.Collections;

public class ManualOcclusion : MonoBehaviour 
{
	public MeshRenderer m_Renderer;
	// Use this for initialization
	void Start()
	{
		m_Renderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (m_Renderer.isVisible)
		{
			m_Renderer.enabled = true;
			Debug.Log("Object is visible");
		}
		else 
		{
			m_Renderer.enabled = false;
			Debug.Log("Object is no longer visible");
		}
	}
}
