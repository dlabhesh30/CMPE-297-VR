using System;

using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
	public static AudioManager _instance;
	public Sound[] sounds; 
	// Use this for initialization
	void Awake () 
	{
		if (_instance == null) {
			_instance = this; 
		} 
		else 
		{
			Destroy (gameObject);
			return;
		}
		foreach (var sound in sounds) 
		{
			sound.audioSource = gameObject.AddComponent<AudioSource> ();
			sound.audioSource.clip = sound.clip;
			sound.audioSource.volume = sound.volume;
			sound.audioSource.pitch = sound.pitch;
			sound.audioSource.loop = sound.loop;
		}
	}

	void Start()
	{
		Play ("BattleMusic");
	}

	public void Play (string soundName)
	{
		Sound sound = Array.Find (sounds, s => s.name == soundName);
		if (sound == null) 
		{
			return;
		}
		sound.audioSource.Play();
	}

	public void Stop (string soundName)
	{
		Sound sound = Array.Find (sounds, s => s.name == soundName);
		if (sound == null) 
		{
			return;
		}
		sound.audioSource.Stop();
	}

	public bool isPlaying (string soundName)
	{
		Sound sound = Array.Find (sounds, s => s.name == soundName);
		if (sound.audioSource.isPlaying) 
		{
			return true;
		}
		return false;
	}
}
