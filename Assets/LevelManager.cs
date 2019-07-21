using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject nodePref;
    public GameObject tilePref;
    public float nodeStep = 10.5f;
    
    public int levelDimention = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < levelDimention; i++)
        {
            for (int j = 0; j < levelDimention; j++)
            {
                GameObject tmpNode = Instantiate(nodePref, new Vector3(nodeStep*j-nodeStep * (levelDimention / 2), 0, -nodeStep * i + nodeStep*(levelDimention/2)), Quaternion.identity, transform);
                if(Random.Range(0,100)>30)
                {
                    Instantiate(tilePref,tmpNode.transform));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
