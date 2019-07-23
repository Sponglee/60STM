using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameObject nodePref;
    public GameObject[] tilePrefs;
   
    public Transform exitsHolder;
    public List<Transform> exits;
    public List<Transform> emptyTiles;

    public float nodeStep = 10.5f;
    
    public int levelDimention = 6;


    [SerializeField]
    private Transform selectedEmptyTile;

    public Transform SelectedEmptyTile 
    {
        get
        {
            return selectedEmptyTile;
        }
        set
        {
            if(value != null && value != selectedEmptyTile)
            {
                value.GetComponent<EmptyTileController>().Toggle(true);
            }
            else if(selectedEmptyTile != null)
            {
                selectedEmptyTile.GetComponent<EmptyTileController>().Toggle(false);
            }
            selectedEmptyTile = value;
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        exits = new List<Transform>();
        emptyTiles = new List<Transform>();


        
       
        for (int i = 0; i < levelDimention; i++)
        {
            for (int j = 0; j < levelDimention; j++)
            {
                GameObject tmpNode = Instantiate(nodePref, new Vector3(nodeStep*j-nodeStep * (levelDimention / 2), 0, -nodeStep * i + nodeStep*(levelDimention/2)), Quaternion.identity, transform);

                tmpNode.GetComponent<NodeController>().Row = i;
                tmpNode.GetComponent<NodeController>().Column = j;

                //if (i == 0 ^ j == 0 ^ i == levelDimention - 1 ^ j == levelDimention - 1)
                //{



                //}
                //else if (i == 0 && j == 0 || i == levelDimention - 1 && j == levelDimention - 1 || i == 0 && j == levelDimention - 1 || j == 0 && i == levelDimention - 1)
                //{
                //    continue;
                //}
                //If center - or proc - Generate tile

                if ((i== levelDimention/2 && j == levelDimention/2))
                {
                    //Generate rocket
                    GameObject tmpTile = Instantiate(tilePrefs[0], tmpNode.transform.position, Quaternion.Euler(0, Random.Range(0, 360) / 90 * 90, 0), tmpNode.transform);
                   
                    
                }
                else
                {
                    //Generate exit
                    if (exits.Count <= 0 && Random.Range(0, 100) < 10)
                    {

                        GameObject tmpExit = Instantiate(tilePrefs[1], tmpNode.transform.position, Quaternion.identity, tmpNode.transform);
                        tmpExit.transform.LookAt(Vector3.zero, Vector3.up);
                        //Debug.Log(" EULER " + tmpExit.transform.eulerAngles.y);
                        tmpExit.transform.rotation = Quaternion.Euler(0, Mathf.Round(tmpExit.transform.eulerAngles.y / 90f) * 90f, 0);
                        exits.Add(tmpExit.transform);

                    }
                    else
                    {
                        //Add empty tile
                        GameObject tmpEmpty = Instantiate(tilePrefs[2], tmpNode.transform.position, Quaternion.identity, tmpNode.transform);
                        emptyTiles.Add(tmpEmpty.transform);
                    }
                    


                  

                }


                
            }
        }

        


    }

    public void BuildTile(int buildingIndex, Transform tmpNode)
    {
        //Generate rocket
        GameObject tmpTile = Instantiate(tilePrefs[buildingIndex], tmpNode.transform.parent.position + new Vector3(0,2f,0), Quaternion.identity, tmpNode.parent);
        //Generate building layout
        int buildingRandomizer= Random.Range(0, tmpTile.transform.GetChild(2).childCount);
        tmpTile.transform.GetChild(2).GetChild(buildingRandomizer).gameObject.SetActive(true);

        tmpTile.GetComponent<TileManager>().BuildRotation = true;
        // Disable ui enable button
        tmpNode.GetChild(1).GetChild(0).gameObject.SetActive(false);
        tmpNode.GetChild(1).GetChild(1).gameObject.SetActive(true);

        tmpNode.GetComponent<EmptyTileController>().TileTemplate = tmpTile.transform;


    }

    //Spawn new rocket
    public void SpawnRocket()
    {
        Transform tmpNode = emptyTiles[Random.Range(0, emptyTiles.Count)];

        GameObject tmpTile = Instantiate(tilePrefs[0], tmpNode.transform.position, Quaternion.Euler(0, Random.Range(0, 360) / 90 * 90, 0), tmpNode.transform);

        tmpTile.transform.GetChild(1).gameObject.SetActive(true);
        tmpTile.transform.GetChild(2).gameObject.SetActive(false);

        GameManager.Instance.previousRocket = GameManager.Instance.rocketHolder;
        //Set Rocket reference
        GameManager.Instance.rocketHolder = tmpTile.transform;

        //Enable road layout
        tmpTile.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
    }


    public void RandomizePrevious(Transform tmpTile)
    {
        //Enable roads/buildings
        tmpTile.GetChild(0).gameObject.SetActive(true);
        tmpTile.GetChild(2).gameObject.SetActive(true);
        tmpTile.GetChild(3).gameObject.SetActive(true);
        tmpTile.GetChild(4).gameObject.SetActive(true);

        //Disable road layout
        tmpTile.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        //Disable rocket - enable roads and buildings
        tmpTile.transform.GetChild(1).gameObject.SetActive(false);
        tmpTile.transform.GetChild(2).gameObject.SetActive(true);
        //Generate building layout
        int buildingIndex = Random.Range(0, tmpTile.transform.GetChild(2).childCount);
        tmpTile.transform.GetChild(2).GetChild(buildingIndex).gameObject.SetActive(true);
        //Generate road layout
        int roadIndex = Random.Range(0, tmpTile.transform.GetChild(0).childCount);
        tmpTile.transform.GetChild(0).GetChild(roadIndex).gameObject.SetActive(true);
    }

    public void DisablePrevious(Transform tmpTile)
    {
        tmpTile.GetChild(0).gameObject.SetActive(false);
        tmpTile.GetChild(2).gameObject.SetActive(false);
        tmpTile.GetChild(3).gameObject.SetActive(false);
        tmpTile.GetChild(4).gameObject.SetActive(false);
    }

    public void DeletePrevious(Transform tmpTile)
    {
        


        //Add empty tile
        GameObject tmpEmpty = Instantiate(tilePrefs[2], tmpTile.parent.transform.position, Quaternion.identity, tmpTile.parent.transform);
        emptyTiles.Add(tmpEmpty.transform);

        Destroy(tmpTile.gameObject);

    }
}
