using UnityEngine;
using System.Collections;

public class MinionSpawnerDominion : MonoBehaviour
{
    public GameObject MinionMelee;
    public GameObject MinionRange;
    public GameObject MinionSiege;
    public Transform Spawner;
    public float Timer;

    public float UpgradeTimer;

    public bool Mid;
    public bool Bot;
    public bool Top;

    public bool Siege;

    void Start()
    {
        Timer = 90f;
    }
    void Update()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
        }
        if (Timer < 0)
        {
            Timer = 0;
            StartCoroutine(Spawn());
        }
    }

    void RotateMinion(GameObject MinionToSpawn)
    {
        GameObject MTS = MinionToSpawn;
        if (MTS != null)
        {
            if (Mid)
            {
                MTS.GetComponent<MinionsAI>().Mid = true;
                MTS.GetComponent<MinionsAI>().Bot = false;
                MTS.GetComponent<MinionsAI>().Top = false;
            }
            else if (Bot)
            {
                MTS.GetComponent<MinionsAI>().Mid = false;
                MTS.GetComponent<MinionsAI>().Bot = true;
                MTS.GetComponent<MinionsAI>().Top = false;
            }
            else if (Top)
            {
                MTS.GetComponent<MinionsAI>().Mid = false;
                MTS.GetComponent<MinionsAI>().Bot = false;
                MTS.GetComponent<MinionsAI>().Top = true;
            }
            Instantiate(MTS, Spawner.position, Spawner.rotation);
        }


    }


    IEnumerator Spawn()
    {
        UpgradeTimer += 30f;
        if (UpgradeTimer > 90f)
        {
            MinionMelee.GetComponent<Character>().Vida += 6f;
            MinionRange.GetComponent<Character>().Vida += 4f;
            MinionSiege.GetComponent<Character>().Vida += 5.5f;

            MinionMelee.GetComponent<Character>().Nivel += 1;
            MinionRange.GetComponent<Character>().Nivel += 1;
            MinionSiege.GetComponent<Character>().Nivel += 1;

            MinionMelee.GetComponent<Character>().VidaMax += 6f;
            MinionRange.GetComponent<Character>().VidaMax += 4f;
            MinionSiege.GetComponent<Character>().VidaMax += 5.5f;

            MinionMelee.GetComponent<Character>().Daño += 0.6f;
            MinionRange.GetComponent<Character>().Daño += 0.4f;
            MinionSiege.GetComponent<Character>().Daño += 0.55f;

            UpgradeTimer = 0;
            Siege = true;
        }
        
        RotateMinion(MinionMelee);
        
        yield return new WaitForSeconds(1);

       
        RotateMinion(MinionMelee);

        yield return new WaitForSeconds(1);

        RotateMinion(MinionMelee);

        yield return new WaitForSeconds(1);

        if (Siege)
        {
            RotateMinion(MinionSiege);

        }
        else if (!Siege)
        {
            RotateMinion(MinionRange);
        }

        yield return new WaitForSeconds(1);

        RotateMinion(MinionRange);

        yield return new WaitForSeconds(1);

        RotateMinion(MinionRange);
        Timer = 30f;
        if (Siege)
        {
            Siege = false;
        }

    }
}
