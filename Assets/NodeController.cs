using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeController : MonoBehaviour
{


    [SerializeField]
    private int column;
    public int Column { get => column; set => column = value; }

    [SerializeField]
    private int row;
    public int Row { get => row; set => row = value; }
}
