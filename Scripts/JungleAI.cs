using UnityEngine;
using System.Collections;

public class JungleAI : MonoBehaviour 
{
	public GameObject RespawnPoint;
	public Collider[] Targets;
	public Collider[] JunglaCercana;
	public LayerMask Enemigo;
	public LayerMask Aliado;
	public GameObject Objetivo;
	public float Rango;
	public float RangoDeDeteccion;
	public bool aiming;
	public UnityEngine.AI.NavMeshAgent nav;
	public Animator animator;
	public bool IsAlive = true;
	public bool Isattacked;
	// Use this for initialization
	void Start () 
	{
		Enemigo = (1 << LayerMask.NameToLayer("TeamBlue"))
			|	  (1 << LayerMask.NameToLayer("TeamRed"))
			|	  (1 << LayerMask.NameToLayer("BuildingRed"))
			|	  (1 << LayerMask.NameToLayer("BuildingBlue"))
			|	  (1 << LayerMask.NameToLayer("ChampionRed"))
			|	  (1 << LayerMask.NameToLayer("ChampionBlue"));
		Aliado = (1 << LayerMask.NameToLayer("Jungle"));
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(IsAlive && nav.enabled == true)
		{
			if(Vector3.Distance(transform.position, RespawnPoint.transform.position) <= 5f)
			{
				Movimiento();
				FindTargets();
				FindAliados();
				DetermineTarget();
			}
			if(Objetivo && Isattacked)
			{
				if (JunglaCercana.Length >= 1) 
				{
					for (var i = 0; i < JunglaCercana.Length; i++) 
					{
						JunglaCercana[i].gameObject.GetComponent<JungleAI>().Isattacked =true;
					}
				} 

				float Dis = Vector3.Distance(transform.position, Objetivo.transform.position);
				if(Dis <= Rango)
				{
					animator.SetBool("IsAttacking",true);
					nav.Stop();
					Vector3 targetPostition = new Vector3(Objetivo.transform.position.x,this.transform.position.y,Objetivo.transform.position.z);
					transform.LookAt(targetPostition);
					
				}
				if(Dis > Rango && Dis <= RangoDeDeteccion && Vector3.Distance(transform.position, RespawnPoint.transform.position) < 5f)
				{
					nav.Resume();
					animator.SetBool("IsAttacking",false);
					animator.SetInteger("State",1);
					nav.SetDestination(Objetivo.transform.position);
				}
				if(Objetivo.GetComponent<Character>().Vida <= 0 
				   || Dis > RangoDeDeteccion 
				   || Vector3.Distance(transform.position, RespawnPoint.transform.position) > 5f
				   || Objetivo.GetComponent<Debuffs>().Invulnerable)
				{
					
					Isattacked = false;
					animator.SetBool("IsAttacking",false);
					Objetivo = null;
				}
			}
			if(Objetivo == null && Isattacked)
			{
				Isattacked = false;
				DetermineTarget();
				Movimiento();
			}

		}
	
	}


	void Movimiento()
	{
		if (!Isattacked) 
		{
			if(Vector3.Distance(transform.position, RespawnPoint.transform.position) >= 1)
			{
				nav.Resume();
				nav.SetDestination(RespawnPoint.transform.position);
				animator.SetBool("IsAttacking",false);
				animator.SetInteger("State",1);
			}
			if(Vector3.Distance(transform.position, RespawnPoint.transform.position) <= 0.5f)
			{
				nav.Stop();
				animator.SetInteger("State",0);
				animator.SetBool("IsAttacking",false);
			}
		}

	}
	void DetermineTarget()
	{
		if (Targets.Length >= 1) 
		{
			
			for (var i = 0; i < Targets.Length; i++) 
			{
				Objetivo = Targets [0].gameObject;
			}
		} 
	}
	void FindTargets() 
	{
		Targets = Physics.OverlapSphere(transform.position, 5f,Enemigo);
	}
	void FindAliados() 
	{
		JunglaCercana = Physics.OverlapSphere(transform.position, 5f,Aliado);
	}
	void randomattack()
	{
		animator.SetInteger ("attack_state", Random.Range(0,2));
		
	}
}
