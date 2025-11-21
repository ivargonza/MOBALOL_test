using UnityEngine;

public class PooledMinion : MonoBehaviour, IPooledObject
{
	private Character character;
	private MinionsAI ai;
	private bool isDead;
	
	private void Awake()
	{
		character = GetComponent<Character>();
		ai = GetComponent<MinionsAI>();
	}
	
	public void OnObjectSpawn()
	{
		isDead = false;
		
		// Resetear valores cuando el minion se reactive
		if (character != null)
		{
			character.Vida = character.VidaMax;
			character.IsAlive = true;
		}
		
		if (ai != null)
		{
			ai.enabled = true;
		}
	}
	
	private void Update()
	{
		// Verificar si el minion murió
		//if (!isDead && character != null && character.Vida <= 0f)
		//{
		//	Die();
		//}
	}
	
	private void OnDisable()
	{
		// Limpiar cuando se desactive
		if (ai != null)
		{
			ai.enabled = false;
		}
	}
	
	public void Die()
	{
		if (isDead) return;
		
		isDead = true;
		
		// Llamar esto cuando el minion muera
		if (ObjectPool.Instance != null)
		{
			ObjectPool.Instance.ReturnToPool(gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
}