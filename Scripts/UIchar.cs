using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class UIchar : MonoBehaviour 
{
	[Header("World Canvas")]
	public Canvas cam;
	public Image BarraDeVida;
	public Image MarcoBarraDeVida;
	public Text nivelactual;
	public Material red;
	public Material blue;
	public MeshRenderer MaterialPlane;
	
	[Header("HUD Elements")]
	public Image BarraDeVidaHUD;
	public Text CantidadDeVida;
	public Image BarraDeManaHUD;
	public Text CantidadDeMana;
	public Text Armor;
	public Text Damage;
	public Text Level;
	public Text SkilsPoints;
	public Text ChampName;
	public string ChName;
	public Image experiencia;
	public GameObject LevelUpback;
	
	[Header("Target UI")]
	public Image BarraDeVidaTarget;
	public Text CantidadDeVidaTarget;
	public Image BarraDeManaTarget;
	public Text CantidadDeManaTarget;
	public Text ArmorTarget;
	public Text DamageTarget;
	public Image PortraitTarget;
	public GameObject UITarget;
	public GameObject Target;
	
	[Header("Portrait")]
	public Sprite Portrait;
	public GameObject Death;
	public Image PortraitSpace;
	public Text countDown;
	public Text Nombre;
	public Text Oro;
	
	[Header("Effects")]
	public Image vidabaja;
	public GameObject muerto;
	
	// === INVENTARIO DEL JUGADOR (siempre visible) ===
	private Image[] playerItemSlots;
	private Text[] playerItemNames;
	private Sprite emptySlotSprite;
	
	// === STATS ADICIONALES DEL JUGADOR ===
	private Text playerAbilityPowerText;
	private Text playerAttackSpeedText;
	private Text playerMovementSpeedText;
	private Text playerLifeStealText;
	private Text playerCritChanceText;
	private Text playerMagicResistText;
	
	// === INVENTARIO DEL TARGET (visible cuando Target != null) ===
	private Image[] targetItemSlots;
	private Text[] targetItemNames;
	private Text targetGoldText;
	private Text targetLevelText;
	private Text targetKDAText;
	
	// Stats adicionales del target
	private Text targetAbilityPowerText;
	private Text targetAttackSpeedText;
	private Text targetMovementSpeedText;
	private Text targetLifeStealText;
	private Text targetCritChanceText;
	private Text targetMagicResistText;
	
	// === TOOLTIP DE ITEMS ===
	private GameObject tooltipPanel;
	private Text tooltipNameText;
	private Text tooltipDescText;
	private Text tooltipStatsText;
	
	[HideInInspector] public Character myChar;
	
	private ChampContainer things;
	private bool isInitialized;
	private InventorySystem myInventory;
	private InventorySystem targetInventory;
	
	private const string STAT_FORMAT_COLOR = "<color=lime>{0}</color>";
	private const string ARMOR_FORMAT = "<color=lime>{0:F1}({1:F1})</color>";
	
	private void Awake()
	{
		myChar = GetComponent<Character>();
		myInventory = GetComponent<InventorySystem>();
	}
	
	private void Start() 
	{
		GameObject container = GameObject.Find("ChampContainer");
		if (container != null)
		{
			things = container.GetComponent<ChampContainer>();
		}
		
		if (myChar.IsPlayer)
		{
			InitializePlayerUI();
		}
		
		StartCoroutine(SetColors());
	}
	
	private void InitializePlayerUI()
	{
		BarraDeVidaHUD = FindUIElement<Image>("HealthBarHUD");
		CantidadDeVida = FindUIElement<Text>("TextHPHUD");
		BarraDeManaHUD = FindUIElement<Image>("ManaBarHUD");
		CantidadDeMana = FindUIElement<Text>("TextMPHUD");
		PortraitSpace = FindUIElement<Image>("portrait");
		countDown = FindUIElement<Text>("Death_CountDown");
		Armor = FindUIElement<Text>("armor");
		Damage = FindUIElement<Text>("damage");
		Level = FindUIElement<Text>("level");
		SkilsPoints = FindUIElement<Text>("SkillPointInd");
		experiencia = FindUIElement<Image>("XPbar");
		ChampName = FindUIElement<Text>("ChampName");
		Oro = FindUIElement<Text>("Gold");
		
		BarraDeVidaTarget = FindUIElement<Image>("HealthBarTarget");
		CantidadDeVidaTarget = FindUIElement<Text>("TextHPTarget");
		BarraDeManaTarget = FindUIElement<Image>("ManaBarTarget");
		CantidadDeManaTarget = FindUIElement<Text>("TextMPTarget");
		ArmorTarget = FindUIElement<Text>("armorTarget");
		DamageTarget = FindUIElement<Text>("damageTarget");
		PortraitTarget = FindUIElement<Image>("portraitTarget");
		UITarget = GameObject.Find("Target");
		Death = GameObject.Find("Death");
		muerto = GameObject.Find("deathFX");
		LevelUpback = GameObject.Find("LVLUPB");
		
		// === INICIALIZAR INVENTARIO DEL JUGADOR ===
		playerItemSlots = new Image[6];
		playerItemNames = new Text[6];
		for (int i = 0; i < 6; i++)
		{
			playerItemSlots[i] = FindUIElement<Image>("PlayerItemSlot" + i);
			playerItemNames[i] = FindUIElement<Text>("PlayerItemName" + i);
			
			// Configurar EventTrigger para tooltip
			if (playerItemSlots[i] != null)
			{
				SetupItemSlotEvents(playerItemSlots[i].gameObject, i, true);
			}
		}
		
		// Stats adicionales del jugador
		playerAbilityPowerText = FindUIElement<Text>("playerAbilityPower");
		playerAttackSpeedText = FindUIElement<Text>("playerAttackSpeed");
		playerMovementSpeedText = FindUIElement<Text>("playerMovementSpeed");
		playerLifeStealText = FindUIElement<Text>("playerLifeSteal");
		playerCritChanceText = FindUIElement<Text>("playerCritChance");
		playerMagicResistText = FindUIElement<Text>("playerMagicResist");
		
		// === INICIALIZAR INVENTARIO DEL TARGET ===
		targetItemSlots = new Image[6];
		targetItemNames = new Text[6];
		for (int i = 0; i < 6; i++)
		{
			targetItemSlots[i] = FindUIElement<Image>("TargetItemSlot" + i);
			targetItemNames[i] = FindUIElement<Text>("TargetItemName" + i);
			
			// Configurar EventTrigger para tooltip
			if (targetItemSlots[i] != null)
			{
				SetupItemSlotEvents(targetItemSlots[i].gameObject, i, false);
			}
		}
		
		// Info del target
		targetGoldText = FindUIElement<Text>("targetGold");
		targetLevelText = FindUIElement<Text>("targetLevel");
		targetKDAText = FindUIElement<Text>("targetKDA");
		
		// Stats adicionales del target
		targetAbilityPowerText = FindUIElement<Text>("targetAbilityPower");
		targetAttackSpeedText = FindUIElement<Text>("targetAttackSpeed");
		targetMovementSpeedText = FindUIElement<Text>("targetMovementSpeed");
		targetLifeStealText = FindUIElement<Text>("targetLifeSteal");
		targetCritChanceText = FindUIElement<Text>("targetCritChance");
		targetMagicResistText = FindUIElement<Text>("targetMagicResist");
		
		// === TOOLTIP ===
		tooltipPanel = GameObject.Find("ItemTooltipPanel");
		tooltipNameText = FindUIElement<Text>("tooltipItemName");
		tooltipDescText = FindUIElement<Text>("tooltipItemDesc");
		tooltipStatsText = FindUIElement<Text>("tooltipItemStats");
		
		// Sprite para slots vacios
		GameObject emptySlotObj = GameObject.Find("EmptySlotSprite");
		if (emptySlotObj != null)
		{
			Image emptyImg = emptySlotObj.GetComponent<Image>();
			if (emptyImg != null)
			{
				emptySlotSprite = emptyImg.sprite;
			}
		}
		
		if (PortraitSpace != null && Portrait != null)
		{
			PortraitSpace.sprite = Portrait;
		}
		
		if (tooltipPanel != null)
		{
			tooltipPanel.SetActive(false);
		}
		
		isInitialized = true;
	}
	
	private T FindUIElement<T>(string objectName) where T : Component
	{
		GameObject obj = GameObject.Find(objectName);
		return obj != null ? obj.GetComponent<T>() : null;
	}
	
	private void SetupItemSlotEvents(GameObject slotObject, int slotIndex, bool isPlayerSlot)
	{
		if (slotObject == null) return;
		
		// Obtener o agregar EventTrigger
		EventTrigger trigger = slotObject.GetComponent<EventTrigger>();
		if (trigger == null)
		{
			trigger = slotObject.AddComponent<EventTrigger>();
		}
		
		// Limpiar eventos existentes
		trigger.triggers.Clear();
		
		// Crear entrada para PointerEnter
		EventTrigger.Entry enterEntry = new EventTrigger.Entry();
		enterEntry.eventID = EventTriggerType.PointerEnter;
		
		int index = slotIndex; // Capturar el valor para el closure
		bool isPlayer = isPlayerSlot;
		
		enterEntry.callback.AddListener((data) => {
			OnSlotPointerEnter(index, isPlayer);
		});
		trigger.triggers.Add(enterEntry);
		
		// Crear entrada para PointerExit
		EventTrigger.Entry exitEntry = new EventTrigger.Entry();
		exitEntry.eventID = EventTriggerType.PointerExit;
		exitEntry.callback.AddListener((data) => {
			OnSlotPointerExit();
		});
		trigger.triggers.Add(exitEntry);
	}
	
	private void OnSlotPointerEnter(int slotIndex, bool isPlayerSlot)
	{
		if (isPlayerSlot)
		{
			ShowPlayerItemTooltip(slotIndex);
		}
		else
		{
			ShowTargetItemTooltip(slotIndex);
		}
	}
	
	private void OnSlotPointerExit()
	{
		HideItemTooltip();
	}
	
	private void Update() 
	{
		UpdateHealthBar();
		
		if (myChar.IsPlayer)
		{
			UpdatePlayerUI();
			UpdatePlayerInventory();
			UpdatePlayerAdditionalStats();
			UpdateTargetUI();
			UpdateTargetInventory();
		}
		
		UpdateLevelText();
	}
	
	// === INVENTARIO DEL JUGADOR (siempre visible) ===
	private void UpdatePlayerInventory()
	{
		if (myInventory == null || playerItemSlots == null) return;
		
		List<ItemSlot> slots = myInventory.GetAllSlots();
		
		for (int i = 0; i < playerItemSlots.Length; i++)
		{
			if (i >= slots.Count)
			{
				SetEmptySlot(playerItemSlots, playerItemNames, i);
				continue;
			}
			
			UpdateSlotDisplay(playerItemSlots, playerItemNames, i, slots[i]);
		}
	}
	
	private void UpdatePlayerAdditionalStats()
	{
		if (playerAbilityPowerText != null)
		{
			playerAbilityPowerText.text = string.Format(STAT_FORMAT_COLOR, Mathf.RoundToInt(myChar.AbilityPower));
		}
		
		if (playerAttackSpeedText != null)
		{
			float totalAS = 1f + myChar.VelocidadDeAtaqueExtra;
			playerAttackSpeedText.text = string.Format(STAT_FORMAT_COLOR, totalAS.ToString("F2"));
		}
		
		if (playerMovementSpeedText != null)
		{
			playerMovementSpeedText.text = string.Format(STAT_FORMAT_COLOR, myChar.VelocidadDeMovimiento.ToString("F0"));
		}
		
		if (playerLifeStealText != null)
		{
			playerLifeStealText.text = string.Format(STAT_FORMAT_COLOR, Mathf.RoundToInt(myChar.LifeSteal) + "%");
		}
		
		if (playerCritChanceText != null)
		{
			float critChance = 0f;
			if (myInventory != null)
			{
				critChance = myInventory.GetTotalBonusStat("CritChance") * 100f;
			}
			playerCritChanceText.text = string.Format(STAT_FORMAT_COLOR, Mathf.RoundToInt(critChance) + "%");
		}
		
		if (playerMagicResistText != null)
		{
			playerMagicResistText.text = string.Format(STAT_FORMAT_COLOR, myChar.ResistenciaMagica.ToString("F1"));
		}
	}
	
	// === INVENTARIO DEL TARGET (solo cuando Target != null) ===
	private void UpdateTargetInventory()
	{
		// Si no hay target, no mostrar nada
		if (Target == null)
		{
			return;
		}
		
		Character targetChar = Target.GetComponent<Character>();
		if (targetChar == null || targetChar.Vida <= 0)
		{
			return;
		}
		
		targetInventory = Target.GetComponent<InventorySystem>();
		
		// Info básica del target
		if (targetGoldText != null && targetInventory != null)
		{
			targetGoldText.text = targetInventory.GetCurrentGold().ToString();
		}
		
		if (targetLevelText != null)
		{
			targetLevelText.text = targetChar.Nivel.ToString();
		}
		
		if (targetKDAText != null)
		{
			targetKDAText.text = string.Format("{0}/{1}/{2}", 
			                                   targetChar.Kills, targetChar.Deaths, targetChar.Assists);
		}
		
		// Stats adicionales del target
		if (targetAbilityPowerText != null)
		{
			targetAbilityPowerText.text = string.Format(STAT_FORMAT_COLOR, Mathf.RoundToInt(targetChar.AbilityPower));
		}
		
		if (targetAttackSpeedText != null)
		{
			float totalAS = 1f + targetChar.VelocidadDeAtaqueExtra;
			targetAttackSpeedText.text = string.Format(STAT_FORMAT_COLOR, totalAS.ToString("F2"));
		}
		
		if (targetMovementSpeedText != null)
		{
			targetMovementSpeedText.text = string.Format(STAT_FORMAT_COLOR, targetChar.VelocidadDeMovimiento.ToString("F0"));
		}
		
		if (targetLifeStealText != null)
		{
			targetLifeStealText.text = string.Format(STAT_FORMAT_COLOR, Mathf.RoundToInt(targetChar.LifeSteal) + "%");
		}
		
		if (targetCritChanceText != null)
		{
			float critChance = 0f;
			if (targetInventory != null)
			{
				critChance = targetInventory.GetTotalBonusStat("CritChance") * 100f;
			}
			targetCritChanceText.text = string.Format(STAT_FORMAT_COLOR, Mathf.RoundToInt(critChance) + "%");
		}
		
		if (targetMagicResistText != null)
		{
			targetMagicResistText.text = string.Format(STAT_FORMAT_COLOR, targetChar.ResistenciaMagica.ToString("F1"));
		}
		
		// Items del target
		if (targetInventory != null && targetItemSlots != null)
		{
			List<ItemSlot> slots = targetInventory.GetAllSlots();
			
			for (int i = 0; i < targetItemSlots.Length; i++)
			{
				if (i >= slots.Count)
				{
					SetEmptySlot(targetItemSlots, targetItemNames, i);
					continue;
				}
				
				UpdateSlotDisplay(targetItemSlots, targetItemNames, i, slots[i]);
			}
		}
	}
	
	// === HELPERS PARA SLOTS ===
	private void UpdateSlotDisplay(Image[] slotImages, Text[] slotNames, int index, ItemSlot slot)
	{
		if (slotImages == null || index >= slotImages.Length) return;
		
		if (slot.IsEmpty())
		{
			SetEmptySlot(slotImages, slotNames, index);
		}
		else
		{
			Item item = slot.item;
			
			if (slotImages[index] != null)
			{
				if (item.icon != null)
				{
					slotImages[index].sprite = item.icon;
				}
				else if (emptySlotSprite != null)
				{
					slotImages[index].sprite = emptySlotSprite;
				}
				slotImages[index].color = Color.white;
			}
			
			if (slotNames != null && index < slotNames.Length && slotNames[index] != null)
			{
				slotNames[index].text = item.itemName;
			}
		}
	}
	
	private void SetEmptySlot(Image[] slotImages, Text[] slotNames, int index)
	{
		if (slotImages == null || index >= slotImages.Length) return;
		
		if (slotImages[index] != null)
		{
			if (emptySlotSprite != null)
			{
				slotImages[index].sprite = emptySlotSprite;
			}
			slotImages[index].color = new Color(1f, 1f, 1f, 0.3f);
		}
		
		if (slotNames != null && index < slotNames.Length && slotNames[index] != null)
		{
			slotNames[index].text = "";
		}
	}
	
	// === TOOLTIP DE ITEMS ===
	public void ShowPlayerItemTooltip(int slotIndex)
	{
		ShowItemTooltip(myInventory, slotIndex);
	}
	
	public void ShowTargetItemTooltip(int slotIndex)
	{
		ShowItemTooltip(targetInventory, slotIndex);
	}
	
	private void ShowItemTooltip(InventorySystem inventory, int slotIndex)
	{
		if (inventory == null || tooltipPanel == null) return;
		
		List<ItemSlot> slots = inventory.GetAllSlots();
		
		if (slotIndex < 0 || slotIndex >= slots.Count) return;
		if (slots[slotIndex].IsEmpty()) return;
		
		Item item = slots[slotIndex].item;
		
		tooltipPanel.SetActive(true);
		
		if (tooltipNameText != null)
		{
			tooltipNameText.text = item.itemName;
		}
		
		if (tooltipDescText != null)
		{
			tooltipDescText.text = item.description;
		}
		
		if (tooltipStatsText != null)
		{
			tooltipStatsText.text = BuildItemStatsText(item);
		}
	}
	
	public void HideItemTooltip()
	{
		if (tooltipPanel != null)
		{
			tooltipPanel.SetActive(false);
		}
	}
	
	private string BuildItemStatsText(Item item)
	{
		System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
		if (item.bonusDamage > 0)
			sb.AppendLine(string.Format("<color=orange>+{0} Daño</color>", item.bonusDamage));
		
		if (item.bonusHealth > 0)
			sb.AppendLine(string.Format("<color=green>+{0} Vida</color>", item.bonusHealth));
		
		if (item.bonusMana > 0)
			sb.AppendLine(string.Format("<color=cyan>+{0} Mana</color>", item.bonusMana));
		
		if (item.bonusArmor > 0)
			sb.AppendLine(string.Format("<color=yellow>+{0} Armadura</color>", item.bonusArmor));
		
		if (item.bonusMagicResist > 0)
			sb.AppendLine(string.Format("<color=magenta>+{0} Resist. Magica</color>", item.bonusMagicResist));
		
		if (item.bonusAbilityPower > 0)
			sb.AppendLine(string.Format("<color=purple>+{0} Poder de Habilidad</color>", item.bonusAbilityPower));
		
		if (item.bonusAttackSpeed > 0)
			sb.AppendLine(string.Format("<color=yellow>+{0}% Vel. Ataque</color>", Mathf.RoundToInt(item.bonusAttackSpeed * 100)));
		
		if (item.bonusMovementSpeed > 0)
			sb.AppendLine(string.Format("<color=white>+{0} Vel. Movimiento</color>", item.bonusMovementSpeed));
		
		if (item.bonusLifeSteal > 0)
			sb.AppendLine(string.Format("<color=red>+{0}% Robo de Vida</color>", Mathf.RoundToInt(item.bonusLifeSteal * 100)));
		
		if (item.bonusCritChance > 0)
			sb.AppendLine(string.Format("<color=orange>+{0}% Prob. Critico</color>", Mathf.RoundToInt(item.bonusCritChance * 100)));
		
		if (item.bonusHealthRegen > 0)
			sb.AppendLine(string.Format("<color=green>+{0} Regen Vida</color>", item.bonusHealthRegen));
		
		if (item.bonusManaRegen > 0)
			sb.AppendLine(string.Format("<color=cyan>+{0} Regen Mana</color>", item.bonusManaRegen));
		
		sb.AppendLine("");
		sb.AppendLine(string.Format("<color=yellow>Costo: {0}g</color>", item.cost));
		
		return sb.ToString();
	}
	
	// === MÉTODOS ORIGINALES ===
	
	private void UpdateHealthBar()
	{
		if (myChar.Vida > 0) 
		{
			if (cam != null) cam.enabled = true;
			if (Death != null) Death.SetActive(false);
			if (muerto != null) muerto.SetActive(false);
			if (countDown != null) countDown.text = " ";
			if (BarraDeVida != null)
			{
				BarraDeVida.fillAmount = myChar.Vida / myChar.VidaMax;
			}
			
			if (MaterialPlane != null && (myChar.Isbot || myChar.IsCreep || myChar.IsJungle))
			{
				MaterialPlane.gameObject.SetActive(true);
			}
		}
		else
		{
			if (cam != null) cam.enabled = false;
			
			if (myChar.IsPlayer)
			{
				if (Death != null) Death.SetActive(true);
				if (muerto != null) muerto.SetActive(true);
				if (countDown != null)
				{
					countDown.text = myChar.TiempoDeRespawn.ToString("0");
				}
			}
			
			if (MaterialPlane != null && (myChar.Isbot || myChar.IsCreep || myChar.IsJungle))
			{
				MaterialPlane.gameObject.SetActive(false);
			}
		}
	}
	
	private void UpdatePlayerUI()
	{
		if (!isInitialized) return;
		
		if (BarraDeVidaHUD != null)
			BarraDeVidaHUD.fillAmount = myChar.Vida / myChar.VidaMax;
		
		if (BarraDeManaHUD != null)
			BarraDeManaHUD.fillAmount = myChar.Mana / myChar.ManaMax;
		
		if (experiencia != null)
			experiencia.fillAmount = myChar.Experiencia / myChar.ExperienciaMax;
		
		if (CantidadDeVida != null)
			CantidadDeVida.text = myChar.Vida.ToString("0") + " / " + myChar.VidaMax.ToString("0");
		
		if (CantidadDeMana != null)
			CantidadDeMana.text = myChar.Mana.ToString("0") + " / " + myChar.ManaMax.ToString("0");
		
		if (Armor != null)
			Armor.text = string.Format(ARMOR_FORMAT, myChar.Armadura, myChar.armaduraTemporal);
		
		if (Damage != null)
			Damage.text = string.Format(STAT_FORMAT_COLOR, myChar.Daño.ToString("F1"));
		
		if (Level != null)
			Level.text = myChar.Nivel.ToString();
		
		if (ChampName != null)
			ChampName.text = ChName;
		
		if (SkilsPoints != null)
			SkilsPoints.text = myChar.Skillponits.ToString();
		
		if (Oro != null && myInventory != null)
			Oro.text = myInventory.gold.ToString();
		
		if (LevelUpback != null)
		{
			LevelUpback.SetActive(myChar.Skillponits != 0 && myChar.Nivel < 18);
		}
	}
	
	private void UpdateTargetUI()
	{
		if (!isInitialized) return;
		
		if (Target == null)
		{
			if (UITarget != null) UITarget.SetActive(false);
			return;
		}
		
		Character targetChar = Target.GetComponent<Character>();
		if (targetChar == null || targetChar.Vida <= 0)
		{
			if (UITarget != null) UITarget.SetActive(false);
			return;
		}
		
		if (UITarget != null) UITarget.SetActive(true);
		
		UIchar targetUI = Target.GetComponent<UIchar>();
		if (PortraitTarget != null && targetUI != null && targetUI.Portrait != null)
		{
			PortraitTarget.sprite = targetUI.Portrait;
		}
		
		if (BarraDeVidaTarget != null)
			BarraDeVidaTarget.fillAmount = targetChar.Vida / targetChar.VidaMax;
		
		if (BarraDeManaTarget != null)
			BarraDeManaTarget.fillAmount = targetChar.Mana / targetChar.ManaMax;
		
		if (CantidadDeVidaTarget != null)
			CantidadDeVidaTarget.text = targetChar.Vida.ToString("0") + " / " + targetChar.VidaMax.ToString("0");
		
		if (CantidadDeManaTarget != null)
			CantidadDeManaTarget.text = targetChar.Mana.ToString("0") + " / " + targetChar.ManaMax.ToString("0");
		
		if (ArmorTarget != null)
			ArmorTarget.text = string.Format(STAT_FORMAT_COLOR, targetChar.Armadura.ToString("F1"));
		
		if (DamageTarget != null)
			DamageTarget.text = string.Format(STAT_FORMAT_COLOR, targetChar.Daño.ToString("F1"));
	}
	
	private void UpdateLevelText()
	{
		if (nivelactual != null)
		{
			nivelactual.text = myChar.Nivel.ToString();
		}
	}
	
	private IEnumerator SetColors()
	{
		yield return new WaitForSeconds(0.2f);
		
		if (things == null) yield break;
		
		bool isPlayerTeam = (things.team == 0 && gameObject.tag == "TeamBlue") ||
			(things.team == 1 && gameObject.tag == "TeamRed");
		
		if (gameObject.tag == "TeamRed" || gameObject.tag == "TeamBlue")
		{
			Color barColor = isPlayerTeam ? Color.green : Color.red;
			Material planeMaterial = isPlayerTeam ? blue : red;
			
			if (BarraDeVida != null)
				BarraDeVida.color = barColor;
			
			if (MaterialPlane != null)
				MaterialPlane.material = planeMaterial;
			
			if (myChar.Isbot && MarcoBarraDeVida != null)
				MarcoBarraDeVida.color = barColor;
		}
		else if (gameObject.tag == "Jungle")
		{
			if (BarraDeVida != null)
			{
				BarraDeVida.color = new Color(0.71f, 0f, 0.76f, 1f);
			}
		}
	}
}