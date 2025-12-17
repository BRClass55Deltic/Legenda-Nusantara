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
    public AudioClip jumpScareSFX;

    
     private void Start()
    {
        BackgroundSource.clip = BackgroundMusic;
        BackgroundSource.Play();
    }
    
    // =========================
    // LOOP SFX (FOOTSTEP)
    // =========================
    
    public void PlayLoopSFX(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) return;

        if (SFXSource.clip == clip && SFXSource.isPlaying)
            return;

        SFXSource.Stop();
        SFXSource.clip = clip;
        SFXSource.loop = true;
        SFXSource.pitch = pitch;
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
    // =========================
    // ONE SHOT (NON LOOP)
    // =========================
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        SFXSource.PlayOneShot(clip);
    }

    // =========================
    // MUSIC
    // =========================
    public void PlayBackgroundMusic(AudioClip clip)
    {
        if (clip == null) return;

        BackgroundSource.clip = clip;
        BackgroundSource.loop = true;
        BackgroundSource.Play();
    }

    public void StopBackgroundMusic()
    {
        BackgroundSource.Stop();
    }

    public void PlayWinMusic()
    {
        StopBackgroundMusic();
        BackgroundSource.clip = winMusic;
        BackgroundSource.loop = false;
        BackgroundSource.Play();
    }

    public void PlayDefeatMusic()
    {
        StopBackgroundMusic();
        BackgroundSource.clip = defeatMusic;
        BackgroundSource.loop = false;
        BackgroundSource.Play();
    }

    
}
