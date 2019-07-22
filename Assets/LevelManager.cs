using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject nodePref;
    public GameObject tilePref;
    public GameObject emptyTilePref;
    public GameObject exitPref;
    public Transform exitsHolder;
    public List<Transform> exits;
    public float nodeStep = 10.5f;
    
    public int levelDimention = 3;

    // Start is called before the first frame update
    void Start()
    {
        exits = new List<Transform>();


        for (int i = 0; i < levelDimention; i++)
        {
            for (int j = 0; j < levelDimention; j++)
            {
                GameObject tmpNode = Instantiate(nodePref, new Vector3(nodeStep*j-nodeStep * (levelDimention / 2), 0, -nodeStep * i + nodeStep*(levelDimention/2)), Quaternion.identity, transform);

                tmpNode.GetComponent<NodeController>().Row = i;
                tmpNode.GetComponent<NodeController>().Column = j;

                //If center - or proc - Generate tile
                if((i== levelDimention/2 && j == levelDimention/2) || Random.Range(0,100)>40)
                {
                    GameObject tmpTile = Instantiate(tilePref,tmpNode.transform.position,  Quaternion.Euler(0,Random.Range(0,360)/90*90,0), tmpNode.transform);


                    //If center - enable rocket and disable buildings
                    if(i == levelDimention/2 && j== levelDimention/2)
                    {
                        tmpTile.transform.GetChild(1).gameObject.SetActive(true);
                        tmpTile.transform.GetChild(2).gameObject.SetActive(false);
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

                    //Build navMesh
                    GameManager.Instance.BuildSurface();
                }
                else
                {
                    //Generate exit
                    if (exits.Count<=0 || Random.Range(0,100)<10)
                    {

                        GameObject tmpExit = Instantiate(exitPref, tmpNode.transform.position, Quaternion.Euler(0, Random.Range(0, 360) / 90 * 90, 0), tmpNode.transform);
                        exits.Add(tmpExit.transform);
                    }
                    //Or leave blank
                    else
                    {
                        GameObject tmpEmpty = Instantiate(emptyTilePref, tmpNode.transform.position, Quaternion.identity, tmpNode.transform);
                    }
                }


                
            }
        }

     
    }

}
