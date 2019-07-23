﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public GameObject nodePref;
    public GameObject tilePref;
    public GameObject emptyTilePref;
    public GameObject exitPref;
    public Transform exitsHolder;
    public List<Transform> exits;
    public List<Transform> freeTiles;

    public float nodeStep = 10.5f;
    
    public int levelDimention = 6;

    // Start is called before the first frame update
    void Start()
    {
        exits = new List<Transform>();
        freeTiles = new List<Transform>();

        for (int i = 0; i < levelDimention; i++)
        {
            for (int j = 0; j < levelDimention; j++)
            {
                GameObject tmpNode = Instantiate(nodePref, new Vector3(nodeStep*j-nodeStep * (levelDimention / 2), 0, -nodeStep * i + nodeStep*(levelDimention/2)), Quaternion.identity, transform);

                tmpNode.GetComponent<NodeController>().Row = i;
                tmpNode.GetComponent<NodeController>().Column = j;

                if (i == 0 ^ j == 0 ^ i == levelDimention - 1 ^ j == levelDimention - 1)
                {

                    GameObject tmpExit = Instantiate(exitPref, tmpNode.transform.position, Quaternion.identity, tmpNode.transform);
                    tmpExit.transform.LookAt(Vector3.zero,Vector3.up);
                    //Debug.Log(" EULER " + tmpExit.transform.eulerAngles.y);
                    tmpExit.transform.rotation = Quaternion.Euler(0, Mathf.Round(tmpExit.transform.eulerAngles.y / 90f) * 90f, 0);
                    exits.Add(tmpExit.transform);
                    
                }
                else if (i == 0 && j == 0 || i == levelDimention - 1 && j == levelDimention - 1 || i == 0 && j == levelDimention - 1 || j == 0 && i == levelDimention - 1)
                {
                    continue;
                }
                //If center - or proc - Generate tile
                else if ((i== levelDimention/2 && j == levelDimention/2) || Random.Range(0,100)>40)
                {
                    GameObject tmpTile = Instantiate(tilePref,tmpNode.transform.position,  Quaternion.Euler(0,Random.Range(0,360)/90*90,0), tmpNode.transform);

                   
                    //If center - enable rocket and disable buildings
                    if(i == levelDimention/2 && j== levelDimention/2)
                    {
                        tmpTile.transform.GetChild(1).gameObject.SetActive(true);
                        tmpTile.transform.GetChild(2).gameObject.SetActive(false);

                        //Set Rocket reference
                        GameManager.Instance.rocketHolder = tmpTile.transform;

                        //Enable road layout
                        tmpTile.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                        
                    }
                    else
                    {
                        //Generate building layout
                        int buildingIndex = Random.Range(0, tmpTile.transform.GetChild(2).childCount);
                        tmpTile.transform.GetChild(2).GetChild(buildingIndex).gameObject.SetActive(true);
                        //Generate road layout
                        int roadIndex = Random.Range(0, tmpTile.transform.GetChild(0).childCount);
                        tmpTile.transform.GetChild(0).GetChild(roadIndex).gameObject.SetActive(true);

                    }

                    
                }
                else
                {

                    //Add empty tile
                    GameObject tmpEmpty = Instantiate(emptyTilePref, tmpNode.transform.position, Quaternion.identity, tmpNode.transform);
                    freeTiles.Add(tmpEmpty.transform);


                    //Generate exit
                    if (exits.Count<=0 || Random.Range(0,100)<10)
                    {

                    }
                    //Or leave blank
                    else
                    {
                    }
                }


                
            }
        }

     
    }



    //Spawn new rocket
    public void SpawnRocket()
    {
        Transform tmpNode = freeTiles[Random.Range(0, freeTiles.Count)];

        GameObject tmpTile = Instantiate(tilePref, tmpNode.transform.position, Quaternion.Euler(0, Random.Range(0, 360) / 90 * 90, 0), tmpNode.transform);

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
        GameObject tmpEmpty = Instantiate(emptyTilePref, tmpTile.parent.transform.position, Quaternion.identity, tmpTile.parent.transform);
        freeTiles.Add(tmpEmpty.transform);

        Destroy(tmpTile.gameObject);

    }
}
