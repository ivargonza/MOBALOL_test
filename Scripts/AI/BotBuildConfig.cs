using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BotBuildConfig : ScriptableObject
{
	[Header("Build Information")]
	public string buildName = "Default Build";
	public BuildType buildType;
	
	[Header("Item Purchase Order")]
	[Tooltip("IDs de items del ItemSystem en orden de compra")]
	public List<int> itemPurchaseOrder = new List<int>();
	
	[Header("Starter Items (comprar al inicio)")]
	public List<int> starterItemIDs = new List<int>();
	
	[Header("Skill Priority")]
	[Tooltip("Order: Q, W, E, R - Numbers represent priority (1-4)")]
	public int[] skillLevelUpPriority = new int[4] { 1, 2, 3, 4 };
	
	[Header("Playstyle Settings")]
	[Range(0f, 100f)]
	public float aggressiveness = 50f;
	
	[Range(0f, 100f)]
	public float farmPriority = 50f;
	
	[Range(10f, 90f)]
	public float healthRetreatThreshold = 30f;
	
	[Range(10f, 90f)]
	public float healthEngageThreshold = 60f;
	
	[Range(0f, 100f)]
	public float roamingTendency = 20f;
	
	public bool preferRangedTargets = false;
	public bool prioritizeLowHealthEnemies = true;
	public bool avoidTurrets = true;
	
	public enum BuildType
	{
		Tank,
		ADCarry,
		APCarry,
		Support,
		Fighter,
		Assassin
	}
	
	// Crear asset desde menu
	#if UNITY_EDITOR
	[UnityEditor.MenuItem("Assets/Create/AI/Bot Build Configuration")]
	public static void CreateAsset()
	{
		BotBuildConfig asset = ScriptableObject.CreateInstance<BotBuildConfig>();
		string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
		if (path == "")
		{
			path = "Assets";
		}
		else if (System.IO.Path.GetExtension(path) != "")
		{
			path = path.Replace(System.IO.Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
		}
		string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/NewBotBuild.asset");
		UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);
		UnityEditor.AssetDatabase.SaveAssets();
		UnityEditor.AssetDatabase.Refresh();
		UnityEditor.EditorUtility.FocusProjectWindow();
		UnityEditor.Selection.activeObject = asset;
	}
	#endif
}