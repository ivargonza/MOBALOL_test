using UnityEngine;

public class ClickToMove : MonoBehaviour  
{
	[Header("Layer Masks")]
	public LayerMask Mapa;
	public LayerMask Enemigo;
	public LayerMask Aliado;
	
	[Header("Settings")]
	public bool HaveVoice;
	public bool SpecialMap;
	public bool EquipoRojo;
	public float Rango = 2f;
	
	[Header("Effects")]
	public GameObject ClickEffect;
	
	[Header("Components")]
	public UnityEngine.AI.NavMeshAgent Navigator;
	public Animator animator;
	public Voice Voz;
	
	[HideInInspector] public Vector3 Position;
	[HideInInspector] public GameObject Objetivo;
	
	private GameObject efectoClick;
	private Camera mainCamera;
	private Debuffs debuffs;
	private UIchar uiChar;
	private int attackState;
	
	private const int TEAM_RED_LAYER = 21;
	private const int TEAM_BLUE_LAYER = 20;
	private const float RAYCAST_DISTANCE = 1000f;
	
	private void Awake()
	{
		mainCamera = Camera.main;
		debuffs = GetComponent<Debuffs>();
		uiChar = GetComponent<UIchar>();
	}
	
	private void Start()
	{
		Position = transform.position;
		
		if (Application.loadedLevel == 6 || Application.loadedLevel == 7)
		{
			SpecialMap = true;
		}
	}
	
	private void FixedUpdate()
	{
		ConfigureTeamLayers();
		HandleInput();
		
		if (Navigator != null && Navigator.enabled)
		{
			MoveToPosition();
			HandleCombat();
		}
	}
	
	private void ConfigureTeamLayers()
	{
		if (EquipoRojo)
		{
			gameObject.tag = "TeamRed";
			Enemigo = (1 << LayerMask.NameToLayer("TeamBlue"))
				| (1 << LayerMask.NameToLayer("Jungle"))
					| (1 << LayerMask.NameToLayer("ChampionBlue"))
					| (1 << LayerMask.NameToLayer("BuildingBlue"));
			
			Aliado = (1 << LayerMask.NameToLayer("TeamRed"))
				| (1 << LayerMask.NameToLayer("ChampionRed"))
					| (1 << LayerMask.NameToLayer("BuildingRed"));
		}
		else
		{
			gameObject.tag = "TeamBlue";
			Enemigo = (1 << LayerMask.NameToLayer("TeamRed"))
				| (1 << LayerMask.NameToLayer("Jungle"))
					| (1 << LayerMask.NameToLayer("ChampionRed"))
					| (1 << LayerMask.NameToLayer("BuildingRed"));
			
			Aliado = (1 << LayerMask.NameToLayer("TeamBlue"))
				| (1 << LayerMask.NameToLayer("ChampionBlue"))
					| (1 << LayerMask.NameToLayer("BuildingBlue"));
		}
	}
	
	private void HandleInput()
	{
		// Left click - Select target
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			SelectTarget();
		}
		
		if (!Navigator.enabled) return;
		
		bool fearActive = debuffs != null && debuffs.Fear;
		
		// Right click - Move or attack
		if (Input.GetKeyDown(KeyCode.Mouse1) && !fearActive)
		{
			ProcessRightClick();
			
			if (HaveVoice && Voz != null)
			{
				Voz.VoiceSounds();
			}
		}
		
		// Hold right click - Continue processing
		if (Input.GetKey(KeyCode.Mouse1) && !fearActive)
		{
			ProcessRightClickHold();
		}
	}
	
	private void SelectTarget()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		// Primero intentar golpear enemigos/aliados (tienen prioridad)
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Enemigo))
		{
			if (uiChar != null) uiChar.Target = hit.transform.gameObject;
			return;
		}
		
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Aliado))
		{
			if (uiChar != null) uiChar.Target = hit.transform.gameObject;
			return;
		}
		
		// Si no golpea nada, limpiar target
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Mapa))
		{
			if (uiChar != null) uiChar.Target = null;
		}
	}
	
	private void ProcessRightClick()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		// PRIORIDAD 1: Verificar si clickeamos un enemigo
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Enemigo))
		{
			SetTarget(hit.transform.gameObject);
			
			if (HaveVoice && Voz != null)
			{
				Voz.AttackSounds();
			}
			return;
		}
		
		// PRIORIDAD 2: Verificar si clickeamos el mapa
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Mapa))
		{
			SetMovePosition(hit.point);
			SpawnClickEffect(hit.point);
			return;
		}
	}
	
	private void ProcessRightClickHold()
	{
		Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		// PRIORIDAD 1: Enemigos
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Enemigo))
		{
			SetTarget(hit.transform.gameObject);
			
			if (HaveVoice && Voz != null)
			{
				Voz.AttackSounds();
			}
			return;
		}
		
		// PRIORIDAD 2: Mapa
		if (Physics.Raycast(ray, out hit, RAYCAST_DISTANCE, Mapa))
		{
			SetMovePosition(hit.point);
		}
	}
	
	private void SetMovePosition(Vector3 newPosition)
	{
		Position = newPosition;
		Objetivo = null;
		
		if (uiChar != null) uiChar.Target = null;
		if (animator != null) animator.SetBool("IsAttacking", false);
	}
	
	private void SetTarget(GameObject target)
	{
		Objetivo = target;
		Position = target.transform.position;
		if (uiChar != null) uiChar.Target = target;
	}
	
	private void SpawnClickEffect(Vector3 position)
	{
		if (ClickEffect == null) return;
		
		if (efectoClick == null)
		{
			efectoClick = Instantiate(ClickEffect, position, Quaternion.identity);
		}
		else
		{
			efectoClick.SetActive(true);
			efectoClick.transform.position = position;
		}
	}
	
	private void MoveToPosition()
	{
		float distance = Vector3.Distance(transform.position, Position);
		
		if (distance > 0.3f)
		{
			Navigator.Resume();
			Navigator.SetDestination(Position);
			
			if (animator != null)
			{
				animator.SetFloat("State", 1f);
			}
		}
		else
		{
			if (animator != null)
			{
				animator.SetFloat("State", 0f);
			}
		}
	}
	
	private void HandleCombat()
	{
		if (Objetivo == null) return;
		
		Character targetChar = Objetivo.GetComponent<Character>();
		if (targetChar == null) return;
		
		// Verificar si el objetivo sigue siendo válido
		Debuffs targetDebuffs = Objetivo.GetComponent<Debuffs>();
		if (targetChar.Vida <= 0 || (targetDebuffs != null && targetDebuffs.Invulnerable))
		{
			ClearTarget();
			return;
		}
		
		float distance = Vector3.Distance(transform.position, Objetivo.transform.position);
		
		if (distance <= Rango)
		{
			// En rango de ataque
			Navigator.Stop();
			Position = transform.position;
			
			Vector3 targetPosition = new Vector3(
				Objetivo.transform.position.x,
				transform.position.y,
				Objetivo.transform.position.z
				);
			transform.LookAt(targetPosition);
			
			if (animator != null)
			{
				animator.SetBool("IsAttacking", true);
			}
		}
		else
		{
			// Fuera de rango, seguir al objetivo
			Navigator.Resume();
			Position = Objetivo.transform.position;
			
			if (animator != null)
			{
				animator.SetBool("IsAttacking", false);
			}
		}
	}
	
	private void ClearTarget()
	{
		Objetivo = null;
		if (animator != null)
		{
			animator.SetBool("IsAttacking", false);
		}
	}
	
	private void randomattack()
	{
		if (animator == null) return;
		
		if (attackState == 0)
		{
			animator.SetFloat("attack_state", 1f);
			attackState = 1;
		}
		else
		{
			animator.SetFloat("attack_state", 0f);
			attackState = 0;
		}
	}
}