using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PatchNotesReader : MonoBehaviour 
{

	public TextAsset PatchNotesInfo;
	public Text PatchNotes;
	// Use this for initialization
	void Start () 
	{
		ReadString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ReadString()
	{
		PatchNotesInfo = (TextAsset)Resources.Load("PatchNotes", typeof(TextAsset));
		string content = PatchNotesInfo.text;
		PatchNotes.text = content;


	}
}
