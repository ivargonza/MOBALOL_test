using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{

    public GameObject[] Modelo;

    public ChampContainer Things;
    public Collider[] Targets;
    public LayerMask Enemigo;

    // Use this for initialization
    void Start ()
    {
        Things = GameObject.Find("ChampContainer").GetComponent<ChampContainer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        SetLayers();
        if(Things.team == 0)
        {
            if(gameObject.tag == "TeamRed")
            {
                FindTargets();
                if (Targets.Length > 0)
                {
                    for (var i = 0; i < Targets.Length; i++)
                    {
                        if (!Targets[i].GetComponent<Character>().IsJungle)
                        {
                            //BarraDeVida.SetActive(true);
                            for (var j = 0; j < Modelo.Length; j++)
                            {
                                Modelo[j].SetActive(true);
                            }
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < Modelo.Length; i++)
                    {
                        Modelo[i].SetActive(false);
                    }
                }
            }
            if (gameObject.tag == "Jungle")
            {
                FindTargetsJungle();
                if (Targets.Length > 0)
                {
                    for (var i = 0; i < Targets.Length; i++)
                    {
                        if (Targets[i].tag == "TeamBlue")
                        {

                            for (var j = 0; j < Modelo.Length; j++)
                            {
                                Modelo[j].SetActive(true);
                            }
                        }
                    }
                }
                else
                {

                    for (var i = 0; i < Modelo.Length; i++)
                    {
                        Modelo[i].SetActive(false);
                    }
                }
            }

        }
        if (Things.team == 1)
        {
            if (gameObject.tag == "TeamBlue")
            {
                FindTargets();
                if (Targets.Length > 0)
                {
                    for (var i = 0; i < Targets.Length; i++)
                    {
                        if (!Targets[i].GetComponent<Character>().IsJungle)
                        {

                            for (var j = 0; j < Modelo.Length; j++)
                            {
                                Modelo[j].SetActive(true);
                            }
                        }
                    }
                }
                else
                {

                    for (var i = 0; i < Modelo.Length; i++)
                    {
                        Modelo[i].SetActive(false);
                    }
                }
            }

            if (gameObject.tag == "Jungle")
            {
                FindTargetsJungle();
                if (Targets.Length > 0)
                {
                    for (var i = 0; i < Targets.Length; i++)
                    {
                        if (Targets[i].tag == "TeamRed")
                        {

                            for (var j = 0; j < Modelo.Length; j++)
                            {
                                Modelo[j].SetActive(true);
                            }
                        }
                    }
                }
                else
                {

                    for (var i = 0; i < Modelo.Length; i++)
                    {
                        Modelo[i].SetActive(false);
                    }
                }
            }
        }

    }

    void SetLayers()
    {
        if (gameObject.GetComponent<Character>().Isbot)
        {
            Enemigo = gameObject.GetComponent<IABots>().Enemigo;
        }
        if (gameObject.GetComponent<Character>().IsJungle)
        {
            Enemigo = gameObject.GetComponent<JungleAI>().Enemigo;
        }
        if (gameObject.GetComponent<Character>().IsCreep)
        {
            Enemigo = gameObject.GetComponent<MinionsAI>().Enemigo;
        }
        if (gameObject.GetComponent<Character>().IsMascot)
        {
            Enemigo = gameObject.GetComponent<MascotAI>().Enemigo;
        }
    }

    IEnumerator DelayFog()
    {
        yield return new WaitForSeconds(1f);

        for (var i = 0; i < Modelo.Length; i++)
        {
            Modelo[i].SetActive(false);
        }
    }

    void FindTargets()
    {
        Targets = Physics.OverlapSphere(transform.position, 10f, Enemigo);
    }
    void FindTargetsJungle()
    {
        Targets = Physics.OverlapSphere(transform.position, 7f, Enemigo);
    }
}
