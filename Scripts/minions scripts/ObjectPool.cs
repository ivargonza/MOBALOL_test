using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
	[System.Serializable]
	public class Pool
	{
		public string tag;
		public GameObject prefab;
		public int size;
	}
	
	public static ObjectPool Instance { get; private set; }
	
	[SerializeField] private List<Pool> pools;
	private Dictionary<string, Queue<GameObject>> poolDictionary;
	private Dictionary<string, GameObject> prefabDictionary;
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		
		InitializePools();
	}
	
	private void InitializePools()
	{
		poolDictionary = new Dictionary<string, Queue<GameObject>>();
		prefabDictionary = new Dictionary<string, GameObject>();
		
		foreach (Pool pool in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();
			
			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = Instantiate(pool.prefab);
				obj.name = pool.prefab.name + "_" + i;
				obj.SetActive(false);
				obj.transform.SetParent(transform);
				objectPool.Enqueue(obj);
			}
			
			poolDictionary.Add(pool.tag, objectPool);
			prefabDictionary.Add(pool.tag, pool.prefab);
		}
	}
	
	public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
	{
		if (!poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag {tag} doesn't exist.");
			return null;
		}
		
		// Buscar un objeto inactivo en el pool
		Queue<GameObject> pool = poolDictionary[tag];
		GameObject objectToSpawn = null;
		int searchCount = 0;
		int poolSize = pool.Count;
		
		// Buscar en el pool hasta encontrar uno inactivo
		while (searchCount < poolSize)
		{
			GameObject obj = pool.Dequeue();
			pool.Enqueue(obj);
			
			if (!obj.activeInHierarchy)
			{
				objectToSpawn = obj;
				break;
			}
			
			searchCount++;
		}
		
		// Si no hay objetos disponibles, crear uno nuevo
		if (objectToSpawn == null)
		{
			Debug.LogWarning("Pool {tag} is full. Creating new instance.");
			objectToSpawn = Instantiate(prefabDictionary[tag]);
			objectToSpawn.name = prefabDictionary[tag].name + "_Extra";
			objectToSpawn.transform.SetParent(transform);
			pool.Enqueue(objectToSpawn);
		}
		
		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);
		
		IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
		if (pooledObj != null)
		{
			pooledObj.OnObjectSpawn();
		}
		
		return objectToSpawn;
	}
	
	public void ReturnToPool(GameObject obj)
	{
		if (obj != null)
		{
			obj.SetActive(false);
			obj.transform.SetParent(transform);
		}
	}
}

public interface IPooledObject
{
	void OnObjectSpawn();
}