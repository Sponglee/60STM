using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FunctionHandler : Singleton<FunctionHandler>
{
    public Text menuText;


    public Transform muskReference;
    public Sprite[] muskImages;


    public void StartLevel()
    {
        SceneManager.LoadScene("Main");
    }




    public void GameOver()
    {
        menuText.gameObject.SetActive(true);
        menuText.text = "GAME OVER";
        GameManager.Instance.timerText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
        //Win sequence


       
    }

    public void LevelComplete()
    {
        menuText.gameObject.SetActive(true);
        menuText.text = "LEVEL COMPLETE";
        GameManager.Instance.timerText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
        //Win sequence

  
        //Win sequence
        //levelCam.gameObject.SetActive(true);
        //levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
        //levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
        GameManager.Instance.rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

    }


    public void MuskEmote(int index)
    {
        switch (index)
        {
            //Exiced
            case 0:
                {
                    StopAllCoroutines();
                    muskReference.GetComponentInChildren<Image>().sprite = muskImages[index];
                    StartCoroutine(StopMusk(index));
                }
                break;
            //Sad
            case 1:
                break;
            //Thumbs up
            case 2:
                break;
            //Pointy
            case 3:
                break;
            //Mars hi
            case 4:
                break;
            default:
                break;
        }
    }

    public IEnumerator StopMusk(int index)
    {
        
        yield return null;
    }

}
