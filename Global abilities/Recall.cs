using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Recall : MonoBehaviour 
{
	public float Tiempo;
	public GameObject Efecto;

	public bool Isbot;
	public bool IsPlayer;
	public bool HaveAnimation;

	public Collider[] Targets;
	public LayerMask Enemigo;


    public Sprite Icon;
    public Image IconSpace;

    public float TiempoDeUso;
	public bool PuedeUsarse;
	// Use this for initialization
	void Start () 
	{
		PuedeUsarse = true;
        if (IsPlayer)
        {
            IconSpace = GameObject.Find("SpellB").GetComponent<Image>();
            IconSpace.sprite = Icon;
        }
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (gameObject.tag == "TeamRed") 
		{
			Enemigo = (1 << LayerMask.NameToLayer("TeamBlue"))
				|	  (1 << LayerMask.NameToLayer("Jungle"))
					|	  (1 << LayerMask.NameToLayer("ChampionBlue"))
					|	  (1 << LayerMask.NameToLayer("BuildingBlue"));
		}
		if (gameObject.tag == "TeamBlue") 
		{
			Enemigo = (1 << LayerMask.NameToLayer("TeamRed"))
				|	  (1 << LayerMask.NameToLayer("Jungle"))
					|	  (1 << LayerMask.NameToLayer("ChampionRed"))
					|	  (1 << LayerMask.NameToLayer("BuildingRed"));
		}
		FindTargets ();
		if (IsPlayer) 
		{
			if (Input.GetKeyDown (KeyCode.Mouse1) && gameObject.GetComponent<Character>().IsAlive == true) 
			{
				Tiempo = 0;
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
				StopCoroutine(TP());
				Efecto.SetActive(false);
                gameObject.GetComponent<ClickToMove>().animator.SetBool("Recall", false);
            }
			if(Input.GetKeyDown(KeyCode.B)&& Tiempo ==0 && gameObject.GetComponent<Character>().IsAlive == true)
			{
				Tiempo = 8f;
				if(Application.loadedLevel == 2 
				   || Application.loadedLevel == 4 
				   || Application.loadedLevel == 5)
				{
					Efecto.SetActive(true);
				}
				
			}
			if (Tiempo != 0) 
			{
				Tiempo -= Time.deltaTime;
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
                if (HaveAnimation)
                {
                    gameObject.GetComponent<ClickToMove>().animator.SetBool("Recall", true);
                    //gameObject.GetComponent<ClickToMove>().animator.SetInteger("State", 3);
                }
                if (!HaveAnimation)
                {
                    gameObject.GetComponent<ClickToMove>().animator.SetBool("Recall", false);
                    gameObject.GetComponent<ClickToMove>().animator.SetFloat("State", 0);
                }
            }
			if (Tiempo < 0) 
			{
				Tiempo=0;
				StartCoroutine(TP());
			}
			if(!gameObject.GetComponent<Character>().IsAlive)
			{
				Tiempo = 0;
				StopCoroutine(TP());
				Efecto.SetActive(false);
			}

		}
		if(Isbot)
		{
			if(Application.loadedLevel == 2 
			   || Application.loadedLevel == 4
			   || Application.loadedLevel == 5 
			   || Application.loadedLevel == 7)
			{
				if((gameObject.GetComponent<Character>().Vida * 100f)/gameObject.GetComponent<Character>().VidaMax < 40f
				   && Vector3.Distance(transform.position,gameObject.GetComponent<Character>().Base.position)>9f
				   && Targets.Length ==0
				   &&gameObject.GetComponent<Character>().IsAlive)
				{
					if(PuedeUsarse)
					{
						if(Tiempo ==0)
						{
							Tiempo = 8f;
							Efecto.SetActive(true);
							if(HaveAnimation)
							{
								gameObject.GetComponent<IABots>().animator.SetBool("Recall", true);
								//gameObject.GetComponent<IABots>().animator.SetInteger("State",3);
							}
							if(!HaveAnimation)
							{
								gameObject.GetComponent<IABots>().animator.SetBool("IsAttacking",false);
                                gameObject.GetComponent<IABots>().animator.SetBool("Recall", false);
                                gameObject.GetComponent<IABots>().animator.SetFloat("State",0);
							}
						}
					}
				}
				if(Targets.Length >0 && gameObject.GetComponent<Character>().Vida > 0 
                    || (gameObject.GetComponent<Character>().Vida * 100f)/ gameObject.GetComponent<Character>().VidaMax > 40f)
				{
					TiempoDeUso = 2f;
					Tiempo = 0;
					PuedeUsarse = false;
					gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
					StopCoroutine(TP());
					Efecto.SetActive(false);
                    gameObject.GetComponent<IABots>().animator.SetBool("Recall", false);
                }
				if(TiempoDeUso > 0)
				{
					TiempoDeUso -= Time.deltaTime;
				}
				if(TiempoDeUso < 0)
				{
					TiempoDeUso = 0;
					PuedeUsarse = true;
				}
				if (Tiempo != 0) 
				{
					Tiempo -= Time.deltaTime;
					gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = false;
				}
				if (Tiempo < 0) 
				{
					Tiempo=0;
					StartCoroutine(TP());
				}
				if(!gameObject.GetComponent<Character>().IsAlive)
				{
					TiempoDeUso = 2f;
					Tiempo = 0;
					PuedeUsarse = false;
					StopCoroutine(TP());
					Efecto.SetActive(false);
				}
			}
		}

	}

	void FindTargets() 
	{
		Targets = Physics.OverlapSphere(transform.position, 5.5f,Enemigo);
	}


	IEnumerator TP()
	{
		if(Application.loadedLevel == 2 
		   || Application.loadedLevel == 4
		   || Application.loadedLevel == 5)
		{
			if(IsPlayer)
			{
				GameObject.Find("FollowCam").transform.position = gameObject.GetComponent<Character>().Base.position;
			}
			if (gameObject.tag == "TeamRed") 
			{
				transform.position = gameObject.GetComponent<Character>().Base.position;
			}
			if (gameObject.tag == "TeamBlue") 
			{
				transform.position = gameObject.GetComponent<Character>().Base.position;
			}
		}
		yield return new WaitForSeconds (0.3f);
		if (IsPlayer) 
		{
			gameObject.GetComponent<ClickToMove>().Objetivo = null;
			gameObject.GetComponent<ClickToMove>().animator.SetBool("Recall", false);
			gameObject.GetComponent<ClickToMove>().animator.SetFloat("State",0);
			if(Application.loadedLevel == 2 
			   || Application.loadedLevel == 4 
			   || Application.loadedLevel == 5)
			{
				if (gameObject.tag == "TeamRed") 
				{
					gameObject.GetComponent<ClickToMove>().Position = gameObject.GetComponent<Character>().Base.position;
				}
				if (gameObject.tag == "TeamBlue") 
				{
					gameObject.GetComponent<ClickToMove>().Position = gameObject.GetComponent<Character>().Base.position;
				}
				Efecto.SetActive(false);
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
			}

		}
		if (Isbot) 
		{
			gameObject.GetComponent<IABots>().Objetivo = null;
			gameObject.GetComponent<IABots>().animator.SetBool("Recall", false);
			gameObject.GetComponent<IABots>().animator.SetFloat("State",0);
			if(Application.loadedLevel == 2 
			   || Application.loadedLevel == 4 
			   || Application.loadedLevel == 5)
			{
				if (gameObject.tag == "TeamRed") 
				{
					gameObject.GetComponent<IABots>().punto = 0;
				}
				if (gameObject.tag == "TeamBlue") 
				{
					gameObject.GetComponent<IABots>().punto = 0;
				}
				Efecto.SetActive(false);
				gameObject.GetComponent<UnityEngine.AI.NavMeshAgent> ().enabled = true;
			}
			
		}

	}
}
