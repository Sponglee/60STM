using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitManager : MonoBehaviour
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
            GameObject tmpEmpty = Instantiate(LevelManager.Instance.emptyTilePref, transform.parent);
            LevelManager.Instance.freeTiles.Add(tmpEmpty.transform);
            LevelManager.Instance.exits.Remove(transform);

            //Check to spawn new Exits
            if (GameManager.Instance.Timer > 0)
            {
                GameManager.Instance.SpawnAlreadyTrigger = false;
            }



            Destroy(gameObject);
        }
    }

}
