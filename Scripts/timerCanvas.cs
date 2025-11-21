using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class timerCanvas : MonoBehaviour
{
    public float Cooldown;
    public float time;
    public float Tiempo;

    public Image IconSpace;
    // Use this for initialization
    void Start ()
    {
        time = Cooldown;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (time > 0)
        {
            float ammount = 1f - (1f / time);
            IconSpace.fillAmount = ammount;
           
            time -= Time.deltaTime;
        }
        if (time < 0)
        {
            IconSpace.fillAmount = 1f;
           
            time = 0;
        }
    }
}
