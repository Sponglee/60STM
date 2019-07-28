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
    public bool MuskInProgress = false;


    public bool MenuActive = false;
    public GameObject menuCanvas;
    public GameObject uiCanvas;

    public void StartLevel()
    {
        SceneManager.LoadScene("Main");
    }


    public void Exit()
    {
        Application.Quit();
    }


    public void ToggleMenu()
    {
        if(menuCanvas.activeSelf == false)
        {
            MenuActive = true;
            Time.timeScale = 0;
        }
        else
        {
            MenuActive = false;
            Time.timeScale = 1;
        }


        menuCanvas.SetActive(!menuCanvas.activeSelf);
        uiCanvas.SetActive(!uiCanvas.activeSelf);
       
    }



    public void GameOver()
    {
        //Sad emote
        FunctionHandler.Instance.MuskEmote(1);


        menuText.gameObject.SetActive(true);
        menuText.text = "GAME OVER";
        GameManager.Instance.timerText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
        //Win sequence


       
    }

    public void LevelComplete()
    {

        //Hi mote
        FunctionHandler.Instance.MuskEmote(4);

        menuText.gameObject.SetActive(true);
        menuText.text = "LEVEL COMPLETE";
        GameManager.Instance.timerText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
        //Win sequence


        //Win sequence
       


        GameManager.Instance.rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

    }


    public void MuskEmote(int index)
    {
        if (!MuskInProgress)
        {
            switch (index)
            {
                //Excited
                case 0:
                    {
                        int muskRandomizer = Random.Range(0, 100);
                        if(muskRandomizer<50)
                        {
                            muskReference.GetComponentInChildren<Image>().sprite = muskImages[index];

                            StartCoroutine(StopMusk(index));
                        }
                        

                      
                    }
                    break;
                //Sad
                case 1:
                    {
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[index];

                        StartCoroutine(StopMusk(index));
                    }
                    break;
                //Thumbs
                case 2:
                    {
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[index];

                        StartCoroutine(StopMusk(index));
                    }
                    break;
                //Pointy
                case 3:
                    {
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[index];

                        StartCoroutine(StopMusk(index));
                    }
                    break;
                //Mars hi
                case 4:
                    {
                        MuskInProgress = false;
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[index];

                        StartCoroutine(StopMusk(index));
                    }
                    break;
                default:
                    break;
            }
        }

    }

    public IEnumerator StopMusk(int index)
    {
        MuskInProgress = true;
        Vector3 from = muskReference.GetChild(1).localPosition;

        float duration = 0.5f;
        //smooth lerp rotation loop
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            muskReference.GetChild(0).localPosition = Vector3.Lerp(from, Vector3.zero, elapsed / duration);
            elapsed += Time.fixedDeltaTime;

            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(1f,1.5f));

        elapsed = 0.0f;
        while (elapsed < duration)
        {
            muskReference.GetChild(0).localPosition = Vector3.Lerp(Vector3.zero, from, elapsed / duration);
            elapsed += Time.fixedDeltaTime;

            yield return null;
        }


        muskReference.GetChild(0).localPosition = muskReference.GetChild(1).localPosition;   
        MuskInProgress = false;
    }

}
