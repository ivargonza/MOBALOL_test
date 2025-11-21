using UnityEngine;
using System.Collections;
using UnityEngine.Scripting;

public class GCML : MonoBehaviour 
{

	public float liberador_contador;
	// Use this for initialization
	void Start () 
	{
        //InvokeRepeating("unload", 1, 5);
        liberador_contador = 60f;
    }
	
	// Update is called once per frame
	void Update () 
	{
        if(liberador_contador > 0)
        {
        	liberador_contador -=Time.deltaTime;
        }
        if(liberador_contador<0f)
        {
            liberador_contador = 60f;
            unload();
        }

    }

    void unload()
    {
        Resources.UnloadUnusedAssets();
        
    }
}
