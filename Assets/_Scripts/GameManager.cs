using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
   public Transform selectedTile;

    public NavMeshSurface surface;

    public Transform rocketHolder;
    public Transform previousRocket;
    public Transform nextRocket;

    public Transform debugTarget;

    public GameObject humanPref;


    public CinemachineVirtualCamera levelCam;

    //Total spawned people
    public int spawnCount = 1;
    //How many different spawns there is
    public int spawnModifier = 1;
    //Reference to count humans in rockets
    public int spawnHumanCount = 0;

    //Trigger To spawn
    public bool SpawnAlreadyTrigger = false;

    [SerializeField]
    private float timer = 60;
    public float Timer
    {
        get
        {
            return timer;
        }
        set
        {
            timer = value;
            timerText.text = value.ToString();
        }
    }

    public bool GameOverBool = false;
    public GameObject WinText;

    public Text timerText;
    public Text humanText;
    public Text LevelText;

    public int levelGoal;
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
            humanText.text = string.Format("{0}/{1}",value,levelGoal);
        }
    }

    private void Awake()
    {
        levelGoal = 100 + PlayerPrefs.GetInt("Level", 1)*15;
        LevelText.text = string.Format("LEVEL {0}:   ", PlayerPrefs.GetInt("Level", 1).ToString());
    }

    private void Start()
    {
        humanText.text = string.Format("{0}/{1}", HumanCount, levelGoal);
        StartCoroutine(LevelSpawner());


        AudioManager.Instance.PlaySound("Music");
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectedTile != null)
        {
            TileManager selectedTileManager = selectedTile.GetComponent<TileManager>();
            selectedTileManager.CollidedBool = false;
            selectedTileManager.GetComponent<TileManager>().Selected = false;
            selectedTileManager.GetComponent<TileManager>().DragActive = false;
            
            //Deselect transform
            selectedTile = null;
            //Enable Agent movement
            if(!selectedTileManager.RotationInProgress)
                selectedTileManager.AgentToggle(true, GameManager.Instance.rocketHolder.position);
            
        }
        else if(Input.GetMouseButtonDown(1))
        {
            //LevelManager.Instance.DespawnExits();
            spawnModifier++;
            LevelManager.Instance.SpawnExit();
        }
        else if(Input.GetMouseButtonDown(2))
        {
            WinText.SetActive(true);
            timerText.gameObject.SetActive(false);
            FunctionHandler.Instance.LevelComplete();

            //Win sequence
            levelCam.gameObject.SetActive(true);
            levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
            levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);


        }

    }

    public IEnumerator LevelSpawner()
    {
        List<int> randomExits = new List<int>();

        for (int i = 0; i < 3; i++)
        {

            randomExits.Add(Random.Range(0, LevelManager.Instance.exits.Count));
        }

        yield return new WaitForSeconds(2f);


        while (Timer>0)
        {
            //Debug.Log(">>>>>>>>>>>" + levelGoal / LevelManager.Instance.freeTiles.Count);
            
            if(!SpawnAlreadyTrigger && !GameOverBool)
            {
              
               
                //Set reference for goal count
                spawnHumanCount = humanCount;
                //Set reference to spawnCOunt
                spawnCount = 0;

                for (int i = 0; i < Mathf.Clamp(spawnModifier,0,1); i++)
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
            Timer -= 1f;
            AudioManager.Instance.PlaySound("TickTock");

            if (Timer == 11)
            {
                AudioManager.Instance.PlaySound("Countdown");
                LevelManager.Instance.SpawnRocket();
                LevelManager.Instance.DisableRocket(nextRocket);
                //Build navMesh
                GameManager.Instance.BuildSurface();
            }

           
            if ( Timer % 10 == 0)
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

            if (HumanCount >= levelGoal)
            {

                AudioManager.Instance.StopSound("All");
                AudioManager.Instance.PlaySound("FireWall");
                AudioManager.Instance.PlaySound("FlareUp");
                //Win sequence
                levelCam.gameObject.SetActive(true);
                levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
                levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
                
                FunctionHandler.Instance.LevelComplete();
                //LevelManager.Instance.DisableNextRocket(previousRocket);
                BuildSurface(true);
                GameOverBool = true;
                StopAllCoroutines();

                yield break;
            }
        }

        //Launch emote
        FunctionHandler.Instance.MuskEmote(2);

        //Reset the timer and spawn new rocket
        Timer = 60f;
        spawnModifier++;

        //Disable Tower
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(2).gameObject.SetActive(false);
        //Launch rocket
        rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        nextRocket.GetChild(1).GetComponent<Animator>().SetTrigger("Return");
        nextRocket.GetChild(1).GetChild(1).gameObject.SetActive(true);


        //Play Sound
        AudioManager.Instance.PlaySound("FlareUp");
        AudioManager.Instance.PlaySound("FireWall");




        //BuildSurface(true);
        LevelManager.Instance.DisableRocket(rocketHolder);
        LevelManager.Instance.EnableRocket(nextRocket);
        BuildSurface();



        yield return new WaitForSeconds(10f);
        LevelManager.Instance.RandomizePrevious(rocketHolder);
        BuildSurface();
        //LevelManager.Instance.DeletePrevious(previousRocket);
        randomExits.Clear();
        StartCoroutine(LevelSpawner());


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
                        node.GetChild(0).GetComponent<TileManager>().AgentToggle(false, Vector3.zero, swear);

                    }
                    else if (EndGameBool)
                    {
                        node.GetChild(0).GetComponent<TileManager>().AgentToggle(false, Vector3.zero, swear);
                    }
                    else if(node.GetChild(0).CompareTag("Tile"))
                    {
                        Debug.Log("I AM HERE");
                        node.GetChild(0).GetComponent<TileManager>().AgentToggle(true, rocketHolder.position);
                    }
                    else if (node.GetChild(0).CompareTag("Exit"))
                    {
                        Debug.Log("I AM HERE");
                        node.GetChild(0).GetComponent<ExitManager>().AgentToggle(true, rocketHolder.position);
                    }
                }

            }

            
           
        }
    }



    

}
