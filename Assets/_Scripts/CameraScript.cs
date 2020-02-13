using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public SpriteRenderer rink;
    public CinemachineVirtualCamera camRef;
    // Use this for initialization
    void Start()
    {
        camRef = GameManager.Instance.levelCam;

        //Set up zoom reference
        rink.transform.localScale = Vector3.one * (LevelManager.Instance.levelDimention + 1)* 10f;

        float screenRatio = (float)Screen.width / (float)Screen.height;
       
        if(screenRatio>1)
        {
            float targetRatio = rink.bounds.size.y / rink.bounds.size.x;
            Debug.Log("<>>>>>>>" + screenRatio + " <<<<<< " + targetRatio);
            if (screenRatio >= targetRatio)
            {
                camRef.m_Lens.OrthographicSize = rink.bounds.size.x / 2  -2f;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                camRef.m_Lens.OrthographicSize = rink.bounds.size.x / 2 * differenceInSize -2f;
            }
        }
        else
        {
            float targetRatio = rink.bounds.size.x / rink.bounds.size.y;
            Debug.Log("<>>>>>>>" + screenRatio + " <<<<<< " + targetRatio);
            if (screenRatio >= targetRatio)
            {
                camRef.m_Lens.OrthographicSize = rink.bounds.size.y / 2 - 2f;
            }
            else
            {
                float differenceInSize = targetRatio / screenRatio;
                camRef.m_Lens.OrthographicSize = rink.bounds.size.y / 2 * differenceInSize - 2f;
            }
        }


        
    }
}