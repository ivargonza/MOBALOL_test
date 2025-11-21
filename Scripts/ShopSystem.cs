using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopSystem : MonoBehaviour
{
    public GameObject Inicio;
    public GameObject Defense;
    public GameObject Attack;
    public GameObject Magic;
    public GameObject Movement;
    public GameObject Consumables;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AbrirDefense()
    {
        Defense.SetActive(true);
        Inicio.SetActive(false);
    }
    public void AbrirAttack()
    {
        Attack.SetActive(true);
        Inicio.SetActive(false);
    }
    public void AbrirMagic()
    {
        Magic.SetActive(true);
        Inicio.SetActive(false);
    }
    public void AbrirMovement()
    {
        Movement.SetActive(true);
        Inicio.SetActive(false);
    }
    public void AbrirConsumable()
    {
        Consumables.SetActive(true);
        Inicio.SetActive(false);
    }

    public void CerrarDefense()
    {
        Defense.SetActive(false);
        Inicio.SetActive(true);
    }
    public void CerrarAttack()
    {
        Attack.SetActive(false);
        Inicio.SetActive(true);
    }

    public void CerrarMagic()
    {
        Magic.SetActive(false);
        Inicio.SetActive(true);
    }

    public void CerrarMovement()
    {
        Movement.SetActive(false);
        Inicio.SetActive(true);
    }
    public void CerrarConsumables()
    {
        Consumables.SetActive(false);
        Inicio.SetActive(true);
    }

}
