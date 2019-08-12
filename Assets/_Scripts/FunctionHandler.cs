using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
using UnityEngine.EventSystems;


public class FunctionHandler : Singleton<FunctionHandler>
{
    public Text menuText;
    public GameObject endGamePanel;

    //Tutorial
    public GameObject tutorialCanvas;
    public int tutorialStep = 0;
    public string[] steps;
    public bool TutInProgress = false;

    public GameObject HoldTut;

    public Transform starsHolderUI;
    public Transform starsHolderEnd;

    public Transform muskReference;
    public Sprite[] muskImages;
    public bool MuskInProgress = false;


    public bool MenuActive = false;
    public GameObject menuCanvas;
    public GameObject uiCanvas;
    public GameObject buyCanvas;


    public CinemachineVirtualCamera horizCamOrth;
    public CinemachineVirtualCamera verticCamOrth;
    public CinemachineVirtualCamera horizCamPersp;
    public CinemachineVirtualCamera verticCamPersp;

    public Camera cameraOrth;
    public Camera cameraPersp;

    public bool PerspBool = false;



    public void ChangeCameras()
    {
        if(PerspBool)
        {
        
            cameraOrth.gameObject.SetActive(true);
            cameraPersp.gameObject.SetActive(false);
            GameManager.Instance.vertCam = verticCamOrth;
            GameManager.Instance.horizCam = horizCamOrth;

            verticCamPersp.gameObject.SetActive(false);
            horizCamPersp.gameObject.SetActive(false);
            verticCamOrth.gameObject.SetActive(true);
            horizCamOrth.gameObject.SetActive(true);
            PerspBool = !PerspBool;
        }
        else
        {
            cameraOrth.gameObject.SetActive(false);
            cameraPersp.gameObject.SetActive(true);
            GameManager.Instance.vertCam = verticCamPersp;
            GameManager.Instance.horizCam = horizCamPersp;
            verticCamOrth.gameObject.SetActive(false);
            horizCamOrth.gameObject.SetActive(false);
            verticCamPersp.gameObject.SetActive(true);
            horizCamPersp.gameObject.SetActive(true);
            PerspBool = !PerspBool;
        }
    }

    public void StartLevel(bool TimedToggle)
    {
        Time.timeScale = 1;
        //Debug modes
        if(TimedToggle || SceneManager.GetActiveScene().name == "Main")
        {
            SceneManager.LoadScene("Main");
        }
        else if(!TimedToggle || SceneManager.GetActiveScene().name == "Relax")
        {
            SceneManager.LoadScene("Relax");
        }
       
    }


    public void Exit()
    {
        Application.Quit();
    }

    public void QuitToTittle()
    {
        SceneManager.LoadScene("Tittle");
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
        Instance.MuskEmote(0);

        uiCanvas.SetActive(false);
        endGamePanel.SetActive(true);
  
        GameManager.Instance.turnCountText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
        //Win sequence


       
    }

    public void LevelComplete()
    {

        //Hi mote
        Instance.MuskEmote(2);
        
        GameManager.Instance.GameOverBool = true;
        AudioManager.Instance.StopSound("All");
        AudioManager.Instance.PlaySound("Win");
        AudioManager.Instance.PlaySound("FireWall");

        uiCanvas.SetActive(false);
        endGamePanel.SetActive(true);
        //Disable rays if no gems
        if(LevelManager.Instance.stars.Count== 0)
        {
            endGamePanel.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
        }

        menuText.gameObject.SetActive(true);
        menuText.text = string.Format("LEVEL {0} COMPLETE",PlayerPrefs.GetInt("Level",1));
        GameManager.Instance.turnCountText.gameObject.SetActive(false);
        //DIsable collider
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;

        //Win sequence
        UpdateStars(starsHolderEnd, 0.5f, "Lootbox");

        //Win sequence

        //Disable Tower
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(2).gameObject.SetActive(false);
        GameManager.Instance.rocketHolder.GetChild(1).GetChild(3).gameObject.SetActive(false);
        //Launch rocket
        GameManager.Instance.rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");

        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);


    }





    public void MuskEmote(int index)
    {
        //if (!MuskInProgress && !tutorialCanvas.activeSelf)
        //{
        //    switch (index)
        //    {
        //        //Sad
        //        case 0:
        //            {
        //                muskReference.GetComponentInChildren<Image>().sprite = muskImages[Random.Range(0,3)];

        //                StartCoroutine(StopMusk(index,true));
        //            }
        //            break;
        //        //Excited
        //        case 1:
        //            {
        //                int muskRandomizer = Random.Range(0, 100);
        //                if(muskRandomizer<50)
        //                {
        //                    muskReference.GetComponentInChildren<Image>().sprite = muskImages[Random.Range(3,7)];

        //                    StartCoroutine(StopMusk(index));
        //                }
                        

                      
        //            }
        //            break;
        //        //Launch
        //        case 2:
        //            {
        //                MuskInProgress = false;
        //                muskReference.GetComponentInChildren<Image>().sprite = muskImages[6];

        //                StartCoroutine(StopMusk(index));
        //            }
        //            break;
        //        //Mars hi
        //        case 3:
        //            {
        //                MuskInProgress = false;
        //                muskReference.GetComponentInChildren<Image>().sprite = muskImages[7];

        //                StartCoroutine(StopMusk(index,true));
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //}

    }

    public IEnumerator StopMusk(int index, bool stay = false)
    {
        MuskInProgress = true;
        Vector3 from = muskReference.GetChild(0).localPosition;

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


        muskReference.GetChild(0).localPosition = muskReference.GetChild(0).localPosition;   
        MuskInProgress = false;
    }



    //Credits
    public void OpenBrowser(string url)
    {
        Application.OpenURL(url);
    }


    //Mute sounds
    public bool musicMuted = false;
    public bool soundMuted = false;

    public void MuteSound(Transform reference)
    {
        
        if(reference.gameObject.name == "Music")
        {
            musicMuted = !musicMuted;
            Debug.Log("MMMMMMMMMMMUUUUUUUUUUUUUUSIC");
            AudioManager.Instance.VolumeMute(musicMuted,true);
            reference.GetChild(0).GetChild(0).gameObject.SetActive(!musicMuted);
            reference.GetChild(0).GetChild(1).gameObject.SetActive(musicMuted);
        }
        else
        {
            soundMuted = !soundMuted;
            AudioManager.Instance.VolumeMute(soundMuted);
            reference.GetChild(0).GetChild(0).gameObject.SetActive(!soundMuted);
            reference.GetChild(0).GetChild(1).gameObject.SetActive(soundMuted);
        }
       

    }



    public void TutorialStep()
    {
        if(!TutInProgress)
        {
            GameManager.Instance.GameOverBool = true;
            StartCoroutine(StopTutStep());
            tutorialCanvas.transform.parent.GetComponent<Animator>().SetTrigger("moveAnim");
        }
       
    }


    public IEnumerator StopTutStep()
    {
        TutInProgress = true;
        while(true)
        {
            if (tutorialStep > steps.Length - 1)
            {
                PlayerPrefs.SetInt("FirstLaunch", 0);
                tutorialCanvas.SetActive(false);
                TutInProgress = false;
                yield break;

            }

            GameManager.Instance.GameOverBool = false;
            //for (int i = 0; i < messages[tutorialStep].Length; i++)
            //{
            //    text.text += messages[tutorialStep][i];
            //    yield return new WaitForEndOfFrame();
            //}
            yield return new WaitForSeconds(2f);

            tutorialStep++;

        }

      
       
    }


    public void UpdateStars(Transform starsHolder, float duration = 0f, string Audio = "Star")
    {
        StartCoroutine(StopUpdateStars(starsHolder, duration, Audio));
    }


    public IEnumerator StopUpdateStars(Transform starsHolder, float duration = 0f, string Audio = "Star")
    {
        yield return new WaitForSeconds(duration*2);
        for (int i = 0; i < LevelManager.Instance.stars.Count; i++)
        {
            if(Audio != "Star")
            {
                AudioManager.Instance.PlaySound(Audio);
            }
            starsHolder.GetChild(i).GetComponent<Image>().color = Color.white;
            yield return new WaitForSeconds(duration);
        }

        if(Audio == "Star")
        {
            AudioManager.Instance.PlaySound("Star");
        }
    }


    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene("Tittle");
    }



    public void BuyTurns()
    {
        buyCanvas.SetActive(true);

    }

    public void ConfirmBuy(int cost)
    {
        if (GameManager.Instance.Currency >= cost)
        {
            GameManager.Instance.Currency -= cost;
            GameManager.Instance.TurnCount++;
        }
        else
        {
            AudioManager.Instance.PlaySound("Negative");
        }
    }

    public  void CancelBuy()
    {
        buyCanvas.SetActive(false);
    }


    // Is touching ui
    public bool IsPointerOverUIObject(string obj)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
            return results[0].gameObject.CompareTag(obj);
        else
            return false;
    }
}
