using UnityEngine;
using System.Collections.Generic;

public class KillingSpreeSystem : MonoBehaviour
{
	[System.Serializable]
	public class SpreeData
	{
		public int killsRequired;
		public string spreeName;
		public AudioClip announceClip;
		public int goldBonus;
		
		public SpreeData(int kills, string name, int gold)
		{
			killsRequired = kills;
			spreeName = name;
			goldBonus = gold;
		}
	}
	
	[Header("Spree Configuration")]
	public List<SpreeData> spreeList = new List<SpreeData>();
	
	[Header("Audio")]
	public AudioClip doubleKillClip;
	public AudioClip tripleKillClip;
	public AudioClip quadraKillClip;
	public AudioClip pentaKillClip;
	
	private Character character;
	private Announcer announcer;
	
	// Tracking
	private int consecutiveKills;
	private int multiKillCount;
	private float multiKillTimer;
	private const float MULTI_KILL_WINDOW = 10f;
	
	// Rewards
	private const int SHUTDOWN_GOLD = 500;
	
	private void Awake()
	{
		character = GetComponent<Character>();
		InitializeDefaultSprees();
	}
	
	private void Start()
	{
		GameObject announcerObj = GameObject.Find("Announcer");
		if (announcerObj != null)
		{
			announcer = announcerObj.GetComponent<Announcer>();
		}
	}
	
	private void Update()
	{
		if (multiKillTimer > 0f)
		{
			multiKillTimer -= Time.deltaTime;
			if (multiKillTimer <= 0f)
			{
				multiKillCount = 0;
			}
		}
	}
	
	private void InitializeDefaultSprees()
	{
		if (spreeList.Count == 0)
		{
			spreeList.Add(new SpreeData(3, "Killing Spree", 100));
			spreeList.Add(new SpreeData(4, "Rampage", 150));
			spreeList.Add(new SpreeData(5, "Unstoppable", 200));
			spreeList.Add(new SpreeData(6, "Dominating", 250));
			spreeList.Add(new SpreeData(7, "Godlike", 300));
			spreeList.Add(new SpreeData(8, "Legendary", 400));
		}
	}
	
	public void OnKill(Character victim)
	{
		if (victim == null) return;
		
		consecutiveKills++;
		multiKillCount++;
		multiKillTimer = MULTI_KILL_WINDOW;
		
		// Check killing spree
		CheckKillingSpree();
		
		// Check multi-kill
		CheckMultiKill();
		
		// End victim's spree if they had one
		KillingSpreeSystem victimSpree = victim.GetComponent<KillingSpreeSystem>();
		if (victimSpree != null)
		{
			int shutdownGold = victimSpree.EndSpree();
			if (shutdownGold > 0)
			{
				AwardGold(shutdownGold);
				AnnounceShutdown(victim.gameObject.name, shutdownGold);
			}
		}
	}
	
	private void CheckKillingSpree()
	{
		foreach (SpreeData spree in spreeList)
		{
			if (consecutiveKills == spree.killsRequired)
			{
				AnnounceSpree(spree);
				AwardGold(spree.goldBonus);
				break;
			}
		}
	}
	
	private void CheckMultiKill()
	{
		AudioClip multiKillClip = null;
		string message = "";
		
		switch (multiKillCount)
		{
		case 2:
			multiKillClip = doubleKillClip;
			message = "Double Kill!";
			break;
		case 3:
			multiKillClip = tripleKillClip;
			message = "Triple Kill!";
			break;
		case 4:
			multiKillClip = quadraKillClip;
			message = "Quadra Kill!";
			break;
		case 5:
			multiKillClip = pentaKillClip;
			message = "PENTA KILL!";
			break;
		}
		
		if (multiKillClip != null && announcer != null)
		{
			Announcer.Announcement announcement = new Announcer.Announcement(multiKillClip, message, 3f);
			announcer.QueueAnnouncement(announcement);
		}
	}
	
	private void AnnounceSpree(SpreeData spree)
	{
		if (announcer == null) return;
		
		string message = gameObject.GetComponent<UIchar>().Nombre.text + " is on a " + spree.spreeName + "!";
		Announcer.Announcement announcement = new Announcer.Announcement(
			spree.announceClip, 
			message, 
			4f
			);
		announcer.QueueAnnouncement(announcement);
	}
	
	private void AnnounceShutdown(string victimName, int gold)
	{
		if (announcer == null) return;
		
		string message = "Shutdown! +" + gold + " gold";
		Announcer.Announcement announcement = new Announcer.Announcement(
			null, 
			message, 
			3f
			);
		announcer.QueueAnnouncement(announcement);
	}
	
	private void AwardGold(int amount)
	{
		InventorySystem inventory = GetComponent<InventorySystem>();
		if (inventory != null)
		{
			inventory.AddGold(amount);
		}
	}
	
	public int EndSpree()
	{
		int goldReward = 0;
		
		if (consecutiveKills >= 3)
		{
			goldReward = SHUTDOWN_GOLD;
		}
		
		consecutiveKills = 0;
		multiKillCount = 0;
		multiKillTimer = 0f;
		
		return goldReward;
	}
	
	public int GetCurrentSpree()
	{
		return consecutiveKills;
	}
	
	public int GetMultiKillCount()
	{
		return multiKillCount;
	}
}