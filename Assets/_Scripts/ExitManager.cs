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
                GameObject tmpEmpty;


                //Despawn to empty if arcadde -to sides if not
                if (GameManager.Instance.ArcadeMode)
                {
                    tmpEmpty = Instantiate(LevelManager.Instance.emptyTilePref, transform.parent);
                }
                else
                {

                    tmpEmpty = Instantiate(LevelManager.Instance.sidesPref, transform.parent);
                    //Set material
                    tmpEmpty.transform.GetChild(1).GetComponent<Renderer>().material = LevelManager.Instance.tileMats[LevelManager.Instance.randTileMat];
                    tmpEmpty.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.gray;// new Color(0.2f, 0.2f, 0.2f);

                }


                LevelManager.Instance.freeTiles.Add(tmpEmpty.transform);
            
                LevelManager.Instance.exits.Remove(transform);

            }


            ////Check to spawn new Exits
            //if (GameManager.Instance.Timer > 0)
            //{
            //    GameManager.Instance.SpawnAlreadyTrigger = false;
            //}

            AudioManager.Instance.PlaySound("Despawn");
            //Excited emote
            FunctionHandler.Instance.MuskEmote(1);

            //if(!GameManager.Instance.ArcadeMode)
            //    FunctionHandler.Instance.LevelComplete();
            
            Destroy(gameObject);
        }
    }

}
