using System;

using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;
//    public GameObject pcCamera;
  //  public GameObject vrCamera;

    bool listenerIsVR = true;

    // Use this for initialization
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        var battleMusic = GetComponent<AudioSource>();
        Play(battleMusic);
    }

    void Update()
    {
      /*  if (listenerIsVR)
        {
            pcCamera.GetComponent<AudioListener>().enabled = false;
            vrCamera.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            pcCamera.GetComponent<AudioListener>().enabled = true;
            vrCamera.GetComponent<AudioListener>().enabled = false;
        }

        listenerIsVR = !listenerIsVR;*/
    }

    public void Play(AudioSource soundName)
    {
        if (soundName == null)
        {
            return;
        }
        soundName.Play();
    }

    public void PlayOneShot(AudioSource audioSource,AudioClip soundClip)
    {
        if (audioSource == null)
        {
            return;
        }
        audioSource.PlayOneShot(soundClip, 1f);
    }

    public void Stop(AudioSource soundName)
    {
        if (soundName == null)
        {
            return;
        }
        soundName.Stop();
    }

    public bool isPlaying(AudioSource soundName)
    {
        if (soundName.isPlaying)
        {
            return true;
        }
        return false;
    }
}