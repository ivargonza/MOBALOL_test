using UnityEngine;
using System.Collections;

public class MascotAI : MonoBehaviour 
{
	public GameObject MyOwner;
	
	public Collider[] Targets;
	public LayerMask Enemigo;
	public GameObject Objetivo;
	public float Rango;
	public float RangoDeDeteccion;
	
	public bool IsAlive;
	public bool CanMove;
	
	public UnityEngine.AI.NavMeshAgent nav;
	public Animator animator;
	int ATT;
    Vector3 pos;
	// Use this for initialization
	void Start () 
	{
		pos = MyOwner.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (IsAlive) 
		{
			if(Objetivo && nav.enabled)
			{
				float Dis = Vector3.Distance(transform.position, Objetivo.transform.position);
				nav.SetDestination(Objetivo.transform.position);
				if(Dis <= Rango && Objetivo.GetComponent<Character>().Vida > 1)
				{
					animator.SetBool("IsAttacking",true);
					nav.Stop();
					Vector3 targetPostition = new Vector3(Objetivo.transform.position.x,this.transform.position.y,Objetivo.transform.position.z);
					transform.LookAt(targetPostition);
					
				}
				if(Dis > Rango && Dis <= RangoDeDeteccion && Objetivo.GetComponent<Character>().Vida > 1)
				{
					nav.Resume();
					animator.SetBool("IsAttacking",false);
					animator.SetFloat("State",1f);
					
				}
				if(Objetivo.GetComponent<Character>().Vida <= 0)
				{
					if(Objetivo.GetComponent<Character>().Isbot || Objetivo.GetComponent<Character>().IsPlayer)
					{
						MyOwner.GetComponent<Character>().Experiencia += (Objetivo.GetComponent<Character>().ExperienciaMax - Objetivo.GetComponent<Character>().Experiencia)/3f;
					}
					if(Objetivo.GetComponent<Character>().IsCreep 
					   || Objetivo.GetComponent<Character>().IsJungle 
					   || Objetivo.GetComponent<Character>().IsMascot)
					{
						MyOwner.GetComponent<Character>().Experiencia += Random.Range(10f,20f);
					}
					MyOwner.GetComponent<Character>().ScalingStats();
					nav.Resume();
					animator.SetBool("IsAttacking",false);
					animator.SetFloat("State",1f);
					Objetivo = null;
				}
               
			}
			if(Objetivo == null || Objetivo.GetComponent<Debuffs>().Invulnerable)
			{
				animator.SetBool("IsAttacking", false);
				if (!CanMove)
				{
					animator.SetFloat("State", 0);
				}
				else
				{

					animator.SetFloat("State", 1f);
				}
				Movimiento ();

			}
			FindTargets();
			DetermineTarget ();

		}
		
	}
	
	void Movimiento()
	{
        
        if (nav.enabled) 
		{
			if(CanMove)
			{
                if (Vector3.Distance(transform.position,pos) >6f)
				{
					nav.Resume();
                    if (MyOwner.GetComponent<ClickToMove>())
                    {
                        if (MyOwner.GetComponent<ClickToMove>().Objetivo != null)
                        {
                            pos = MyOwner.GetComponent<ClickToMove>().Objetivo.transform.position;
							nav.SetDestination(pos);
                        }
						if (Objetivo == null)
						{
                            Vector3 RandomPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                            pos = MyOwner.transform.position + RandomPos;
							nav.SetDestination(pos);
                        }
                    }
                    if (MyOwner.GetComponent<IABots>())
                    {
                        if (MyOwner.GetComponent<IABots>().Objetivo != null)
                        {
                            pos = MyOwner.GetComponent<IABots>().Objetivo.transform.position;
							nav.SetDestination(pos);
                        }
						if (Objetivo == null)
						{
                            Vector3 RandomPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                            pos = MyOwner.transform.position + RandomPos;
							nav.SetDestination(pos);
                        }
                    }
                    animator.SetFloat("State",1f);
					animator.SetBool("IsAttacking",false);
				}
				else if(Vector3.Distance(transform.position,pos) <1)
				{
					nav.Stop();
					animator.SetFloat("State",0);
					animator.SetBool("IsAttacking",false);
				}
			}
		}
	}

    void DetermineTarget()
    {
        if (Targets.Length != 0)
        {
            for (int i = 0; i < Targets.Length; i++)
            {

                Collider tMin = null;
                float minDist = Mathf.Infinity;
                Vector3 currentPos = transform.position;
                foreach (Collider t in Targets)
                {
                    float dist = Vector3.Distance(t.gameObject.transform.position, currentPos);
                    if (dist < minDist)
                    {
                        tMin = t;
                        minDist = dist;
                        if (tMin.gameObject.GetComponent<Character>().IsAlive)
                        {
                            Objetivo = tMin.gameObject;
                        }

                    }
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
		Targets = Physics.OverlapSphere(transform.position, RangoDeDeteccion - 1f,Enemigo);
	}
	void randomattack()
	{
		if (ATT == 0) 
		{
			animator.SetFloat ("attack_state",  1f);
			ATT = 1;
		}
		else if (ATT == 1) 
		{
			animator.SetFloat ("attack_state",  0);
			ATT = 0;
		}
		
	}
}
