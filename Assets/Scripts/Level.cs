using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public GameObject player;
    public AudioSource audioSource;
    public AudioSource audioSourceCounter;
    public MissionWaypoint missionWayPoint;
    public Material skyred;
    public GameObject targetPrefab;
    public Canvas CanvasTarget;
    public Transform tutoTarget;
    // Start is called before the first frame update
    public int currentPhase = 0;


    public GameObject[] mechPrefabs;
    public Text infos;
    public GameObject mediumMechPrefab;
    public Transform[] spawns;
    public Transform[] mediumSpawns;
    public Transform bossSpawn;

    public GameObject bossPrefab;
    public GameObject mechboss;

    public List<GameObject> aliveMechs = new List<GameObject>();
    public List<GameObject> mediumMechs = new List<GameObject>();

    bool running = true;
    void Start()
    {
        GoToPhase(currentPhase);
    }

    int counter = 0;
    public void GoToPhase(int phase) 
    {
        running = false;
        if (audioSource && phase != 0)
        {
            audioSource.Play();
        }
        missionWayPoint.ClearTargets();
        
        ChangePhase(phase);
        
    } 

    void StartTuto()
    {
        currentPhase = 0;
        StartCoroutine(TutoCoroutine());
    }

    void ChangePhase(int phase)
    {
        switch (phase)
        {
            case 0:
                StartTuto();
                break;
            case 1:
                StartSimple();
                break;
            case 2:
                StartMedium();
                break;
            case 3:
                StartBoss();
                break;
            default:
                break;
        }
        
    }

    IEnumerator TutoCoroutine()
    {
        ShowText("Welcome to Arachnarok !");
        yield return new WaitForSeconds(5);
        ShowText("You have to protect the city");
        yield return new WaitForSeconds(3);
        ShowText("You can shoot your gun with left click");
        yield return new WaitForSeconds(3);
        ShowText("And you can shoot your grapple gun with right click");
        yield return new WaitForSeconds(3);
        ShowText("If you press space while grappling, you can move towards the hook, and swing faster");
        yield return new WaitForSeconds(3);
        ShowText("Also you do not take fall damage");
        
        yield return new WaitForSeconds(3);
        ShowText("Go to the top of this skyscrapper to start");
        GameObject currentTarget = Instantiate(targetPrefab, tutoTarget.position, Quaternion.identity);
        Target target = currentTarget.GetComponent<Target>();
        target.SetTarget(tutoTarget);
        target.SetCanvas(CanvasTarget);
        missionWayPoint.AddTarget(target);
        
        yield return new WaitForSeconds(3);
        HideText();
        running = true;
    }
    void ShowText(string text) 
    {
        infos.enabled = true;
        infos.text = text;
    } 
    void HideText() 
    {
        infos.enabled = false;
    } 
    void StartSimple() 
    {
        StartCoroutine(SimpleCoroutine());
    }
    
    IEnumerator SimpleCoroutine()
    {
        audioSourceCounter.pitch = 1;
        ShowText("Congrats!");
        yield return new WaitForSeconds(3);
        ShowText("Invasion in 3");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("Invasion in 2");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("Invasion in 1");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        audioSourceCounter.pitch = 2;
        audioSourceCounter.Play();
        SpawnSimple();
        currentPhase = 1;
        HideText();
        running = true;
    }
    void SpawnSimple()
    {
        foreach (Transform spawn in spawns)
        {
            if (Random.Range(-10.0f, 10.0f) > 3f) 
            {
                continue;
            }
            int rint = (int) Random.Range(0, mechPrefabs.Length);
            GameObject mechPrefab = mechPrefabs[rint];
            MechSettings mechPrefabSettings = mechPrefab.GetComponent<MechSettings>();
            GameObject mech = Instantiate(mechPrefab, spawn.position + mechPrefabSettings.offset, Quaternion.identity);
            MechSettings settings = mech.GetComponent<MechSettings>();
            settings.SetTarget(player.transform);
            aliveMechs.Add(mech);
        }
    }

    void StartMedium() 
    {
        StartCoroutine(MediumCoroutine());
    }
    IEnumerator MediumCoroutine()
    {    
        audioSourceCounter.pitch = 1;
        ShowText("Congrats!");
        yield return new WaitForSeconds(3);
        ShowText("To destroy the big mechas, aim for their heart");
        yield return new WaitForSeconds(3);
        ShowText("Floor is lava in 3");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("Floor is lava in 2");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("Floor is lava in 1");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        audioSourceCounter.pitch = 2;
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("(just kidding)");
        yield return new WaitForSeconds(1);
        HideText();
        SpawnSimple();
        // RenderSettings.skybox=skyred;
        MechSettings mediumMechPrefabSettings = mediumMechPrefab.GetComponent<MechSettings>();
        foreach (Transform spawn in mediumSpawns)
        {
            GameObject mech = Instantiate(mediumMechPrefab, spawn.position + mediumMechPrefabSettings.offset, Quaternion.identity);
            mediumMechs.Add(mech);
            MechSettings settings = mech.GetComponent<MechSettings>();
            settings.SetTarget(player.transform);
        }
        currentPhase = 2;
        running = true;
    }
    void StartBoss() 
    {
        
        StartCoroutine(BossCoroutine());
    }
    
    IEnumerator BossCoroutine()
    {    
        audioSourceCounter.pitch = 1;
        ShowText("Now for the big finale!");
        yield return new WaitForSeconds(3);
        ShowText("Broodmother in 3");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("Broodmother in 2");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        ShowText("Broodmother in 1");
        audioSourceCounter.Play();
        yield return new WaitForSeconds(1);
        HideText();
        audioSourceCounter.pitch = 2;
        audioSourceCounter.Play();

        SpawnSimple();
        mechboss = Instantiate(bossPrefab, bossSpawn.position, Quaternion.identity);
        MechSettings settings = mechboss.GetComponent<MechSettings>();
        settings.SetTarget(player.transform);
        currentPhase = 3;
        running = true;
    }
    void CheckAliveMech() 
    {
        for (int n = aliveMechs.Count - 1; n >= 0 ; n--) {
            if (!aliveMechs[n] || !aliveMechs[n].GetComponent<MechSettings>().IsAlive())
            {
                aliveMechs.RemoveAt(n);
            }
        }
        for (int n = mediumMechs.Count - 1; n >= 0 ; n--) {
            if (!mediumMechs[n] || !mediumMechs[n].GetComponent<MechSettings>().IsAlive())
            {
                mediumMechs.RemoveAt(n);
            }
        }
    }
    void Update()
    {
        if (Input.GetKey("escape"))
            SceneManager.LoadScene("menu");

        if (running) 
        {
            CheckAliveMech();
            switch (currentPhase)
            {
                case 0:
                    break;
                case 1:
                
                    if (aliveMechs.Count <= 3 && counter < 2)
                    {
                        SpawnSimple();
                        counter++;
                    } else if (aliveMechs.Count <= 3 && counter >= 2)
                    {
                        GoToPhase(2);
                    }
                    break;
                case 2:            
                    if (aliveMechs.Count <= 3 && mediumMechs.Count == 0)
                    {
                        GoToPhase(3);
                    }
                    break;
                case 3:    
                    if (!mechboss.GetComponent<MechSettings>().IsAlive())
                    {
                        Win();
                    }
                    break;
                default:
                    break;
            }
        }
        
    }

    public void Win()
    {
        running = false;
        StartCoroutine(WinCoroutine());
    }
    
    IEnumerator WinCoroutine()
    {    
        audioSourceCounter.pitch = 1;
        ShowText("The city is saved");
        yield return new WaitForSeconds(3);
        ShowText("yay");
        yield return new WaitForSeconds(1);
        ShowText("You can now leave");
        yield return new WaitForSeconds(3);
        HideText();
    }
}
