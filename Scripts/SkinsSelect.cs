using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkinsSelect : MonoBehaviour
{
    public ChampContainer Things;

    public GameObject[] Skins0;
    public GameObject[] Skins1;
    public GameObject[] Skins2;

    public Sprite[] PantallaDeCarga0;
    public Sprite[] PantallaDeCarga1;
    public Sprite[] PantallaDeCarga2;


    public Button Skin0butt;
    public Button Skin1butt;
    public Button Skin2butt;

    //public string[] NombreSkin;

    public int index { get; set; }
    public int ChampIndex;
    

    void Start()
    {
        Things = GameObject.Find("ChampContainer").GetComponent<ChampContainer>();

    }

    // Update is called once per frame
    void Update ()
    {
        
    }
    public void ChangeLoadScreen()
    {
        if (PantallaDeCarga0[ChampIndex] != null)
        {
            Skin0butt.image.sprite = PantallaDeCarga0[ChampIndex];
            Skin0butt.interactable = true;
        }
        else
        {
            Skin0butt.interactable = false;
            Skin0butt.image.sprite = PantallaDeCarga0[ChampIndex];
        }

        if (PantallaDeCarga1[ChampIndex] != null)
        {
            Skin1butt.image.sprite = PantallaDeCarga1[ChampIndex];
            Skin1butt.interactable = true;
        }
        else
        {
            Skin1butt.interactable = false;
            Skin1butt.image.sprite = PantallaDeCarga0[ChampIndex];
        }

        if (PantallaDeCarga2[ChampIndex] != null)
        {
            Skin2butt.image.sprite = PantallaDeCarga2[ChampIndex];
            Skin2butt.interactable = true;
        }

        else
        {
            Skin2butt.interactable = false;
            Skin2butt.image.sprite = PantallaDeCarga0[ChampIndex];
        }
    }


    public void SkinSelected()
    {
        if(index == 0)
        {
            Things.Champ = Skins0[ChampIndex];
            Things.LoadScreenSelected = PantallaDeCarga0[ChampIndex];
            Skins0[ChampIndex].GetComponent<UIchar>().Nombre.text = Things.Nombre;
            Skin0butt.interactable = false;
            Skin1butt.interactable = true;
            Skin2butt.interactable = true;
        }
        if (index == 1)
        {
            Things.Champ = Skins1[ChampIndex];
            Things.LoadScreenSelected = PantallaDeCarga1[ChampIndex];
            Skins1[ChampIndex].GetComponent<UIchar>().Nombre.text = Things.Nombre;
            Skin0butt.interactable = true;
            Skin1butt.interactable = false;
            Skin2butt.interactable = true;
        }
        if (index == 2)
        {
            Things.Champ = Skins2[ChampIndex];
            Things.LoadScreenSelected = PantallaDeCarga2[ChampIndex];
            Skins2[ChampIndex].GetComponent<UIchar>().Nombre.text = Things.Nombre;
            Skin0butt.interactable = true;
            Skin1butt.interactable = true;
            Skin2butt.interactable = false;
        }
    }
   

}

