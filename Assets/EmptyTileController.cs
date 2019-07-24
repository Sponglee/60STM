using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyTileController : MonoBehaviour
{

    [SerializeField]
    private bool emptyTileSelected = false;

    [SerializeField]
    private Transform tileTemplate;
    public Transform TileTemplate { get => tileTemplate; set => tileTemplate = value; }

    [SerializeField]
    private bool TileBuildingProgress = false;

    private void OnMouseOver()
    {
        if (LevelManager.Instance.SelectedEmptyTile == null)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            
        }
    }

    private void OnMouseExit()
    {
        if(LevelManager.Instance.SelectedEmptyTile == null)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            // Disable ui enable button
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        }
       
    }



    private void OnMouseDown()
    {
        if(!TileBuildingProgress && LevelManager.Instance.SelectedEmptyTile == null && LevelManager.Instance.SelectedEmptyTile != transform)
        {
            LevelManager.Instance.SelectedEmptyTile = transform;
        }
        else if(!TileBuildingProgress && LevelManager.Instance.SelectedEmptyTile != null && LevelManager.Instance.SelectedEmptyTile == transform)
        {
            Debug.Log("HERE");
            LevelManager.Instance.SelectedEmptyTile = null;
        }

      
    }



    public void Toggle(bool toggle)
    {

        transform.GetChild(0).gameObject.SetActive(toggle);
        // Disable ui enable button
        transform.GetChild(1).gameObject.SetActive(toggle);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(toggle);
        transform.GetChild(1).GetChild(1).gameObject.SetActive(!toggle);
    }




    public void BuildTile(int index)
    {
        LevelManager.Instance.BuildTile(index, transform);
        TileBuildingProgress = true;

    }


    public void ConfirmBuild()
    {
        TileBuildingProgress = false;
        if(tileTemplate != null)
        {
            tileTemplate.transform.position = transform.position;
            tileTemplate.GetComponent<TileManager>().BuildRotation = false;
            Destroy(gameObject);
        }
        //Build navMesh
        GameManager.Instance.BuildSurface();
    }

    public void CancelBuild()
    {
        TileBuildingProgress = false;
        if(tileTemplate != null)
        {
            Destroy(TileTemplate.gameObject);
            Toggle(true);
        }
        else
        {
            LevelManager.Instance.SelectedEmptyTile = null;
        }
    }

}
