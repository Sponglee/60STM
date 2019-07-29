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
        Time.timeScale = 1;
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
        AudioManager.Instance.StopSound("All");
        AudioManager.Instance.PlaySound("GameOver");
        //Sad emote
        FunctionHandler.Instance.MuskEmote(0);


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
        FunctionHandler.Instance.MuskEmote(3);

        menuText.gameObject.SetActive(true);
        menuText.text = "LEVEL COMPLETE";
        GameManager.Instance.timerText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
        //Win sequence


        //Win sequence

        //Disable Tower
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(2).gameObject.SetActive(false);
        //Launch rocket
        GameManager.Instance.rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

    }


    public void MuskEmote(int index)
    {
        if (!MuskInProgress)
        {
            switch (index)
            {
                //Sad
                case 0:
                    {
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[Random.Range(0,3)];

                        StartCoroutine(StopMusk(index,true));
                    }
                    break;
                //Excited
                case 1:
                    {
                        int muskRandomizer = Random.Range(0, 100);
                        if(muskRandomizer<50)
                        {
                            muskReference.GetComponentInChildren<Image>().sprite = muskImages[Random.Range(3,7)];

                            StartCoroutine(StopMusk(index));
                        }
                        

                      
                    }
                    break;
                //Launch
                case 2:
                    {
                        MuskInProgress = false;
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[6];

                        StartCoroutine(StopMusk(index));
                    }
                    break;
                //Mars hi
                case 3:
                    {
                        MuskInProgress = false;
                        muskReference.GetComponentInChildren<Image>().sprite = muskImages[7];

                        StartCoroutine(StopMusk(index,true));
                    }
                    break;
                default:
                    break;
            }
        }

    }

    public IEnumerator StopMusk(int index, bool stay = false)
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

       

        if(stay)
            yield return new WaitForSeconds(Random.Range(3f, 5f));
        else
            yield return new WaitForSeconds(Random.Range(1f, 1.5f));

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



    //Credits
    public void OpenBrowser(string url)
    {
        Application.OpenURL(url);
    }


    //Mute sounds
    public bool volumeMuted = false;
   

    public void MuteSound(Transform reference)
    {
        volumeMuted = !volumeMuted;
        if(reference.gameObject.name == "Music")
        {
            Debug.Log("MMMMMMMMMMMUUUUUUUUUUUUUUSIC");
            AudioManager.Instance.VolumeMute(volumeMuted,true);
        }
        else
        {
            AudioManager.Instance.VolumeMute(volumeMuted);
        }
        reference.GetChild(0).GetChild(0).gameObject.SetActive(!volumeMuted);
        reference.GetChild(0).GetChild(1).gameObject.SetActive(volumeMuted);

    }


}
