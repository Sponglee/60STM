using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private bool CollidedBool = false;
    [SerializeField]
    private bool Selected = false;


    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 oldPosition;

    void OnMouseDown()
    {
        Selected = true;
        oldPosition = transform.parent.position;

        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag()
    {
        if(!CollidedBool)
        {
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;
            transform.position = new Vector3(cursorPosition.x, transform.position.y, cursorPosition.z);
        }
      
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Empty") && !CollidedBool && Selected)
        {
            CollidedBool = true;
            Debug.Log("HERE");
            //Remember tmp parent and position
            Vector3 tmpPosition = transform.parent.position;
            Transform tmpParent = transform.parent;

            //Swap positions and parents
            transform.position = other.transform.parent.position;
            transform.SetParent(other.transform.parent);

            other.transform.position = tmpPosition;
            other.transform.SetParent(tmpParent);
        }
        else if(!CollidedBool && Selected)
        {
            Debug.Log("OLD " + other.name);
            CollidedBool = true;
            transform.position = oldPosition;
           
        }
    }


    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            CollidedBool = false;
            Selected = false;
        }
    }
}
