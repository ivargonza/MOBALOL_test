using UnityEngine;

public class MissilGenerico : MonoBehaviour
{
	public GameObject TargetMisil;
	public GameObject MyOwner;
	public float damage;
	public float speed = 20f;
	
	private bool hasHit;
	
	private void Update()
	{
		if (TargetMisil == null || hasHit)
		{
			Destroy(gameObject);
			return;
		}
		
		// Mover hacia el objetivo
		Vector3 direction = (TargetMisil.transform.position - transform.position).normalized;
		transform.position += direction * speed * Time.deltaTime;
		transform.LookAt(TargetMisil.transform);
		
		// Verificar distancia
		float distance = Vector3.Distance(transform.position, TargetMisil.transform.position);
		if (distance < 0.5f)
		{
			HitTarget();
		}
	}
	
	private void OnTriggerEnter(Collider other)
	{
		if (hasHit) return;
		
		if (other.gameObject == TargetMisil)
		{
			HitTarget();
		}
	}
	
	private void HitTarget()
	{
		if (hasHit) return;
		hasHit = true;
		
		Character targetChar = TargetMisil.GetComponent<Character>();
		if (targetChar != null)
		{
			// Calcular daño con armadura
			float totalArmor = targetChar.Armadura + targetChar.armaduraTemporal;
			float armorReduction = (totalArmor * 100f) / (totalArmor + 100f);
			float finalDamage = Mathf.Max(0, damage - armorReduction);
			
			// Registrar el daño para sistema de asistencias
			if (MyOwner != null)
			{
				targetChar.RegisterDamage(MyOwner, finalDamage);
			}
			
			// Revelar al objetivo si recibe daño
			Debuffs targetDebuffs = TargetMisil.GetComponent<Debuffs>();
			if (targetDebuffs != null)
			{
				targetDebuffs.OnDamageTaken();
			}
			
			// Aplicar daño
			targetChar.Vida -= finalDamage;
			
			// Si el dueño es campeón y mata al objetivo
			if (targetChar.Vida <= 0 && MyOwner != null)
			{
				Character ownerChar = MyOwner.GetComponent<Character>();
				if (ownerChar != null && (ownerChar.IsPlayer || ownerChar.Isbot))
				{
					ownerChar.GainKillExperience(targetChar);
				}
			}
			
			// Marcar jungle como atacado
			if (targetChar.IsJungle)
			{
				JungleAI jungleAI = targetChar.GetComponent<JungleAI>();
				if (jungleAI != null)
				{
					jungleAI.Isattacked = true;
				}
			}
		}
		
		Destroy(gameObject);
	}
}