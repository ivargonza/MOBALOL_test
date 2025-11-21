using UnityEngine;
using UnityEngine.UI;

public class AspectUtility : MonoBehaviour 
{
	public CanvasScaler scaller;
	public float _wantedAspectRatio = 1.777778f;
	static Camera cam;
	public Camera MinimapCamera;

	void Start ()
	{
		cam = GetComponent<Camera>();
	}

	void Update () 
	{

		SetCamera();
	}

	public void SetCamera () 
	{
		float currentAspectRatio = (float)Screen.width / Screen.height;
		if ((int)(currentAspectRatio * 100) / 100.0f == (int)(_wantedAspectRatio * 100) / 100.0f) 
		{
			if(scaller != null)
			{
				scaller.matchWidthOrHeight = 0.5f;
			}
			if(MinimapCamera != null)
			{
				MinimapCamera.rect = new Rect(0.82f, 0.001f, 1.0f, 0.29f);
			}
		}
		else
		{
			if(scaller != null)
			{
				scaller.matchWidthOrHeight = 0.8f;
			}
			if(MinimapCamera != null)
			{
				MinimapCamera.rect = new Rect(0.82f, 0.0f, 1.0f, 0.26f);
			}
		}
	}

	public static int screenHeight 
	{
		get {
			return (int)(Screen.height * cam.rect.height);
		}
	}
	
	public static int screenWidth {
		get {
			return (int)(Screen.width * cam.rect.width);
		}
	}

}