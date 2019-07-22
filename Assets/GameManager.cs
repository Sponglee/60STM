using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
   public Transform selectedTile;




    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectedTile != null)
        {
            selectedTile.GetComponent<TileManager>().CollidedBool = false;
            selectedTile.GetComponent<TileManager>().Selected = false;
            selectedTile.GetComponent<TileManager>().DragActive = false;
            selectedTile = null;
        }
    }
}
