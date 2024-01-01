using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : SingletonCreator<SoundController>
{
    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundEffectSource;

    [Header("Audio Clips References")]
    [SerializeField] private AudioClip BGMusic;
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip scoreTile;
    [SerializeField] private AudioClip wrongTille;

    [Header("Variables")]
    [SerializeField] [Range(0f,1.0f)] private float soundEffectsVolume;
    [SerializeField] [Range(0f, 1.0f)] private float BGMusicVolume;
    public bool isBGMusicMuted;
    public bool isSoundEffectsMuted;

    // Start is called before the first frame update
    void Start()
    {
        soundEffectSource.volume = soundEffectsVolume;
        musicSource.volume = BGMusicVolume;

        isBGMusicMuted = false;
        isSoundEffectsMuted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonClick()
    {
        PlaySoundEffect(buttonClick);
    }

    public void PlayScoredTile()
    {
        PlaySoundEffect(scoreTile);
    }

    public void PlayWrongTile()
    {
        PlaySoundEffect(wrongTille);
    }

    public void PlayBackgroundMusic()
    {
        PlayBGMusic(BGMusic);
    }

    public void MuteSoundEffects()
    {
        isSoundEffectsMuted = true;
        //call the function so the booleean value can come into effect
        PlayButtonClick();
    }
    public void UnmuteSoundEffects()
    {
        isSoundEffectsMuted = false;
        //call the function so the booleean value can come into effect
        PlayButtonClick();
    }

    public void MuteBGMusic()
    {
        isBGMusicMuted = true;
        //call the function so the booleean value can come into effect
        PlayBackgroundMusic();
    }
    public void UnmuteBGMusic()
    {
        isBGMusicMuted = false;
        //call the function so the booleean value can come into effect
        PlayBackgroundMusic();
    }

    private void PlaySoundEffect(AudioClip seClip)
    {
        if(isSoundEffectsMuted == true)
        {
            //if (soundEffectSource.isPlaying)
            //{
                soundEffectSource.Stop();
            //}
        }
        else
        {
            if (soundEffectSource.isPlaying)
            {
                soundEffectSource.Stop();
            }

            soundEffectSource.clip = seClip;
            soundEffectSource.Play();
        }

    }

    private void PlayBGMusic(AudioClip BGClip)
    {
        if(isBGMusicMuted == true)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }
        else
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }

            musicSource.clip = BGClip;
            musicSource.loop = true;
            musicSource.Play();
        }

    }
}
