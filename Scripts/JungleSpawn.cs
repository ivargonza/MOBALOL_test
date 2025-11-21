using UnityEngine;
using System.Collections;

public class JungleSpawn : MonoBehaviour 
{
	public GameObject jungle;
	public MultiJungleSpawn MJS;
	public void spawn()
	{
		GameObject junglatospawn = (GameObject)Instantiate (jungle, transform.position, transform.rotation);
		junglatospawn.GetComponent<JungleAI> ().RespawnPoint = this.gameObject;
		MJS.Amount += 1;
	}
}
