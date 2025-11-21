using UnityEngine;
using System.Collections;


public class cursor : MonoBehaviour 
{
	public Texture2D CursorNormal;
	public Texture2D CursorOverEnemy;
	public Texture2D CursorOverAlly;
	public Texture2D CursorAbinval;
	public Texture2D CursorAbAlly;
	public Texture2D CursorAbEnm;
	public Texture2D CursorShop;

	public CursorMode cursorMode = CursorMode.Auto;
	public bool spellactive;
	// Use this for initialization
	void Start () 
	{
        
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Cursor.lockState = CursorLockMode.Confined;
        if (Physics.Raycast (ray, out hit, 1000)) 
		{
			if (gameObject.tag == "TeamRed") 
			{
				if(hit.collider.tag == "BuildingBlue"||hit.transform.tag == "ChampionBlue"||hit.transform.tag == "TeamBlue"||hit.transform.tag == "Jungle") 
				{
					Cursor.SetCursor(CursorOverEnemy, Vector2.zero, cursorMode);
				}
				if(hit.collider.tag == "BuildingRed"||hit.transform.tag == "TeamRed"||hit.transform.tag == "ChampionRed") 
				{
					Cursor.SetCursor(CursorOverAlly, Vector2.zero, cursorMode);
				}
			}
			else if (gameObject.tag == "TeamBlue") 
			{
				if(hit.collider.tag == "BuildingRed"||hit.transform.tag == "TeamRed"||hit.transform.tag == "ChampionRed"||hit.transform.tag == "Jungle") 
				{
					Cursor.SetCursor(CursorOverEnemy, Vector2.zero, cursorMode);
				}
				if(hit.collider.tag == "BuildingBlue"||hit.transform.tag == "TeamBlue"||hit.transform.tag == "ChampionBlue") 
				{
					Cursor.SetCursor(CursorOverAlly, Vector2.zero, cursorMode);
				}
			}
            if (hit.collider.tag == null || hit.collider.tag == "Untagged")
            {
                Cursor.SetCursor(CursorNormal, Vector2.zero, cursorMode);
            }
            if (hit.collider.tag == "Shop")
            {
                Cursor.SetCursor(CursorShop, Vector2.zero, cursorMode);
            }
            if (hit.collider.tag == "Ward")
            {
                Cursor.SetCursor(CursorOverAlly, Vector2.zero, cursorMode);
            }
            if (hit.collider.tag == "CapturePoint")
            {
                Cursor.SetCursor(CursorAbinval, Vector2.zero, cursorMode);
            }
        }
        else
        {
            Cursor.SetCursor(CursorNormal, Vector2.zero, cursorMode);
        }
        

        if (spellactive) 
		{
			Cursor.SetCursor(CursorAbinval, Vector2.zero, cursorMode);
		}
	}
}
