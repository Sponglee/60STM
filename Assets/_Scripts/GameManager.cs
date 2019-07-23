using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
   public Transform selectedTile;

    public NavMeshSurface surface;

    public Transform rocketHolder;

    public Transform debugTarget;

    public GameObject humanPref;



    public int spawnCount = 10;




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



    public Text timerText;
    public Text humanText;
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
            humanText.text = value.ToString();
        }
    }


    private void Start()
    {
        StartCoroutine(LevelSpawner());
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


    }

    public IEnumerator LevelSpawner()
    {
        int randomExit = 0;

        while(Timer>0)
        {
            randomExit = Random.Range(0, LevelManager.Instance.exits.Count);
            for (int i = 0; i < spawnCount; i++)
            {
                Instantiate(humanPref, LevelManager.Instance.exits[randomExit].GetChild(2));
            }

            //spawnCount += 1;
            yield return new WaitForSecondsRealtime(1f);
            Timer -= 1f;
        }

        Timer = 60f;
        rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        surface.enabled = false;
        Debug.Log("YOU WON");
    }

    //Rebake navmesh
    public void BuildSurface()
    {
        surface.BuildNavMesh();
    }



    

}
