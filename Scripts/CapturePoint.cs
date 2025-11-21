using UnityEngine;
using System.Collections;

public class CapturePoint : MonoBehaviour 
{
	public string PointName;
	public float TimeCapture;
	public int TeamColor;

	public GameObject Captor;

	public GameObject RedGem;
	public GameObject GreenGem;
	public GameObject WhiteGem;

	public bool IsCaptured;
	public bool TeamRed;
	public bool BCaptured;

	public SpecialMapMeshanics Score;
	public TorretaRojaAI turretFunction;
	public Announcer Ann;

	float periodTime;

	//ui colors
	public Material Green;
	public Material Red;
	public Material White;

	public MeshRenderer IconBorder;

	// Use this for initialization
	void Start () 
	{
		periodTime = 2f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(IsCaptured)
		{
			WhiteGem.SetActive(false);
			if(TeamRed)
			{
				RedGem.SetActive(true);
				GreenGem.SetActive(false);
				gameObject.tag = "TeamRed";
				IconBorder.material = Red;

				if (periodTime > 0) 
				{
					periodTime -= Time.deltaTime;
				}
				if (periodTime < 0) 
				{
					periodTime = 4f;
					if(Score.ScoreBlue > 0)
					{
						Score.ScoreBlue -= 1f;
					}
					if(Score.ScoreBlue == 0)
					{
						gameObject.GetComponent<Collider> ().enabled = false;
					}

				}
			}
			else
			{
				RedGem.SetActive(false);
				GreenGem.SetActive(true);
				gameObject.tag = "TeamBlue";
				IconBorder.material = Green;

				if (periodTime > 0) 
				{
					periodTime -= Time.deltaTime;
				}
				if (periodTime < 0) 
				{
					periodTime = 4f;
					if(Score.ScorePurple > 0)
					{
						Score.ScorePurple -= 1f;
					}
					if(Score.ScorePurple == 0)
					{
						gameObject.GetComponent<Collider> ().enabled = false;
					}

				}
			}
		}
		else
		{
			WhiteGem.SetActive(true);
			gameObject.tag = "CapturePoint";
			IconBorder.material = White;
			TeamColor = -1;

		}



	}

	public void CapturingFase2()
	{

		if(Captor.gameObject.tag == "TeamRed")
		{
			TeamRed = true;
			IsCaptured = true;
			TeamColor = 1;
		}
		else
		{
			TeamRed = false;
			IsCaptured = true;
			TeamColor = 0;

		}
		TeamAnn1();

	}
	public void CapturingFase1()
	{
		IsCaptured = false;
		gameObject.GetComponent<Collider> ().enabled = false;
		StartCoroutine (ReEnableColl ());
	}

	IEnumerator ReEnableColl()
	{
		yield return new WaitForSeconds (7f);
		gameObject.GetComponent<Collider> ().enabled = true;
		BCaptured = false;

	}


	public void TeamAnn1()
	{
		if(TeamColor == 0)
		{
			if(PointName == "Quarry")
			{
				Ann.QuarryAlly();
			}
			if(PointName == "Windmill")
			{
				Ann.WindmillAlly();
			}
			if(PointName == "Drill")
			{
				Ann.DrillAlly();
			}
			if(PointName == "Boneyard")
			{
				Ann.BoneyardAlly();
			}
			if(PointName == "Refinery")
			{
				Ann.RefineryAlly();
			}
		}
		else if(TeamColor == 1)
		{
			if(PointName == "Quarry")
			{
				Ann.QuarryEnemy();
			}
			if(PointName == "Windmill")
			{
				Ann.WindmillEnemy();
			}
			if(PointName == "Drill")
			{
				Ann.DrillEnemy();
			}
			if(PointName == "Boneyard")
			{
				Ann.BoneyardEnemy();
			}
			if(PointName == "Refinery")
			{
				Ann.RefineryEnemy();
			}
		}
	}


}
