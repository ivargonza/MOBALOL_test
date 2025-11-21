using UnityEngine;
using System.Collections;

public class announcerbuilding : MonoBehaviour 
{
	public Announcer ann;
	public bool isTurret;
	public bool isInhibitor;
	public BackDooring back;

    public ChampContainer Things;

    public int team;
    public bool RedTeam;


    private void Update()
    {
        Things = GameObject.Find("ChampContainer").GetComponent<ChampContainer>();
        team = Things.team;
    }

    void OnEnable() 
	{
        StartCoroutine(LaunchSound());
	}

    IEnumerator LaunchSound()
    {
        yield return new WaitForSeconds(0.1f);
        if (back != null)
        {
            back.Ondeath();
        }
        if (isTurret)
        {
            if (team == 0)
            {
                if (RedTeam)
                {
                    ann.EneTurret();
                    
                }
                else
                {
                   
                    ann.AllTurret();
                }
            }
            if (team == 1)
            {
                if (RedTeam)
                {
                    ann.AllTurret();
                    
                }
                else
                {
                    
                    ann.EneTurret();
                }
            }
        }
		if (isInhibitor) 
		{
			if (team == 0)
			{
				if (RedTeam)
				{
					ann.EneInhibitor();
					
				}
				else
				{
					
					ann.AllInhibitor();
				}
			}
			else if (team == 1)
			{
				if (RedTeam)
				{
					ann.AllInhibitor();
					
				}
				else
				{
					
					ann.EneInhibitor();
				}
			}
			
		}
    }

}
