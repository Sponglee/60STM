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

    public Transform debugTarget;

    public GameObject humanPref;


    public CinemachineVirtualCamera levelCam;

    public int spawnCount = 1;




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


    public GameObject WinText;

    public Text timerText;
    public Text humanText;

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
        levelGoal = PlayerPrefs.GetInt("Level", 1)*50;
    }

    private void Start()
    {
        humanText.text = string.Format("{0}/{1}", HumanCount, levelGoal);
        //StartCoroutine(LevelSpawner());
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
                selectedTileManager.AgentToggle(true);
            
        }
        else if(Input.GetMouseButtonDown(1))
        {
            Instantiate(humanPref, debugTarget);
        }
        else if(Input.GetMouseButtonDown(2))
        {
            WinText.SetActive(true);
            timerText.gameObject.SetActive(false);
            //Win sequence
            levelCam.gameObject.SetActive(true);
            levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
            levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
            rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");

        }

    }

    public IEnumerator LevelSpawner()
    {
        List<int> randomExits = new List<int>();

        for (int i = 0; i < 3; i++)
        {

            randomExits.Add(Random.Range(0, LevelManager.Instance.exits.Count));
        }
        

        while (Timer>0)
        {
            //Debug.Log(">>>>>>>>>>>" + levelGoal / LevelManager.Instance.freeTiles.Count);
            
            if(spawnCount<levelGoal)
            {
                for (int i = 0; i <Random.Range(0,3f); i++)
                {
                    Instantiate(humanPref, LevelManager.Instance.exits[randomExits[Random.Range(0, randomExits.Count)]].GetChild(2));
                }
                    spawnCount += 1;
            }
           

           
            yield return new WaitForSecondsRealtime(1f);
            Timer -= 1f;

            if (HumanCount >= levelGoal)
            {
                WinText.SetActive(true);
                timerText.gameObject.SetActive(false);
                //DIsable collider
                rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
                //Win sequence

                WinText.SetActive(true);
                timerText.gameObject.SetActive(false);
                //Win sequence
                //levelCam.gameObject.SetActive(true);
                //levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
                //levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
                rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

                yield break;
            }
        }

       


        Timer = 60f;
        
        rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");

        LevelManager.Instance.SpawnRocket();
        LevelManager.Instance.DisablePrevious(previousRocket);
        BuildSurface();
        yield return new WaitForSeconds(2f);
      
        yield return new WaitForSeconds(5f);
        LevelManager.Instance.RandomizePrevious(previousRocket);
        //LevelManager.Instance.DeletePrevious(previousRocket);
        randomExits.Clear();
        StartCoroutine(LevelSpawner());
    }

    //Rebake navmesh
    public void BuildSurface()
    {
        surface.BuildNavMesh();
    }



    

}
