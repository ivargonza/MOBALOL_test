using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class Visionward : MonoBehaviour
{
    public GameObject WardPrefab;
    public bool isavidated;
    public bool islaunched;
    public bool Player;
    public Vector3 pos;
    private cursor spell;
    private LayerMask Mapa;
    float CastTime;
    public ChampContainer Things;

    //Stats
   
    public float Cooldown = 60f;
    public float time;
    public float rango = 3;

    //ui
    public Image IconSpace;
    public Text numero;
    public Image NoMana;


    // Use this for initialization
    void Start()
    {
        if (Player)
        {
            IconSpace = GameObject.Find("Spell9").GetComponent<Image>();
            NoMana = GameObject.Find("NoMana9").GetComponent<Image>();
            numero = GameObject.Find("CoolDown9").GetComponent<Text>();
            Mapa = (1 << LayerMask.NameToLayer("Mapa"));
        }
        Things = GameObject.Find("ChampContainer").GetComponent<ChampContainer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player)
        {
            
            if (gameObject.GetComponent<Debuffs>().Stun
                || gameObject.GetComponent<Debuffs>().Silence
                || gameObject.GetComponent<Character>().Vida <= 0)
            {
                NoMana.enabled = true;
            }
            else
            {
                NoMana.enabled = false;
            }

            
            if (!gameObject.GetComponent<Debuffs>().Stun)
            {
                if (!gameObject.GetComponent<Debuffs>().Silence)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha4)
                        && gameObject.GetComponent<Character>().IsAlive
                        && time == 0)
                    {
                        isavidated = true;
                        spell = gameObject.GetComponent<cursor>();
                        spell.spellactive = true;
                    }
                }
            }
            if (isavidated)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000, Mapa))
                {
                    Vector3 hitpoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    pos = hit.point;
                }
                
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                if (isavidated || islaunched)
                {
                    isavidated = false;
                    islaunched = false;
                    spell = gameObject.GetComponent<cursor>();
                    spell.spellactive = false;
                    CastTime = 0;
                    gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                }

            }
            float D = Vector3.Distance(transform.position, pos);
            if (islaunched)
            {
                if (D <= rango)
                {
                    if (Things.URF)
                    {
                        time = Cooldown / 3f;
                    }
                    else
                    {
                        time = Cooldown;
                    }
                    lanzarhab();
                    CastTime = 0.3f;
                }
                if (D > rango)
                {
                    gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                    gameObject.GetComponent<ClickToMove>().Position = pos;

                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && isavidated && gameObject.GetComponent<Character>().IsAlive)
            {
                islaunched = true;
                isavidated = false;
                spell.spellactive = false;
            }
            if (CastTime > 0)
            {
                gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
                CastTime -= Time.deltaTime;
                gameObject.GetComponent<ClickToMove>().animator.SetBool("IsAttacking", false);
                gameObject.GetComponent<ClickToMove>().animator.SetInteger("State", 4);
            }
            if (CastTime < 0)
            {
                gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                CastTime = 0;
                gameObject.GetComponent<ClickToMove>().animator.SetBool("IsAttacking", false);
                gameObject.GetComponent<ClickToMove>().animator.SetInteger("State", 0);
            }
            if (time > 0)
            {
                IconSpace.fillAmount = 1f / time;
                if (time > 1)
                {
                    numero.text = time.ToString("0");
                }
                if (time < 1)
                {
                    numero.text = time.ToString("F1");
                }
                time -= Time.deltaTime;
            }
            if (time < 0)
            {
                IconSpace.fillAmount = 1f;
                numero.text = "";
                time = 0;
            }
        }
        
    }

    void lanzarhab()
    {
        Vector3 targetPostition = new Vector3(pos.x, pos.y, pos.z);
        transform.LookAt(targetPostition);
        islaunched = false;
        GameObject Ward = Instantiate(WardPrefab, targetPostition, Quaternion.identity) as GameObject;

    }


}