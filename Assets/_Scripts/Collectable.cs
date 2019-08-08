using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    Rigidbody collectableRb;

    public bool ChestCollectable = false;
    public float chestDuration = 1f;
    private float chestTimer = 0f;

    public Text powerText;

    [SerializeField]
    private int powerCol = -2;
    // -3 - Key PowerCol, -2 - gem PowerUp -1 - gem collectable
    public int PowerCol
    {
        get
        {
            return powerCol;
        }

        set
        {
            powerCol = value;
        }
    }

    private void Awake()
    {
        collectableRb = transform.GetComponent<Rigidbody>();

        
        //RandomizeCollectable();

        chestTimer = 0;
    }
    //private void Start()
    //{
    //    collectableRb = transform.GetComponent<Rigidbody>();

    //    RandomizeCollectable();

    //    chestTimer = 0;
    //    //if(ChestCollectable)
    //    //{
    //    //    transform.localPosition += new Vector3( Random.Range(-1f,1f),Random.Range(-1f,1f),0);
    //    //}
    //}

    private void Update()
    {
        //if(!ChestCollectable)
        //{
        //    transform.GetChild(0).Rotate(Vector3.forward, 3f);
        //}
        //else
        //{
        //    chestTimer += Time.deltaTime;
        //    if (chestTimer > chestDuration)
        //    {
        //        transform.parent.GetChild(1).gameObject.SetActive(false);
        //    }
        //}
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (PowerCol == -1 && other.CompareTag("Magnet"))
    //    {
    //        //transform.SetParent(SpawnManager.Instance.transform.GetChild(0));
    //        //StartCoroutine(MagnetCollectable());
    //    }
    //}



    private void OnEnable()
    {
        //if(PowerCol == -3)
        //{
        //    ////Debug.Log("ONENABLE KEY");
        //    //if (GameManager.Instance.KeyCount >= 3)
        //    //{
        //    //    //Debug.Log("DISABLE");
        //    //    transform.parent.gameObject.SetActive(false);
        //    //}
        //}
       
    }


    //private IEnumerator MagnetCollectable()
    //{
    //    //Transform ballTrans = BallController.Instance.transform;

    //    //collectableRb.constraints = RigidbodyConstraints.None;
    //    //while (true)
    //    //{
    //    //    Debug.DrawLine(collectableRb.transform.position, ballTrans.position, Color.blue);
    //    //    collectableRb.velocity = (ballTrans.position + new Vector3(0, Random.Range(0f,10f), 0f) - collectableRb.transform.position) * 5f;
    //    //    //Debug.Log(collectableRb.velocity);
    //    //    yield return null;
    //    //}
    //}

    public void RandomizeCollectable()
    {
        int PowerColRand = Random.Range(0, 100);

        ////Shield
        if (PowerColRand >= 0 && PowerColRand < 40)
        {
            powerText.text = "10";
        }
        //Magnet
        else if (PowerColRand >= 40 && PowerColRand < 60)
        {
            powerText.text = "100";
        }
        //Powered Up
        else if (PowerColRand >= 60 && PowerColRand <= 100)
        {
            powerText.text = "50";
        }


    }
}
