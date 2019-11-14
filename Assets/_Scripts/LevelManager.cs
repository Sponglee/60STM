using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public GameObject nodePref;
    public GameObject tilePref;
    public GameObject emptyTilePref;
    public GameObject sidesPref;
    public GameObject exitPref;
    public GameObject boardTilePref;
    public GameObject starPref;
    public GameObject fltTextPref;

    public List<Transform> stars;
    public List<Transform> exits;
    public List<Transform> freeTiles;

    public Transform boardHolder;
    public Transform exitsHolder;
    public Transform ground;
    public Transform trees;

    public Mesh[] treeVariants;
    public Material[] groundMats;

    public Material[] tileMats;
    public Material[] materials;

    //Randomize ground
    public int randGroundIndex;
    public int randTreeIndex;
    public int randTileMat;
    public int randGroundEuler;
    public int[] levelData;

    public float nodeStep = 10.5f;
    
    public int levelDimention = 25;
    public int levelDimentionCutOff = 5;
    public int tileCount = 0;

    public GameObject lastExit;

    // Start is called before the first frame update
    void Start()
    {
        exits = new List<Transform>();
        freeTiles = new List<Transform>();
        stars = new List<Transform>();
        levelData = new int[6];

        //LOAD SYSTEM
        string levelDataString = PlayerPrefs.GetString("LastLevel", "0,0,0,0,0,0");
        string[] levelDataStrArr = levelDataString.Split(',');
        for (int i = 0; i < levelDataStrArr.Length; i++)
        {
            //Debug.Log("SS");
            levelData[i] = int.Parse(levelDataStrArr[i]);
        }


        if (PlayerPrefs.GetInt("Level",1)!= levelData[0] || (GameManager.Instance != null && GameManager.Instance.ArcadeMode))
        {
           
            if (GameManager.Instance != null && GameManager.Instance.ArcadeMode)
            {
                Debug.Log("RANDOM");
                //Randomize ground
                randGroundIndex = Random.Range(0, groundMats.Length);
                randGroundEuler = Random.Range(0, 360);
                randTreeIndex = Random.Range(0, treeVariants.Length);
                randTileMat = Random.Range(0, tileMats.Length);
                levelDimentionCutOff = 8;
            }
            else
            {
                //Randomize ground
                randGroundIndex = Random.Range(0, groundMats.Length);
                randGroundEuler = Random.Range(0, 360);
                randTreeIndex = Random.Range(0, treeVariants.Length);
                randTileMat = Random.Range(0, tileMats.Length);
                levelDimentionCutOff = Random.Range(3, 5);
                PlayerPrefs.SetString("LastLevel", string.Format("{0},{1},{2},{3},{4},{5}", PlayerPrefs.GetInt("Level", 1), randGroundIndex, randGroundEuler, randTreeIndex, randTileMat, levelDimentionCutOff));
            }


           
        }
        else
        {
            randGroundIndex = levelData[1];
            randGroundEuler = levelData[2];
            randTreeIndex = levelData[3];
            randTileMat = levelData[4];
            levelDimentionCutOff = levelData[5];
        }

        ground.eulerAngles = new Vector3(0,randGroundEuler, 0);
        ground.GetChild(0).GetComponent<Renderer>().material = groundMats[randGroundIndex];

        //Disable trees on mars and moon
        if(randGroundIndex == 2 || randGroundIndex == 1)
        {
            trees.gameObject.SetActive(false);
        }
        //If desert - palms
        else if(randGroundIndex == 3)
        {
            foreach (Transform child in trees)
            {
                child.GetComponent<MeshFilter>().mesh = treeVariants[3];
            }
        }
        //If winter - trees
        else if (randGroundIndex == 4)
        {
            foreach (Transform child in trees)
            {
                child.GetComponent<MeshFilter>().mesh = treeVariants[0];
            }
        }
        else 
        {
            foreach (Transform child in trees)
            {
                child.GetComponent<MeshFilter>().mesh = treeVariants[randTreeIndex];
            }
        }
            trees.eulerAngles = new Vector3(0, randGroundEuler, 0);


        for (int i = 0; i < levelDimention; i++)
        {
            for (int j = 0; j < levelDimention; j++)
            {
                GameObject tmpNode = Instantiate(nodePref, new Vector3(nodeStep * j - nodeStep * (levelDimention / 2), 0, -nodeStep * i + nodeStep * (levelDimention / 2)), Quaternion.identity, transform);

                tmpNode.GetComponent<NodeController>().Row = i;
                tmpNode.GetComponent<NodeController>().Column = j;


              

            }
        }


        ////Randomize and populate stars
        //for (int i = 0; i < 3; i++)
        //{
        //    Transform tmpTile;

        //    do
        //    {
        //        tmpTile = transform.GetChild(Random.Range(0, transform.childCount));
        //        //Debug.Log("CHECK" + tmpTile.GetChild(0).tag);
        //    }
        //    while (stars.Contains(tmpTile) || tmpTile.childCount== 0 || !tmpTile.GetChild(0).CompareTag("Tile"));

        //    //Debug.Log(tmpTile.GetChild(0).tag);
        //    Instantiate(starPref, tmpTile.GetChild(0).GetChild(5));
        //    //Add star to list
        //    stars.Add(tmpTile);

            
        //}
        ////Clear stars list for levelcomplete stars system later
        //stars.Clear();
     
    }
    
    //Spawnpoint
    public Transform SpawnExit()
    {
        if (GameManager.Instance.ArcadeMode && Instance.stars.Count >= 3)
        {
            RespawnGem();
        }

        if (GameManager.Instance.ArcadeMode && freeTiles.Count == 0)
        {
            FunctionHandler.Instance.GameOver();
            return null;
        }
       else
        {
            int tmpFree = Random.Range(0, freeTiles.Count);

            //Debug.Log(">>>>" + tmpFree);
            Transform tmpFreeTile = freeTiles[tmpFree];
            freeTiles.Remove(tmpFreeTile);
            GameObject tmpExit = Instantiate(exitPref, tmpFreeTile.parent.position, Quaternion.Euler(0, Random.Range(0, 360) / 90 * 90, 0), tmpFreeTile.parent);

            //Set material
            tmpExit.transform.GetChild(1).GetComponent<Renderer>().material = tileMats[randTileMat];
            tmpExit.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.gray;// new Color(0.2f, 0.2f, 0.2f);

            tmpExit.transform.LookAt(Vector3.zero, Vector3.up);
            //Debug.Log(" EULER " + tmpExit.transform.eulerAngles.y);

            tmpExit.transform.rotation = Quaternion.Euler(0, Mathf.Round(tmpExit.transform.eulerAngles.y / 90f) * 90f, 0);

            tmpExit.transform.SetAsFirstSibling();
            Destroy(tmpFreeTile.gameObject);
            lastExit = tmpExit;
            AudioManager.Instance.PlaySound("Bump");
            exits.Add(tmpExit.transform);

            GameManager.Instance.BuildSurface();
            return tmpExit.transform;
        }


       

    }

    ////Despawn all exits
    //public void DespawnExits()
    //{
    //    foreach (Transform exit in exits)
    //    {
    //        GameObject tmpEmpty = Instantiate(emptyTilePref, exit.parent);
    //        freeTiles.Add(tmpEmpty.transform);
    //        Destroy(exit.gameObject);
    //    }
    //    exits.Clear();
    //}


    public void RespawnGem()
    {
        int randomIndex = Random.Range(0, LevelManager.Instance.transform.childCount);

        Transform tmpTile = LevelManager.Instance.transform.GetChild(randomIndex);

        if (tmpTile.childCount != 0 && tmpTile.GetChild(0).CompareTag("Tile"))
        {
            //Debug.Log(tmpTile.GetChild(0).tag);
            Instantiate(LevelManager.Instance.starPref, tmpTile.GetChild(0).GetChild(5));
            //Add star to list
            LevelManager.Instance.stars.Add(tmpTile);
            AudioManager.Instance.PlaySound("Star");
        }
    }


    //Spawn new rocket
    public void SpawnRocket()
    {

        //Set Rocket reference


        if (GameManager.Instance.ArcadeMode && freeTiles.Count == 0)
        {
            FunctionHandler.Instance.GameOver();
        }
        Transform tmpNode = freeTiles[Random.Range(0, freeTiles.Count)];

        GameObject tmpTile = Instantiate(tilePref, tmpNode.transform.parent.position, Quaternion.Euler(0, Random.Range(0, 360) / 90 * 90, 0), tmpNode.transform.parent);

        tmpTile.tag = "Rocket";
        if (GameManager.Instance.rocketHolder != null)
        {
            GameManager.Instance.nextRocket = tmpTile.transform;
            //Enable cube disable rocket
            tmpTile.transform.GetChild(1).gameObject.SetActive(true);
            tmpTile.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            tmpTile.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            GameManager.Instance.rocketHolder = tmpTile.transform;
            //Enable cube disable rocket
            tmpTile.transform.GetChild(1).gameObject.SetActive(true);
            tmpTile.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            tmpTile.transform.GetChild(2).gameObject.SetActive(true);
            tmpTile.transform.GetChild(1).GetComponent<Animator>().Play("IdleRocket");
        }


       
        //Disable moving
        tmpTile.GetComponent<TileManager>().Movable = false;

       

       
     
        //Enable road layout
        tmpTile.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);


        freeTiles.Remove(tmpNode);
        Destroy(tmpNode.gameObject);   
    }


    public void RandomizePrevious(Transform tmpTile)
    {
      
        //tmpTile.GetComponent<TileManager>().Movable = true;

        ////Enable roads/buildings
        //tmpTile.GetChild(0).gameObject.SetActive(true);
        //tmpTile.GetChild(2).gameObject.SetActive(true);
        //tmpTile.GetChild(3).gameObject.SetActive(true);
        //tmpTile.GetChild(4).gameObject.SetActive(true);

        ////Disable road layout
        //tmpTile.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        ////Disable rocket - enable roads and buildings
        //tmpTile.transform.GetChild(1).gameObject.SetActive(false);
        //tmpTile.transform.GetChild(2).gameObject.SetActive(true);
        ////Generate building layout
        //int buildingIndex = Random.Range(0, tmpTile.transform.GetChild(2).childCount);
        //tmpTile.transform.GetChild(2).GetChild(buildingIndex).gameObject.SetActive(true);
        ////Generate road layout
        //int roadIndex = Random.Range(0, tmpTile.transform.GetChild(0).childCount);
        //tmpTile.transform.GetChild(0).GetChild(roadIndex).gameObject.SetActive(true);

        GameManager.Instance.rocketHolder = GameManager.Instance.nextRocket;
        GameManager.Instance.nextRocket = null;

        DeletePrevious(tmpTile);
       
    }

    public void DisableRocket(Transform tmpTile)
    {

        tmpTile.GetChild(0).gameObject.SetActive(false);
        //tmpTile.GetChild(2).gameObject.SetActive(false);
        //tmpTile.GetChild(3).gameObject.SetActive(false);
        tmpTile.GetChild(4).gameObject.SetActive(false);

        GameManager.Instance.BuildSurface(false, true);
    }


    public void EnableRocket(Transform tmpTile)
    {
        tmpTile.GetChild(0).gameObject.SetActive(true);
        tmpTile.GetChild(4).gameObject.SetActive(true);
        GameManager.Instance.RocketFilling = 0;

    }
    public void DeletePrevious(Transform tmpTile)
    {

       
        //Add empty tile
        GameObject tmpEmpty = Instantiate(emptyTilePref, tmpTile.parent.transform.position, Quaternion.identity, tmpTile.parent.transform);
        freeTiles.Add(tmpEmpty.transform);

        Destroy(tmpTile.gameObject);

    }
}
