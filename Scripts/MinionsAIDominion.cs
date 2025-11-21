using UnityEngine;
using System.Collections;

public class MinionsAIDominion : MonoBehaviour
{
    public Transform[] camino;
    public Collider[] Targets;
    public LayerMask Enemigo;
    public GameObject Objetivo;
    public Transform ultimopunto;
    public float Rango;
    public float RangoDeDeteccion;

    public bool Bot;
    public bool Top;


    public int punto;
    public int N;
    public UnityEngine.AI.NavMeshAgent nav;
    public Animator animator;

    public bool EquipoRojo;

    int ATT;


    void OnEnable()
    {

        if (Bot)
        {
            if (!EquipoRojo)
            {
                camino = new Transform[5];
                camino[0] = GameObject.Find("botA1").transform;
                camino[1] = GameObject.Find("botA2").transform;
                camino[2] = GameObject.Find("botA3").transform;
                camino[3] = GameObject.Find("botA4").transform;
                camino[4] = GameObject.Find("botA5").transform;
                ultimopunto = camino[punto];
            }
            if (EquipoRojo)
            {
                camino = new Transform[5];
                camino[0] = GameObject.Find("botB1").transform;
                camino[1] = GameObject.Find("botB2").transform;
                camino[2] = GameObject.Find("botB3").transform;
                camino[3] = GameObject.Find("botB4").transform;
                camino[4] = GameObject.Find("botB5").transform;
                ultimopunto = camino[punto];
            }
        }
        else if (Top)
        {
            if (!EquipoRojo)
            {
                camino = new Transform[8];
                camino[0] = GameObject.Find("topA1").transform;
                camino[1] = GameObject.Find("topA2").transform;
                camino[2] = GameObject.Find("topA3").transform;
                camino[3] = GameObject.Find("topA4").transform;
                camino[4] = GameObject.Find("topB4").transform;
                camino[5] = GameObject.Find("topB3").transform;
                camino[6] = GameObject.Find("topB2").transform;
                camino[7] = GameObject.Find("topB1").transform;
                ultimopunto = camino[punto];
            }
            if (EquipoRojo)
            {
                camino = new Transform[8];
                camino[0] = GameObject.Find("topB1").transform;
                camino[1] = GameObject.Find("topB2").transform;
                camino[2] = GameObject.Find("topB3").transform;
                camino[3] = GameObject.Find("topB4").transform;
                camino[4] = GameObject.Find("topA4").transform;
                camino[5] = GameObject.Find("topA3").transform;
                camino[6] = GameObject.Find("topA2").transform;
                camino[7] = GameObject.Find("topA1").transform;
                ultimopunto = camino[punto];
            }
        }

    }



    void Update()
    {
        if (EquipoRojo)
        {
            gameObject.tag = "TeamRed";
            Enemigo = (1 << LayerMask.NameToLayer("TeamBlue"))
                | (1 << LayerMask.NameToLayer("Jungle"))
                | (1 << LayerMask.NameToLayer("ChampionBlue"))
                | (1 << LayerMask.NameToLayer("BuildingBlue"));
        }
        if (!EquipoRojo)
        {
            gameObject.tag = "TeamBlue";
            Enemigo = (1 << LayerMask.NameToLayer("TeamRed"))
                | (1 << LayerMask.NameToLayer("Jungle"))
                | (1 << LayerMask.NameToLayer("ChampionRed"))
                | (1 << LayerMask.NameToLayer("BuildingRed"));
        }

        if (nav.enabled)
        {
            FindTargets();

            if (gameObject.GetComponent<Character>().IsCreep)
            {
                if (Objetivo)
                {

                    float Dis = Vector3.Distance(transform.position, Objetivo.transform.position);
                    if (Dis <= Rango)
                    {
                        animator.SetBool("IsAttacking", true);
                        nav.Stop();
                        Vector3 targetPostition = new Vector3(Objetivo.transform.position.x, this.transform.position.y, Objetivo.transform.position.z);
                        transform.LookAt(targetPostition);

                    }
                    if (Dis > Rango && Dis <= RangoDeDeteccion)
                    {
                        nav.Resume();
                        animator.SetBool("IsAttacking", false);
                        animator.SetInteger("State", 1);
                        nav.SetDestination(Objetivo.transform.position);
                    }
                    if (Dis > RangoDeDeteccion
                       || Objetivo.GetComponent<Debuffs>().Invulnerable)
                    {
                        nav.Resume();
                        animator.SetBool("IsAttacking", false);
                        animator.SetInteger("State", 1);
                        Objetivo = null;
                    }
                    if (Objetivo.GetComponent<Character>().IsAlive == false)
                    {
                        DetermineTarget();
                    }
                }
                else
                {
                    DetermineTarget();
                    Movimiento();
                }

            }

        }



    }

    void Movimiento()
    {
        if (nav.enabled)
        {
            if (gameObject.GetComponent<Character>().IsCreep)
            {
                if (Vector3.Distance(transform.position, camino[punto].position) > 1f)
                {
                    nav.Resume();
                    nav.SetDestination(camino[punto].position);
                    ultimopunto = camino[punto];
                    animator.SetInteger("State", 1);
                }
                else if (Vector3.Distance(transform.position, camino[punto].position) < 2f)
                {
                    nav.Stop();
                    animator.SetInteger("State", 0);
                    punto++;
                }
                animator.SetBool("IsAttacking", false);
                nav.SetDestination(ultimopunto.position);

                if (Vector3.Distance(transform.position, camino[camino.Length-1].position) < 1f)
                {
                    Destroy(gameObject);
                }
            }

        }

    }
    void DetermineTarget()
    {
        if (Targets.Length >= 1)
        {
            for (int i = 0; i < Targets.Length; i++)
            {
                if (Targets[i].gameObject.GetComponent<Character>().IsAlive)
                {
                    Objetivo = Targets[i].gameObject;
                }
                else
                {
                    DetermineTarget();
                }
            }
        }
        if (Targets.Length < 1)
        {
            Objetivo = null;
        }

    }

    void FindTargets()
    {
        Targets = Physics.OverlapSphere(transform.position, 5.5f, Enemigo);
    }
    
    void randomattack()
    {
        if (ATT == 0)
        {
            animator.SetInteger("attack_state", 1);
            ATT = 1;
        }
        else if (ATT == 1)
        {
            animator.SetInteger("attack_state", 0);
            ATT = 0;
        }

    }

}
