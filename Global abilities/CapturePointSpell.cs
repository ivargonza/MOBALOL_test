using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CapturePointSpell : MonoBehaviour 
{
	public GameObject Efecto;
	public GameObject Objetivo;
	public Transform Root;
	public bool Player;
	public bool Bot;
	public bool IsActivated;
	public bool Islaunched;
	public LayerMask Enemigo;
	private cursor spell;
	float CastTime;
	float tiempodeuso;
	//Stats
	public float rango = 4.7f;
	

	// Use this for initialization
	void Start () 
	{
		if (Player) 
		{

		}
	}
	
	void Update () 
	{
		Enemigo = (1 << LayerMask.NameToLayer ("Capture Point"));

		if (Player) 
		{	
			if(!gameObject.GetComponent<Debuffs> ().Stun)
			{
				if(!gameObject.GetComponent<Debuffs> ().Silence)
				{
					if (Input.GetKeyDown (KeyCode.T) 
					    && gameObject.GetComponent<Character>().IsAlive 
					    && CastTime ==0
					    && tiempodeuso ==0) 
					{
						IsActivated = true;
						spell = gameObject.GetComponent<cursor>();
						spell.spellactive = true;
					}
				}
			}
			if (Input.GetKeyDown (KeyCode.Mouse1) && IsActivated && gameObject.GetComponent<Character>().IsAlive) 
			{
				IsActivated = false;
				spell = gameObject.GetComponent<cursor>();
				spell.spellactive = false;
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
				CastTime = -1f;
				Objetivo = null;
			}
			if (Input.GetKeyDown (KeyCode.Mouse1) && Islaunched && gameObject.GetComponent<Character>().IsAlive) 
			{
				Islaunched = false;
				spell = gameObject.GetComponent<cursor>();
				spell.spellactive = false;
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
				CastTime = -0.5f;
				Objetivo = null;
				StopCoroutine(BeingCapture());
			}
			if (Input.GetKeyDown (KeyCode.Mouse0)&& IsActivated && gameObject.GetComponent<Character>().IsAlive) 
			{
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				spell.spellactive = false;
				if(Physics.Raycast(ray, out hit, 1000,Enemigo))
				{
					
					if(hit.transform.gameObject.GetComponent<Character>().IsBuilding)
					{
						
						Objetivo = hit.transform.gameObject;
						float d = Vector3.Distance (transform.position,Objetivo.transform.position);
						if(d <= rango)
						{
							IsActivated = false;
							Islaunched = true;
							CastTime = 6f;
							Vector3 targetPostition = new Vector3(Objetivo.transform.position.x,this.transform.position.y,Objetivo.transform.position.z);
							transform.LookAt (targetPostition);
							StartCoroutine(BeingCapture());
						}
						
					}
				}
			}
			
			if (CastTime > 0) 
			{
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
				CastTime -= Time.deltaTime;
				gameObject.GetComponent<ClickToMove>().animator.SetBool ("IsAttacking", false);
				gameObject.GetComponent<ClickToMove>().animator.SetInteger ("State", 3);
			}
			if (CastTime < 0) 
			{
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
				CastTime = 0;
				tiempodeuso = 5f;
				Objetivo = null;
				gameObject.GetComponent<ClickToMove>().animator.SetBool ("IsAttacking", false);
				gameObject.GetComponent<ClickToMove>().animator.SetInteger ("State", 0);
			}
			if (tiempodeuso > 0) 
			{
				tiempodeuso -= Time.deltaTime;
			}
			if (tiempodeuso < 0) 
			{
				tiempodeuso = 0;
			}

			if(Objetivo)
			{
				Efecto.GetComponent<LineRenderer> ().SetPosition (0, Root.position);
				Efecto.GetComponent<LineRenderer> ().SetPosition (1, Objetivo.gameObject.GetComponent<TorretaRojaAI>().Launcher.position);
				Efecto.SetActive(true);
				if (Vector3.Distance (Objetivo.transform.position, this.transform.position) > rango) 
				{
					Objetivo = null;
					CastTime = -1f;
				}
			}
			if(!Objetivo)
			{
				Efecto.GetComponent<LineRenderer> ().SetPosition (0, Root.position);
				Efecto.GetComponent<LineRenderer> ().SetPosition (1, Root.position);
				Efecto.SetActive(false);
				Objetivo = null;
			}
			
		}
		
		if (Bot) 
		{
			if(!gameObject.GetComponent<Debuffs> ().Stun)
			{
				if(!gameObject.GetComponent<Debuffs> ().Silence)
				{
					if(gameObject.GetComponent<IABots> ().CapturePoint.Length > 0
					   && gameObject.GetComponent<Character> ().IsAlive 
					   && gameObject.GetComponent<IABots> ().CapPo != null 
					   && gameObject.GetComponent<IABots> ().ItsSafe
					   && CastTime == 0
					   && tiempodeuso ==0)
					{
						if(gameObject.GetComponent<IABots> ().CapPo.gameObject.GetComponent<Character>().IsBuilding
						   && Vector3.Distance (gameObject.GetComponent<IABots> ().CapPo.transform.position, this.transform.position) <= rango)
						{
							Objetivo = gameObject.GetComponent<IABots> ().CapPo;

							if(Objetivo.tag != gameObject.tag &&  Objetivo.gameObject.GetComponent<CapturePoint>().IsCaptured
							   ||Objetivo.tag == "CapturePoint" &&  !Objetivo.gameObject.GetComponent<CapturePoint>().IsCaptured)
							{
								CastTime = 6f;
								StartCoroutine(BeingCapture());
							}

						}
					}
					if(!gameObject.GetComponent<IABots> ().ItsSafe
					   && gameObject.GetComponent<Character> ().IsAlive)
					{
						Objetivo = null;
						CastTime = 0;
						gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
					}
				}
			}
			
			if (CastTime > 0) 
			{
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
				CastTime -= Time.deltaTime;
				gameObject.GetComponent<IABots>().animator.SetBool ("IsAttacking", false);
				gameObject.GetComponent<IABots>().animator.SetInteger ("State", 3);
			}
			if (CastTime < 0) 
			{
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
				CastTime = 0;
				Objetivo = null;
				gameObject.GetComponent<IABots>().animator.SetBool ("IsAttacking", false);
				gameObject.GetComponent<IABots>().animator.SetInteger ("State", 0);
				StopCoroutine(BeingCapture());
			}
			
			if(Objetivo && CastTime != 0)
			{
				Efecto.GetComponent<LineRenderer> ().SetPosition (0, Root.position);
				Efecto.GetComponent<LineRenderer> ().SetPosition (1, Objetivo.gameObject.GetComponent<TorretaRojaAI>().Launcher.position);
				Efecto.SetActive(true);
				if (Vector3.Distance (Objetivo.transform.position, this.transform.position) > rango
				    || gameObject.GetComponent<Character>().Vida <= 0) 
				{
					Objetivo = null;
					CastTime = -1f;
				}
			}
			if(!Objetivo || CastTime ==0 )
			{
				Efecto.GetComponent<LineRenderer> ().SetPosition (0, Root.position);
				Efecto.GetComponent<LineRenderer> ().SetPosition (1, Root.position);
				Efecto.SetActive(false);
				Objetivo = null;
			}
		}
	}

	IEnumerator BeingCapture()
	{
		Objetivo.GetComponent<CapturePoint> ().CapturingFase1 ();
		Objetivo.GetComponent<CapturePoint> ().Captor = this.gameObject;
		Objetivo.GetComponent<TorretaRojaAI> ().Target = null;
		yield return new WaitForSeconds (5.5f);
		if(Objetivo != null)
		{
			Objetivo.GetComponent<CapturePoint> ().CapturingFase2 ();
		}

	}

}
