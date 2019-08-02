﻿using System.Collections;
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
    private bool movable = true;
    public bool Movable
    {
        get => movable;
        set
        {
            if(value == false && movable == true)
            {
                transform.GetChild(3).GetComponent<Renderer>().material = LevelManager.Instance.materials[1];
            }
            else if(value == true && movable == false)
            {
                transform.GetChild(3).GetComponent<Renderer>().material = LevelManager.Instance.materials[0];
            }
            movable = value;
        }
    }


    public float rotationCheck = 0;
    [SerializeField]
    private bool rotationInProgress = false;
    public bool RotationInProgress { get => rotationInProgress; set => rotationInProgress = value; }

    [SerializeField]
    private Animator tileAnim;

    private Vector3 screenPoint;
    private Vector3 offset;

    private Vector3 oldPosition;

    


    public void AgentToggle(bool enableToggle, Vector3 toggleDesto, bool swear = false)
    {
        //Debug.Log("MOVE " + transform.GetChild(4).childCount);
        //Disable agents
        foreach (Transform child in transform.GetChild(4))
        {
            child.GetComponent<NavMeshAgent>().enabled = enableToggle;
            if(swear)
            {
                StartCoroutine(child.GetComponent<HumanController>().StopShowMessage(":O",true));
                child.GetComponent<HumanController>().humanAnim.SetTrigger("Stop");
            }
            else if (enableToggle)
            {
                child.GetComponent<HumanController>().Move(toggleDesto);
                child.GetComponent<HumanController>().humanAnim.SetTrigger("Start");
            }
            else
            {
                child.position = child.parent.position + new Vector3(Random.Range(0,0.5f),0,Random.Range(0,0.5f));
                child.GetComponent<HumanController>().humanAnim.SetTrigger("Stop");
            }
        }
    }



    private void Update()
    {
        if(RotationInProgress)
        {
            rotationCheck += Time.deltaTime;
            if(rotationCheck>2f)
            {
                rotationCheck = 0;
                RotationInProgress = false;
            }
        }
    }


    void OnMouseDown()
    {
        if(/* Movable &&*/ !FunctionHandler.Instance.MenuActive)
        {
            Selected = true;
            GameManager.Instance.selectedTile = transform;


            oldPosition = transform.parent.position;

            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }
          


        //Disable agents for movement
        AgentToggle(false, GameManager.Instance.rocketHolder.position);


    }



    void OnMouseDrag()
    {
        if(transform.CompareTag("Tile") && Selected && !CollidedBool && Movable && !RotationInProgress)
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
        if (/*transform.CompareTag("Tile") &&*/ Selected && !CollidedBool && !RotationInProgress && !DragActive)
        {

            StartCoroutine(StopRotate());
        }
        else if(transform.CompareTag("Tile"))
            //Build navMesh
            //GameManager.Instance.BuildSurface();




        if(DragActive)
        {
            AudioManager.Instance.PlaySound("Bump");
        }
        //}

    }

    public IEnumerator StopRotate(float duration = 0.3f)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Round(transform.eulerAngles.y) / 90 * 90, transform.eulerAngles.z);
        //Debug.Log(transform.eulerAngles.y/90);
        transform.position = transform.parent.position;
        RotationInProgress = true;
        float angle = 90f;
        AudioManager.Instance.PlaySound("Swipe");
        float elapsed = 0;
        Vector3 startEul = transform.eulerAngles;
        Vector3 destEul = startEul + new Vector3(0, angle, 0);
        //Debug.Log("HH");
        while (elapsed < duration)
        {
            transform.eulerAngles = Vector3.Lerp(startEul, destEul, elapsed / duration);
            //Debug.Log(">R> " + transform.eulerAngles + " " + destEul.y % 360f);
            elapsed += Time.deltaTime;

            yield return null;
        }
        transform.eulerAngles = destEul;
        RotationInProgress = false;

        //Enable agents for movement
        AgentToggle(true, GameManager.Instance.rocketHolder.position);
        //Build navMesh
        GameManager.Instance.BuildSurface();
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
