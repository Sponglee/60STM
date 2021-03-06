﻿
using UnityEngine;

//audio manager object class on scene load
[System.Serializable]
public class Sound
{
   

    public string name;
    public AudioClip clip;
    private AudioSource source;
    [Range(0f, 1f)]
    public float volume = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    public bool Looped = false;
    public bool Acceleration = false;

    //get all the clips to the 'pool'
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public float starMultiplier = 10f;
    public void Play(bool powUp = false, bool star = false)
    {
        source.volume = volume;
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2, randomPitch / 2));
       
        if (powUp)
        {
            if(star)
            {
                source.pitch = Mathf.Clamp(1 + starMultiplier, 0, 3f);
                
            }
            else
            {
                source.pitch = Mathf.Clamp(1 /*+ GameManager.Instance.Multiplier / 100f*/, 0, 30f);
            }

            //source.Play();
            source.Play();
        }
        else
            source.Play();
        //if(!source.isPlaying)
        //{
        //    source.Play();
        //}

    }
    public void Stop()
    {
        source.volume = volume;
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2, randomPitch / 2));

       
        source.Stop();
        //if(!source.isPlaying)
        //{
        //    source.Play();
        //}

    }
}




public class AudioManager : Singleton<AudioManager>
{

    [SerializeField]
    Sound[] sounds;



    void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        //if (FindObjectsOfType(GetType()).Length > 1)
        //{
        //    Destroy(gameObject);
        //}

        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            //set the source
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
            if (sounds[i].Looped)
                _go.GetComponent<AudioSource>().loop = true;
        }
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                if(sounds[i].name == "Lootbox")
                {
                    sounds[i].Play(false, true);

                }
                else
                    sounds[i].Play();

                return;
            }
        }
        //no sounds with that name
        Debug.Log("AudioManager: no sounds like that " + _name);
    }


    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {

            if(_name == "All")
            {
                sounds[i].Stop();
            }
            else if (sounds[i].name == _name)
            {
               
                 sounds[i].Stop();

                return;
            }
        }
        //no sounds with that name
        Debug.Log("AudioManager: no sounds like that " + _name);
    }

    public void VolumeChange(float value)
    {
        PlayerPrefs.SetFloat("Volume", value);
       
    }


    public void VolumeMute(bool on=false, bool musicToggle= false)
    {
        Debug.Log(">>" + on);
        
        if (on)
        {
            if (musicToggle)
            {
                foreach (Transform child in transform)
                {
                    if(child.name == "Sound_4_Music")
                    {
                        child.GetComponent<AudioSource>().mute = true;

                    }
                }
                PlayerPrefs.SetInt("VolumeMute", 1);
            }
            else
            {
                foreach (Transform child in transform)
                {
                    if(child.name != "Sound_4_Music")
                    {
                        Debug.Log("MUTE");
                        child.GetComponent<AudioSource>().mute = true;
                    }
                  
                }
                PlayerPrefs.SetInt("VolumeMute", 1);
            }
            
        }
        else
        {
            if (musicToggle)
            {
                foreach (Transform child in transform)
                {
                    if (child.name == "Sound_4_Music")
                    {
                        child.GetComponent<AudioSource>().mute = false;

                    }
                }
                PlayerPrefs.SetInt("VolumeMute", 1);
            }
            else
            {
                foreach (Transform child in transform)
                {

                    if (child.name != "Sound_4_Music")
                    {
                        Debug.Log("MUTE");
                        child.GetComponent<AudioSource>().mute = false;
                    }

                }
                PlayerPrefs.SetInt("VolumeMute", 1);
            }
        }
    }
}
