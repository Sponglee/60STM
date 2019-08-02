using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour

{
    public bool X = false;
    public bool Y = false;
    public bool Z = false;


    public bool LookAtBool = true;

    public Quaternion initialRotation;

    private void Awake()
    {
        initialRotation = transform.rotation;    
    }
    // Update is called once per frame
    void Update()
    {
        if(LookAtBool)
        {
            transform.LookAt(Camera.main.transform.position);
            if (X)
            {
                transform.rotation = Quaternion.Euler(initialRotation.x, transform.rotation.y, transform.rotation.z);
            }
            if (Y)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, initialRotation.y, transform.rotation.z);
            }
            if (Z)
            {
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, initialRotation.z);
            }
        }
     
    }





    public GameManager gameManager;
    private void OnRectTransformDimensionsChange()
    {
        if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
        {
            gameManager.levelCam = gameManager.vertCam;
            gameManager.vertCam.Priority = 60;
            gameManager.horizCam.Priority = 40;
        }
        else if (Screen.orientation == ScreenOrientation.Landscape)
        {
            gameManager.levelCam = gameManager.horizCam;
            gameManager.vertCam.Priority = 40;
            gameManager.horizCam.Priority = 60;
        }
    }
}
