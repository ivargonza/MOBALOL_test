using UnityEngine;
using System.Collections;

public class RandomSkins : MonoBehaviour 
{

	public Material[] ChromasDefault;
    public Material[] AltSkins;
    public SkinnedMeshRenderer ChampionMesh;
	
    //alternate skins meshes
    public Mesh[] skins;

    public int SkinsAmmount;

    void OnEnable ()
	{
        SkinsAmmount = Random.Range(0, skins.Length);
        if(SkinsAmmount == 0)//default and chromas
        {
            if (ChampionMesh != null)
            {
                ChampionMesh.material = ChromasDefault[Random.Range(0, ChromasDefault.Length)];
                ChampionMesh.sharedMesh = skins[0];


            }
        }
        else
        {
            if (ChampionMesh != null)
            {
                ChampionMesh.material = AltSkins[SkinsAmmount];
                ChampionMesh.sharedMesh = skins[SkinsAmmount];

            }
        }

        
	}

    public void resetskin()
    {
        ChampionMesh.material = ChromasDefault[0];
        ChampionMesh.sharedMesh = skins[0];
    }
}
