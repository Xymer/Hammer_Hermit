using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance;
    AudioSource SFX;
    AudioSource BGM;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
            var sources = GetComponents<AudioSource>();
            SFX = sources[0];
            BGM = sources[1];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<AudioClip> soundLibrary;
    public List<AudioClip> musicLibrary;
    [SerializeField, Range(0f,1f)] private float BGMVolume = .6f;

    public void PlaySound(string sound, float volumeScale = 1)
    {
        foreach (var playSound in soundLibrary)
        {
            if (playSound.name == sound)
            {
                SFX.PlayOneShot(playSound, volumeScale);
                break;
            }
        }
    }

    public void SetMusic(int musicIndex)
    {
        BGM.Stop();
        StopAllCoroutines();
        BGM.volume = BGMVolume;
        BGM.clip = musicLibrary[musicIndex];
        BGM.loop = true;
        BGM.Play();
    }

    public void FadeMusic()
    {
        StartCoroutine(FadeVolume());
    }

    private IEnumerator FadeVolume()
    {
        while (BGM.volume > 0)
        {
            BGM.volume -= 0.02f;
            yield return new WaitForFixedUpdate();
        }
        BGM.volume = 0;
        BGM.Stop();
    }
}
