using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Announcer : MonoBehaviour 
{
	[System.Serializable]
	public class Announcement
	{
		public AudioClip clip;
		public string text;
		public float duration;
		
		public Announcement(AudioClip audioClip, string message, float time = 2f)
		{
			clip = audioClip;
			text = message;
			duration = time;
		}
	}
	
	[Header("Audio")]
	public AudioSource Au;
	
	[Header("Classic Mode Announcements")]
	public AudioClip[] Anunciadora;
	
	[Header("Dominion Announcements")]
	public AudioClip[] AnunciadoraDominion;
	public AudioClip QuarryCapturedPurple;
	public AudioClip QuarryCapturedBlue;
	public AudioClip RefineryCapturedPurple;
	public AudioClip RefineryCapturedBlue;
	public AudioClip WindmillCapturedPurple;
	public AudioClip WindmillCapturedBlue;
	public AudioClip DrillCapturedPurple;
	public AudioClip DrillCapturedBlue;
	public AudioClip BoneyardCapturedPurple;
	public AudioClip BoneyardCapturedBlue;
	
	[Header("UI")]
	public Text Texto;
	public Text Textosombra;
	
	[Header("Settings")]
	public string WellcomeText = "Welcome to Summoner's Rift!";
	public string ThirtySecondsText = "Thirty seconds until minions spawn!";
	public string BattleBegText = "Minions have spawned!";
	
	public bool PS = true; // First Blood flag
	
	private ChampContainer cont;
	private Queue<Announcement> announcementQueue;
	private bool isPlayingAnnouncement;
	
	// Cache de mensajes comunes
	private Dictionary<string, string> cachedMessages;
	
	private void Awake()
	{
		announcementQueue = new Queue<Announcement>();
		InitializeMessageCache();
	}
	
	private void Start() 
	{
		GameObject container = GameObject.Find("ChampContainer");
		if (container != null)
		{
			cont = container.GetComponent<ChampContainer>();
		}
		StartCoroutine(InitialSequence());
	}
	
	private void InitializeMessageCache()
	{
		cachedMessages = new Dictionary<string, string>
		{
			{ "ally_turret", "Your turret has been Destroyed!" },
			{ "enemy_turret", "Your team has destroyed a turret!" },
			{ "ally_inhibitor", "Your inhibitor has been Destroyed!" },
			{ "enemy_inhibitor", "Your team has destroy an inhibitor!" },
			{ "player_killed", "You have been slain!" },
			{ "first_blood", "First blood!" },
			{ "ally_killed", "An allied have been slain!" },
			{ "enemy_killed", "An enemy have been slain!" },
			
			// Dominion
			{ "quarry_blue", "Blue team has capture The Quarry" },
			{ "quarry_purple", "Purple team has capture The Quarry" },
			{ "windmill_blue", "Blue team has capture The Windmill" },
			{ "windmill_purple", "Purple team has capture The Windmill" },
			{ "drill_blue", "Blue team has capture The Drill" },
			{ "drill_purple", "Purple team has capture The Drill" },
			{ "boneyard_blue", "Blue team has capture The Boneyard" },
			{ "boneyard_purple", "Purple team has capture The Boneyard" },
			{ "refinery_blue", "Blue team has capture The Refinery" },
			{ "refinery_purple", "Purple team has capture The Refinery" }
		};
	}
	
	private IEnumerator InitialSequence()
	{
		yield return new WaitForSeconds(30f);
		QueueAnnouncement(new Announcement(Anunciadora[0], WellcomeText));
		
		yield return new WaitForSeconds(30f);
		QueueAnnouncement(new Announcement(Anunciadora[1], ThirtySecondsText));
		
		yield return new WaitForSeconds(30f);
		QueueAnnouncement(new Announcement(Anunciadora[2], BattleBegText, 3f));
	}
	
	public void QueueAnnouncement(Announcement announcement)
	{
		announcementQueue.Enqueue(announcement);
		
		if (!isPlayingAnnouncement)
		{
			StartCoroutine(ProcessQueue());
		}
	}
	

	private IEnumerator ProcessQueue()
	{
		isPlayingAnnouncement = true;
		
		while (announcementQueue.Count > 0)
		{
			Announcement current = announcementQueue.Dequeue();
			
			if (current.clip != null && Au != null)
			{
				Au.clip = current.clip;
				Au.Play();
			}
			
			if (Texto != null) Texto.text = current.text;
			if (Textosombra != null) Textosombra.text = current.text;
			
			yield return new WaitForSeconds(current.duration);
			
			if (Texto != null) Texto.text = "";
			if (Textosombra != null) Textosombra.text = "";
			
			// Pequeña pausa entre anuncios
			yield return new WaitForSeconds(0.5f);
		}
		
		isPlayingAnnouncement = false;
	}
	
	// ===== MÉTODOS PÚBLICOS =====
	
	// Turrets
	public void AllTurret()
	{
		if (Anunciadora.Length > 3)
			QueueAnnouncement(new Announcement(Anunciadora[3], cachedMessages["ally_turret"]));
	}
	
	public void EneTurret()
	{
		if (Anunciadora.Length > 4)
			QueueAnnouncement(new Announcement(Anunciadora[4], cachedMessages["enemy_turret"]));
	}
	
	// Inhibitors
	public void AllInhibitor()
	{
		if (Anunciadora.Length > 5)
			QueueAnnouncement(new Announcement(Anunciadora[5], cachedMessages["ally_inhibitor"]));
	}
	
	public void EneInhibitor()
	{
		if (Anunciadora.Length > 6)
			QueueAnnouncement(new Announcement(Anunciadora[6], cachedMessages["enemy_inhibitor"]));
	}
	
	// Dominion - Ally Captures
	public void QuarryAlly()
	{
		QueueAnnouncement(new Announcement(QuarryCapturedBlue, cachedMessages["quarry_blue"]));
	}
	
	public void WindmillAlly()
	{
		QueueAnnouncement(new Announcement(WindmillCapturedBlue, cachedMessages["windmill_blue"]));
	}
	
	public void DrillAlly()
	{
		QueueAnnouncement(new Announcement(DrillCapturedBlue, cachedMessages["drill_blue"]));
	}
	
	public void BoneyardAlly()
	{
		QueueAnnouncement(new Announcement(BoneyardCapturedBlue, cachedMessages["boneyard_blue"]));
	}
	
	public void RefineryAlly()
	{
		QueueAnnouncement(new Announcement(RefineryCapturedBlue, cachedMessages["refinery_blue"]));
	}
	
	// Dominion - Enemy Captures
	public void QuarryEnemy()
	{
		QueueAnnouncement(new Announcement(QuarryCapturedPurple, cachedMessages["quarry_purple"]));
	}
	
	public void WindmillEnemy()
	{
		QueueAnnouncement(new Announcement(WindmillCapturedPurple, cachedMessages["windmill_purple"]));
	}
	
	public void DrillEnemy()
	{
		QueueAnnouncement(new Announcement(DrillCapturedPurple, cachedMessages["drill_purple"]));
	}
	
	public void BoneyardEnemy()
	{
		QueueAnnouncement(new Announcement(BoneyardCapturedPurple, cachedMessages["boneyard_purple"]));
	}
	
	public void RefineryEnemy()
	{
		QueueAnnouncement(new Announcement(RefineryCapturedPurple, cachedMessages["refinery_purple"]));
	}
	
	// Kills
	public void PlayKilled()
	{
		if (Anunciadora.Length <= 7 && Anunciadora.Length <= 8) return;
		
		if (PS)
		{
			QueueAnnouncement(new Announcement(Anunciadora[8], cachedMessages["first_blood"], 2f));
			PS = false;
		}
		else
		{
			QueueAnnouncement(new Announcement(Anunciadora[7], cachedMessages["player_killed"], 2f));
		}
	}
	
	public void AKill()
	{
		if (Anunciadora.Length <= 8 && Anunciadora.Length <= 9) return;
		
		if (PS)
		{
			QueueAnnouncement(new Announcement(Anunciadora[8], cachedMessages["first_blood"], 2f));
			PS = false;
		}
		else
		{
			QueueAnnouncement(new Announcement(Anunciadora[9], cachedMessages["ally_killed"], 2f));
		}
	}
	
	public void EKill()
	{
		if (Anunciadora.Length <= 8 && Anunciadora.Length <= 10) return;
		
		if (PS)
		{
			QueueAnnouncement(new Announcement(Anunciadora[8], cachedMessages["first_blood"], 2f));
			PS = false;
		}
		else
		{
			QueueAnnouncement(new Announcement(Anunciadora[10], cachedMessages["enemy_killed"], 2f));
		}
	}
	
	// ===== MÉTODOS DE UTILIDAD =====
	
	public void ClearQueue()
	{
		announcementQueue.Clear();
	}
	
	public int GetQueueCount()
	{
		return announcementQueue.Count;
	}
	
	public bool IsPlaying()
	{
		return isPlayingAnnouncement;
	}
}