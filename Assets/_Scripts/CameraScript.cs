﻿using Cinemachine;
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



        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = rink.bounds.size.x / rink.bounds.size.y;

        if (screenRatio >= targetRatio)
        {
            camRef.m_Lens.OrthographicSize = rink.bounds.size.y / 2;
        }
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            camRef.m_Lens.OrthographicSize = rink.bounds.size.y / 2 * differenceInSize;
        }
    }
}