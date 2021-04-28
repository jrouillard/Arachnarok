using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public GameObject player;
    public MissionWaypoint missionWayPoint;

    public GameObject targetPrefab;
    public Canvas CanvasTarget;
    public Transform tutoTarget;
    // Start is called before the first frame update
    int currentPhase = 0;


    public GameObject[] mechPrefabs;
    public Text infos;
    public GameObject bigMechPrefab;
    public Transform[] spawns;
    public Transform[] mediumSpawns;

    public GameObject boss;

    public List<GameObject> aliveMechs = new List<GameObject>();
    private List<GameObject> bigMechs = new List<GameObject>();
    void Start()
    {
        GoToPhase(0);
    }

    public void GoToPhase(int phase) 
    { 
        missionWayPoint.ClearTargets();
        Debug.Log("Going phase " + phase);
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

    void StartTuto()
    {
        GameObject currentTarget = Instantiate(targetPrefab, tutoTarget.position, Quaternion.identity);
        Target target = currentTarget.GetComponent<Target>();
        target.SetTarget(tutoTarget);
        target.SetCanvas(CanvasTarget);
        missionWayPoint.AddTarget(target);
        currentPhase = 0;
    }
    void StartSimple() 
    {
        SpawnSimple();
        currentPhase = 1;
    }
    void SpawnSimple()
    {
        foreach (Transform spawn in spawns)
        {
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
        SpawnSimple();
        foreach (Transform spawn in mediumSpawns)
        {
            GameObject mech = Instantiate(bigMechPrefab, spawn.position, Quaternion.identity);
            bigMechs.Add(mech);
        }
        currentPhase = 2;
    }

    void StartBoss() 
    {
        SpawnSimple();
        foreach (Transform spawn in mediumSpawns)
        {
            GameObject mech = Instantiate(bigMechPrefab, spawn.position, Quaternion.identity);
            bigMechs.Add(mech);
        }
        currentPhase = 3;
    }
    void CheckAliveMech() 
    {
        foreach(GameObject mech in aliveMechs) 
        {   
            if (!mech.GetComponent<MechSettings>().IsAlive()) 
            {
                aliveMechs.Remove(mech);
            }
        }
    }
    void Update()
    {
        CheckAliveMech();
        switch (currentPhase)
        {
            case 0:
                break;
            case 1:
                if (aliveMechs.Count <= 3)
                {
                    GoToPhase(2);
                }
                break;
            case 2:            
                if (aliveMechs.Count <= 3 && bigMechs.Count == 0)
                {
                    GoToPhase(3);
                }
                break;
            case 3:    
                if (aliveMechs.Count <= 5 && boss == null)
                {
                    Win();
                }
                break;
            default:
                break;
        }
    }

    public void Win()
    {
        
    }
}
