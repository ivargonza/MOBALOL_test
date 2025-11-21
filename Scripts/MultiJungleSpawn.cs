using UnityEngine;
using System.Collections;

public class MultiJungleSpawn : MonoBehaviour 
{
	public JungleSpawn[] JS;
	public float TiempoDeRespawnBase;
	public float TiempoDeRespawn;
	public int CantidadDeMonstruos;
	public int Amount;
	
	void Update () 
	{
		if (TiempoDeRespawn > 0 && Amount ==0) 
		{
			TiempoDeRespawn-= Time.deltaTime;
		}
		if (TiempoDeRespawn < 0 && Amount >0) 
		{
			TiempoDeRespawn = 0;
		}
		if (TiempoDeRespawn ==0 && Amount ==0) 
		{
			TiempoDeRespawn = TiempoDeRespawnBase;
		}
		if (TiempoDeRespawn < 0) 
		{
			if (JS.Length >= 1) 
			{
				
				for (var i = 0; i < JS.Length; i++) 
				{
					JS[i].spawn();

				}
			} 
		}
		
	}

}
