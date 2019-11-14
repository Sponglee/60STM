﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.SceneManagement;
//using GameAnalyticsSDK;

public class GameManager : Singleton<GameManager>
{


    public bool BuildActive = false;

    public Text currencyText;

    public int currency = 0;
    public int Currency
    {
        get
        {
            return currency;
        }
        set
        {
            
            currency = value;
            currencyText.text = string.Format("{0}", value);

            PlayerPrefs.SetInt("Currency", value);

        }
    }
    public ScreenOrientation currentOrientation;

    
    public bool ArcadeMode = false;

    public Transform selectedTile;

    public NavMeshSurface surface;

    public Transform rocketHolder;
    public Transform previousRocket;
    public Transform nextRocket;

    public Transform debugTarget;

    public GameObject humanPref;

    public CinemachineVirtualCamera levelCam;
    
    //public CinemachineVirtualCamera endCam;
    //public CinemachineVirtualCamera vertCam;
    //public CinemachineVirtualCamera horizCam;
    //public CinemachineVirtualCamera zoomInCam;

  

    //Camera follow

    public List<Transform> zoomTargets;
    public int zoomThreshold = 20;
    //Total spawned people
    public int spawnCount = 1;
    //How many different spawns there is
    public int spawnModifier = 1;
    //Reference to count humans in rockets
    public int spawnHumanCount = 0;

   
    //Trigger To spawn
    public bool SpawnAlreadyTrigger = false;



  
    [SerializeField]

    private float timer = 0;
    public float Timer
    {
        get
        {
            return timer;
        }
        set
        {
            timer = value;
            timeText.text = value.ToString();
            if(ArcadeMode)
                progressSlider.value = (float)value / 60f;
        }
    }

    public bool GameOverBool = false;
    public GameObject WinText;

    public Text timeText;
    public Text humanText;
    public Text highScoreText;
    public Text LevelText;
    public Text NextLevelText;
    public Slider progressSlider;

    public int levelGoal;

    //Also Score for arcade
    [SerializeField]
    private int humanCount = 0;
    public int HumanCount
    {
        get
        {
            return humanCount;
        }
        set
        {
            humanCount = value;
            humanText.text = string.Format("{0}", value);

            //Remember the score
            if(ArcadeMode)
            {
                if(value> PlayerPrefs.GetInt("ArcadeScore",0))
                {
                    highScoreText.gameObject.SetActive(false);
                    PlayerPrefs.SetInt("ArcadeScore", value);
                }
                
            }
            else
                progressSlider.value = (float)value / (float)levelGoal;

        }
    }

    [SerializeField]
    private int highScore = 0;
    public int HighScore
    {
        get
        {
            return highScore;
        }
        set
        {
            highScore = value;
            highScoreText.text = string.Format("{0}", value);

        }
    }
    public int exitCount = 0;

    //Rocket capacity
    public int rocketCap = 100;
    public Slider rocketSlider;
    [SerializeField]
    private int rocketFilling = 0;
    public int RocketFilling
    {
        get
        {
            return rocketFilling;
        }
        set
        {
            rocketSlider = rocketHolder.transform.GetChild(1).GetChild(3).GetChild(0).GetComponent<Slider>();
            if (rocketFilling == 0 && value != 0)
            {
                rocketHolder.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
              
            }
            else if(rocketFilling >= 100 && value != 0)
            {
                rocketHolder.transform.GetChild(1).GetChild(3).gameObject.SetActive(false);
            }
            rocketFilling = value;
            //humanText.text = string.Format("{0}/{1}",value,levelGoal);
            rocketSlider.value = (float)value / (float)rocketCap;


        }
    }


 

    private void Awake()
    {
       
    }

    private void Start()
    {

       

        AudioManager.Instance.PlaySound("Music");
    }


    private void Update()
    {
       
    }

   

    public IEnumerator LevelSpawnerTimed()
    {
        List<int> randomExits = new List<int>();

        for (int i = 0; i < 3; i++)
        {

            randomExits.Add(Random.Range(0, LevelManager.Instance.exits.Count));
        }

        yield return new WaitForSeconds(2f);


        while (Timer > 0)
        {
            //Debug.Log(">>>>>>>>>>>" + levelGoal / LevelManager.Instance.freeTiles.Count);

            if (!SpawnAlreadyTrigger && !GameOverBool)
            {


                //Set reference for goal count
                spawnHumanCount = humanCount;
                //Set reference to spawnCOunt
                spawnCount = 0;

                for (int i = 0; i < Mathf.Clamp(spawnModifier, 0, 1); i++)
                {
                    Transform thisExit = LevelManager.Instance.SpawnExit();
                    for (int j = 0; j < 15f; j++)
                    {
                        GameObject tmpHuman = Instantiate(humanPref, thisExit.GetChild(4));
                        spawnCount += 1;

                        //Add an exit reference to human
                        tmpHuman.GetComponent<HumanController>().exitManager = thisExit.GetComponent<ExitManager>();
                        //Add a human reference
                        thisExit.GetComponent<ExitManager>().humansRef.Add(tmpHuman.transform);
                    }
                }

                BuildSurface();
                //foreach (Transform exit in LevelManager.Instance.exits)
                //{

                //    for (int i = 0; i < 15f; i++)
                //    {
                //       GameObject tmpHuman =  Instantiate(humanPref, exit.GetChild(2));
                //       spawnCount += 1;
                //        tmpHuman.GetComponent<NavMeshMover>().exitRef = thisExit; 
                //    }
                //}
                Debug.Log("SPAWN " + spawnModifier);
                SpawnAlreadyTrigger = true;

            }


            yield return new WaitForSeconds(1f);
            if (!GameOverBool)
            {
                Timer -= 1f;
                if (Timer <= 20 || (Timer <= 60 && Timer > 50))
                    AudioManager.Instance.PlaySound("TickTock");
            }



            if (Timer == 11)
            {
                AudioManager.Instance.PlaySound("Countdown");
                LevelManager.Instance.SpawnRocket();
                LevelManager.Instance.DisableRocket(nextRocket);
                //Build navMesh
                BuildSurface();
            }


            if (Timer % 10 == 0)
            {

                SpawnAlreadyTrigger = false;
            }
            //Debug.Log(humanCount + "- " + spawnHumanCount + " ( " + spawnCount + ")");





            //Check to spawn new Exits
            //if (humanCount - spawnHumanCount >= spawnCount && Timer >0)
            //{
            //    SpawnAlreadyTrigger = false;
            //    //LevelManager.Instance.DespawnExits();
            //}

            //if (HumanCount >= levelGoal)
            //{
            //    HumanCount = levelGoal;
            //    AudioManager.Instance.StopSound("All");
            //    AudioManager.Instance.PlaySound("FireWall");
            //    AudioManager.Instance.PlaySound("FlareUp");
            //    //Win sequence
            //    endCam.gameObject.SetActive(true);
            //    endCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
            //    endCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);

            //    FunctionHandler.Instance.LevelComplete();
            //    //LevelManager.Instance.DisableNextRocket(previousRocket);
            //    BuildSurface(true);
            //    GameOverBool = true;
            //    StopAllCoroutines();

            //    yield break;
            //}
        }

        //Launch emote
        FunctionHandler.Instance.MuskEmote(0);

        //Reset the timer and spawn new rocket
        Timer = 60f;
        spawnModifier++;

        //Disable Tower
        rocketHolder.GetChild(1).GetChild(2).gameObject.SetActive(false);
        //Launch rocket
        rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        rocketHolder.GetChild(1).GetChild(3).gameObject.SetActive(false);
        nextRocket.GetChild(1).GetComponent<Animator>().SetTrigger("Return");
        //Disable Tower
        nextRocket.GetChild(1).GetChild(2).gameObject.SetActive(false);
        nextRocket.GetChild(1).GetChild(1).gameObject.SetActive(true);


        //Play Sound
        AudioManager.Instance.PlaySound("FlareUp");
        AudioManager.Instance.PlaySound("FireWall");




        //BuildSurface(true);
        LevelManager.Instance.DisableRocket(rocketHolder);
        LevelManager.Instance.EnableRocket(nextRocket);
        BuildSurface();



        yield return new WaitForSeconds(10f);
        //Enable tower
        nextRocket.GetChild(1).GetChild(2).gameObject.SetActive(true);
        LevelManager.Instance.RandomizePrevious(rocketHolder);
        BuildSurface();
        //LevelManager.Instance.DeletePrevious(previousRocket);
        randomExits.Clear();
        StartCoroutine(LevelSpawnerTimed());


    }


  

    public IEnumerator LevelSpawner()
    {
        List<int> randomExits = new List<int>();

        //spawn rocket

        for (int i = 0; i < spawnModifier; i++)
        {

            randomExits.Add(Random.Range(0, LevelManager.Instance.exits.Count));
        }

        yield return new WaitForSeconds(2f);


        while (rocketFilling < rocketCap)
        {
            //Debug.Log(">>>>>>>>>>>" + levelGoal / LevelManager.Instance.freeTiles.Count);

            if (!SpawnAlreadyTrigger && !GameOverBool)
            {


                //Set reference for goal count
                spawnHumanCount = humanCount;
                //Set reference to spawnCOunt
                spawnCount = 0;

                for (int i = 0; i <spawnModifier; i++)
                {
                    Transform thisExit = LevelManager.Instance.SpawnExit();
                    yield return new WaitForSeconds(0.5f);
                    for (int j = 0; j < 15f; j++)
                    {
                        GameObject tmpHuman = Instantiate(humanPref, thisExit.GetChild(4));
                        spawnCount += 1;

                        //Add an exit reference to human
                        tmpHuman.GetComponent<HumanController>().exitManager = thisExit.GetComponent<ExitManager>();
                        //Add a human reference
                        thisExit.GetComponent<ExitManager>().humansRef.Add(tmpHuman.transform);
                    }
                }

                BuildSurface();
                //foreach (Transform exit in LevelManager.Instance.exits)
                //{

                //    for (int i = 0; i < 15f; i++)
                //    {
                //       GameObject tmpHuman =  Instantiate(humanPref, exit.GetChild(2));
                //       spawnCount += 1;
                //        tmpHuman.GetComponent<NavMeshMover>().exitRef = thisExit; 
                //    }
                //}
                Debug.Log("SPAWN " + spawnModifier);
                SpawnAlreadyTrigger = true;

            }


            yield return new WaitForSeconds(1f);
            //if (!GameOverBool)
            //{
            //    Timer -= 1f;
            //    if (Timer <= 20 || (Timer <= 60 && Timer > 56))
            //        AudioManager.Instance.PlaySound("TickTock");
            //}



            //if (RocketFilling >= rocketCap - 15 && nextRocket == null)
            //{
            //    ////AudioManager.Instance.PlaySound("Countdown");`
            //    //LevelManager.Instance.SpawnRocket();
            //    //LevelManager.Instance.DisableRocket(nextRocket);
            //    ////Build navMesh
            //    //BuildSurface();
            //}

            //////Debug.Log(RocketFilling);
            //if (LevelManager.Instance.exits.Count < 2 && RocketFilling != 0 && SpawnAlreadyTrigger)
            //{

            //    SpawnAlreadyTrigger = false;
            //}
            ////Debug.Log(humanCount + "- " + spawnHumanCount + " ( " + spawnCount + ")");





            ////Check to spawn new Exits
            //if (humanCount - spawnHumanCount >= spawnCount && Timer > 0)
            //{
            //    SpawnAlreadyTrigger = false;
            //    //LevelManager.Instance.DespawnExits();
            //}
            while(!GameOverBool)
            {
                if (HumanCount >= levelGoal)
                {
                    HumanCount = levelGoal;
                    AudioManager.Instance.StopSound("All");
                    AudioManager.Instance.PlaySound("FireWall");
                    AudioManager.Instance.PlaySound("FlareUp");

                    ////Win sequence
                    //endCam.gameObject.SetActive(true);
                    //endCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
                    //endCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);

                    FunctionHandler.Instance.LevelComplete();
                    //LevelManager.Instance.DisableNextRocket(previousRocket);
                    BuildSurface(true);
                    GameOverBool = true;
                    StopAllCoroutines();

                    yield break;
                }
                yield return new WaitForSeconds(1f);
            }
          
        }

        //Launch emote
        FunctionHandler.Instance.MuskEmote(0);

        //Reset the timer and spawn new rocket

        //spawnModifier++;

        ////Disable Tower
        //rocketHolder.GetChild(1).GetChild(2).gameObject.SetActive(false);
        ////Launch rocket
        //rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        //rocketHolder.GetChild(1).GetChild(3).gameObject.SetActive(false);
        //nextRocket.GetChild(1).GetComponent<Animator>().SetTrigger("Return");
        ////Disable Tower
        //nextRocket.GetChild(1).GetChild(2).gameObject.SetActive(false);
        //nextRocket.GetChild(1).GetChild(1).gameObject.SetActive(true);


        ////Play Sound
        //AudioManager.Instance.PlaySound("FlareUp");
        //AudioManager.Instance.PlaySound("FireWall");




        ////BuildSurface(true);
        //LevelManager.Instance.DisableRocket(rocketHolder);
        //LevelManager.Instance.EnableRocket(nextRocket);
        //BuildSurface();



        yield return new WaitForSeconds(10f);
        ////Enable tower
        //nextRocket.GetChild(1).GetChild(2).gameObject.SetActive(true);
        //LevelManager.Instance.RandomizePrevious(rocketHolder);
        //BuildSurface();
        ////LevelManager.Instance.DeletePrevious(previousRocket);
        //randomExits.Clear();
        //StartCoroutine(LevelSpawnerArcade());


    }

    //Rebake navmesh
    public void BuildSurface(bool EndGameBool = false, bool swear = false)
    {
        surface.BuildNavMesh();

        foreach (Transform node in LevelManager.Instance.transform)
        {
           
            if (node.childCount > 0)
            {
                if (node.GetChild(0).CompareTag("Tile") || node.GetChild(0).CompareTag("Exit"))
                {
                    if (swear)
                    {

                        Debug.Log(node.GetChild(0).name);
                        node.GetChild(0).GetComponent<TileManager>().AgentToggle(true, Vector3.zero, swear);

                    }
                    else if (EndGameBool)
                    {
                        //node.GetChild(0).GetComponent<TileManager>().AgentToggle(false, Vector3.zero, swear);
                    }
                    else if(node.GetChild(0).CompareTag("Tile"))
                    {
                        //Debug.Log("I AM HERE");
                        node.GetChild(0).GetComponent<TileManager>().AgentToggle(true, rocketHolder.position);
                    }
                    else if (node.GetChild(0).CompareTag("Exit"))
                    {
                        //Debug.Log("I AM HERE");
                        node.GetChild(0).GetComponent<ExitManager>().AgentToggle(true, rocketHolder.position);
                    }
                }

            }

            
           
        }
    }


  
}
