using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitManager : TileManager
{
   public List<Transform> humansRef;


    private void Awake()
    {
        humansRef = new List<Transform>();
    }

  
   
    //Trigger despawn check when human passes to rocket
    public void DespawnCheck()
    {
        if (humansRef.Count <=1)
        {
            if(transform.parent != null)
            {
                GameObject tmpEmpty = Instantiate(LevelManager.Instance.sidesPref, transform.parent);
                LevelManager.Instance.freeTiles.Add(tmpEmpty.transform);
            
                LevelManager.Instance.exits.Remove(transform);

            }


            ////Check to spawn new Exits
            //if (GameManager.Instance.Timer > 0)
            //{
            //    GameManager.Instance.SpawnAlreadyTrigger = false;
            //}

            AudioManager.Instance.PlaySound("Success");
            //Excited emote
            FunctionHandler.Instance.MuskEmote(1);

            //if(!GameManager.Instance.ArcadeMode)
            //    FunctionHandler.Instance.LevelComplete();
            
            Destroy(gameObject);
        }
    }

}
