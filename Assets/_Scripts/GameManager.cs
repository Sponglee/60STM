using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public Transform rocket;
    public GameObject human;
    public Transform humanHolder;
    public Transform booths;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StopSpawn());
    }


    public IEnumerator StopSpawn()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(human, humanHolder.GetChild(Random.Range(0, humanHolder.childCount)));
        }
    }
    // Update is called once per frame
    void Update()
    {
      
    }
}
