using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class DrawRange : MonoBehaviour 
{
	[Range(0,50)]
	public int segments = 50;
	[Range(0,99)]
	public float xradius = 5;
	[Range(0,99)]
	public float yradius = 5;
	LineRenderer line;
	
	void Start ()
	{
		StartCoroutine (Initialize ());
	}
	
	public void CreatePoints ()
	{
		float x;
		//float y;
		float z;
		
		float angle = 20f;
		
		for (int i = 0; i < (segments + 1); i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * xradius;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * yradius;
			
			line.SetPosition (i,new Vector3(x,0,z) );
			
			angle += (360f / segments);
		}
	}
	IEnumerator Initialize()
	{
		line = gameObject.GetComponent<LineRenderer>();
		
		line.SetVertexCount (segments + 1);
		line.useWorldSpace = false;
		CreatePoints ();
		yield return new WaitForSeconds (0.01f);
		line.enabled = false;
	}
}
