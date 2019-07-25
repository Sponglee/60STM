using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private bool collidedBool = false;
    public bool CollidedBool { get => collidedBool; set => collidedBool = value; }
    [SerializeField]
    private bool selected = false;
    public bool Selected { get => selected; set => selected = value; }
    [SerializeField]
    private bool dragActive = false;
    public bool DragActive { get => dragActive; set => dragActive = value; }

    [SerializeField]
    private bool rotationInProgress = false;
    public bool RotationInProgress { get => rotationInProgress; set => rotationInProgress = value; }

    [SerializeField]
    private Animator tileAnim;

    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 oldPosition;

    


    public void AgentToggle(bool enableToggle)
    {
        //Disable agents
        foreach (Transform child in transform.GetChild(4))
        {
            child.GetComponent<NavMeshAgent>().enabled = enableToggle;
            if(enableToggle)
                child.GetComponent<NavMeshMover>().Move();
        }
    }

   



    void OnMouseDown()
    {
        
            Selected = true;
            GameManager.Instance.selectedTile = transform;


            oldPosition = transform.parent.position;

            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            //Disable agents for movement
            AgentToggle(false);
      
       
    }



    void OnMouseDrag()
    {
        if(Selected && !CollidedBool)
        {
            //if (transform.GetChild(4).childCount == 0)
            //{
                Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
                transform.position = new Vector3(cursorPosition.x, transform.position.y, cursorPosition.z);
            //}
        }
      
    }


    private void OnMouseUp()
    {
        //if (transform.GetChild(4).childCount == 0)
        //{
                                    //if (!CollidedBool && !RotationInProgress && !DragActive)
                                    //    StartCoroutine(StopRotate());
        //}
    }

    public IEnumerator StopRotate(float duration = 0.5f)
    {
        RotationInProgress = true;
        float angle = 90f;
       
        float elapsed = 0;
        Vector3 startEul = transform.eulerAngles;
        Vector3 destEul = startEul + new Vector3(0, angle, 0);
        //Debug.Log("HH");
        while (Mathf.Abs(transform.eulerAngles.y - destEul.y%360f) >= 0.05f)
        {
            transform.eulerAngles = Vector3.Lerp(startEul, destEul, elapsed / duration);
            //Debug.Log(">R> " + transform.eulerAngles + " " + destEul.y%360f);
            elapsed += Time.deltaTime;

            yield return null;
        }
        RotationInProgress = false;
        //Build navMesh
        GameManager.Instance.BuildSurface();

        //Enable agents for movement
        AgentToggle(true);
    }


  


    private void OnTriggerEnter(Collider other)
    {
        if(this.CompareTag("Tile") && other.CompareTag("Empty") && !CollidedBool && Selected)
        {
           
            DragActive = true;
            CollidedBool = true;
            //Debug.Log("HERE");
            //Remember tmp parent and position
            Vector3 tmpPosition = transform.parent.position;
            Transform tmpParent = transform.parent;

            //Swap positions and parents
            transform.position = other.transform.parent.position;
            transform.SetParent(other.transform.parent);

            other.transform.position = tmpPosition;
            other.transform.SetParent(tmpParent);
            //Build navMesh
            GameManager.Instance.BuildSurface();
        }
        else if(!CollidedBool && Selected)
        {

            
            DragActive = true;
            //Debug.Log("OLD " + other.name);
            CollidedBool = true;
            transform.position = oldPosition;
           
        }
    }


  
}
