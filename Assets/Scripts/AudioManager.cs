using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource BackgroundSource;

    [Header("----------- Music -------------")]
    public AudioClip BackgroundMusic;
    public AudioClip winMusic;
    public AudioClip defeatMusic;

    [Header("----------- SFX ------------")]
    public AudioClip WalkSFX;
    public AudioClip runSFX;
    public AudioClip woodSFX;
    public AudioClip woodRunSFX;
    //public AudioClip winSFX;
    public AudioClip jumpScareSFX;

    private void Start()
    {
       /* BackgroundSource.clip = BackgroundMusic;
        BackgroundSource.Play(); */
    }
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.pitch = Random.Range(0.9f, 1.1f);
        SFXSource.PlayOneShot(clip);
    }

    public void PlayLoopSFX(AudioClip clip)
    {
        if (SFXSource.clip == clip && SFXSource.isPlaying)
            return;

        SFXSource.clip = clip;
        SFXSource.loop = true;
        SFXSource.pitch = Random.Range(0.95f, 1.05f);
        SFXSource.Play();
    }

    public void StopLoopSFX()
    {
        if (SFXSource.isPlaying)
        {
            SFXSource.Stop();
            SFXSource.clip = null;
        }
    }

    public void PlayBackgroundMusic(AudioClip clip)
    {
        BackgroundSource.clip = clip;
        BackgroundSource.Play();
    }
    
    public void StopBackgroundMusic()
    {
        BackgroundSource.Stop();
    }

    public void PlayWinMusic()
    {
        StopBackgroundMusic();
        if (winMusic != null)
        {
            BackgroundSource.clip = winMusic;
            BackgroundSource.loop = false; // only play once
            BackgroundSource.Play();
        }
        else
        {
            Debug.LogWarning("No winMusic assigned in AudioManager!");
        }
    }

    public void PlayDefeatMusic()
    {
        StopBackgroundMusic();

        if (defeatMusic != null)
        {
            BackgroundSource.clip = defeatMusic;
            BackgroundSource.loop = false;
            BackgroundSource.Play();
        }
        else
        {
            Debug.LogWarning("No defeatMusic assigned in AudioManager!");
        }
    }
}
