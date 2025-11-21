using UnityEngine;
using UnityEngine.AI;

public class MinionsAI : MonoBehaviour
{
	[Header("Rutas y objetivos")]
	public Transform[] camino;
	public Collider[] Targets;
	public GameObject Objetivo;
	public Transform ultimopunto;
	
	[Header("Detección y ataque")]
	public LayerMask Enemigo;
	public float Rango = 2f;
	public float RangoDeDeteccion = 7f;
	
	[Header("Configuración de línea")]
	public bool Mid, Bot, Top;
	public bool EquipoRojo;
	
	[Header("Componentes")]
	public NavMeshAgent nav;
	public Animator animator;
	public CapsuleCollider col;
	
	private int punto;
	private float ATT;
	private bool pathInitialized;
	
	void OnEnable()
	{
		if (nav == null) nav = GetComponent<NavMeshAgent>();
		if (animator == null) animator = GetComponent<Animator>();
		if (col == null) col = GetComponent<CapsuleCollider>();
		
		// Reset de estado
		nav.enabled = true;
		nav.isStopped = false;
		col.enabled = true;
		animator.SetBool("IsAttacking", false);
		animator.SetFloat("State", 1f);
		Objetivo = null;
		punto = 0;
		
		// ⚙️ Solo se crea el camino cuando el spawner asigna el carril
		//if (!pathInitialized && (Mid || Bot || Top))
		//{
			InitializePath();
			pathInitialized = true;
		//}
		
		// 🧭 Ajuste de posición al activarse desde el pool
		if (camino != null && camino.Length > 0)
		{
			nav.Warp(camino[0].position);
			transform.position = camino[0].position;
			ultimopunto = camino[0];
		}
	}
	
	public void InitializePath()
	{
		string linea = Mid ? "Mid" : Bot ? "Bot" : "Top";
		string[] sufijosR = { "6_A", "5_B", "4_C", "3_D", "2_E", "1_F" };
		string[] sufijosB = { "1_F", "2_E", "3_D", "4_C", "5_B", "6_A" };
		camino = new Transform[7];
		
		if (EquipoRojo)
		{
			for (int i = 0; i < sufijosR.Length; i++)
				camino[i] = GameObject.Find("Waypoint Minion" + linea + sufijosR[i]).transform;
			camino[6] = GameObject.Find("Spawner Minion" + linea).transform;
		}
		else
		{
			camino[0] = GameObject.Find("Spawner Minion" + linea).transform;
			for (int i = 0; i < sufijosB.Length; i++)
				camino[i + 1] = GameObject.Find("Waypoint Minion" + linea + sufijosB[i]).transform;
		}
		
		// ⚑ Configurar equipo y capas
		string equipo = EquipoRojo ? "TeamRed" : "TeamBlue";
		gameObject.tag = equipo;
		gameObject.layer = LayerMask.NameToLayer(equipo);
		
		Enemigo = EquipoRojo
			? (1 << LayerMask.NameToLayer("TeamBlue")) | (1 << LayerMask.NameToLayer("ChampionBlue")) | (1 << LayerMask.NameToLayer("BuildingBlue"))
				: (1 << LayerMask.NameToLayer("TeamRed")) | (1 << LayerMask.NameToLayer("ChampionRed")) | (1 << LayerMask.NameToLayer("BuildingRed"));
	}
	
	void Update()
	{
		if (!nav.enabled) return;
		FindTargets();
		
		var character = GetComponent<Character>();
		if (!character.IsCreep) return;
		
		if (Objetivo)
			HandleAttack();
		else
		{
			DetermineTarget();
			Movimiento();
		}
	}
	
	void HandleAttack()
	{
		if (!Objetivo) return;
		
		float distancia = Vector3.Distance(transform.position, Objetivo.transform.position);
		var debuffs = Objetivo.GetComponent<Debuffs>();
		var objetivoChar = Objetivo.GetComponent<Character>();
		
		if (objetivoChar == null || debuffs == null)
		{
			Objetivo = null;
			return;
		}
		
		if (!objetivoChar.IsAlive || debuffs.Invulnerable || debuffs.Invisible)
		{
			ResetTarget();
			return;
		}
		
		if (distancia <= Rango)
		{
			nav.isStopped = true;
			animator.SetBool("IsAttacking", true);
			LookAtTarget();
		}
		else if (distancia <= RangoDeDeteccion)
		{
			nav.isStopped = false;
			animator.SetBool("IsAttacking", false);
			animator.SetFloat("State", 1f);
			nav.SetDestination(Objetivo.transform.position);
		}
		else ResetTarget();
	}
	
	void ResetTarget()
	{
		Objetivo = null;
		nav.isStopped = false;
		animator.SetBool("IsAttacking", false);
		animator.SetFloat("State", 1f);
	}
	
	void Movimiento()
	{
		if (!nav.enabled || camino == null || camino.Length == 0) return;
		
		float distancia = Vector3.Distance(transform.position, camino[punto].position);
		
		if (distancia > 1f)
		{
			nav.isStopped = false;
			nav.SetDestination(camino[punto].position);
			ultimopunto = camino[punto];
			animator.SetFloat("State", 1f);
		}
		else if (distancia < 2f && punto < camino.Length - 1)
		{
			punto++;
		}
		
		animator.SetBool("IsAttacking", false);
	}
	
	void DetermineTarget()
	{
		if (Targets == null || Targets.Length == 0)
		{
			Objetivo = null;
			return;
		}
		
		Collider objetivoCercano = null;
		float distanciaMinima = Mathf.Infinity;
		Vector3 posicion = transform.position;
		
		foreach (var t in Targets)
		{
			if (t == null) continue;
			var c = t.GetComponent<Character>();
			var d = t.GetComponent<Debuffs>();
			
			if (c == null || d == null || !c.IsAlive || d.Invulnerable || d.Invisible) continue;
			
			float dist = Vector3.Distance(t.transform.position, posicion);
			if (dist < distanciaMinima)
			{
				distanciaMinima = dist;
				objetivoCercano = t;
			}
		}
		
		Objetivo = objetivoCercano ? objetivoCercano.gameObject : null;
	}
	
	void FindTargets()
	{
		Targets = Physics.OverlapSphere(transform.position, RangoDeDeteccion, Enemigo);
	}
	
	void LookAtTarget()
	{
		Vector3 lookPos = new Vector3(Objetivo.transform.position.x, transform.position.y, Objetivo.transform.position.z);
		transform.LookAt(lookPos);
	}
	
	void randomattack()
	{
		ATT = 1 - ATT;
		animator.SetFloat("attack_state", ATT);
	}
}