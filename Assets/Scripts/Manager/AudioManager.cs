using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public List<Sound> musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource1, sfxSource2, sfxSourcePopup;


    private void Start()
    {

        if (PlayerPrefs.HasKey("vibrate"))
        {
            if (PlayerPrefs.GetInt("vibrate") == 1)
            {

            }
            
        }
        else
        {
            PlayerPrefs.SetInt("vibrate", 1);
        }

        if (PlayerPrefs.HasKey("music"))
        {
            if (PlayerPrefs.GetInt("music") == 1)
            {
                musicSource.mute = false;
            }

            else
            {
                musicSource.mute = true;
            }

        }
        else
        {
            PlayerPrefs.SetInt("music", 1);
        }

        if (PlayerPrefs.HasKey("sfx"))
        {
            if (PlayerPrefs.GetInt("sfx") == 1)
            {
                sfxSource1.mute = false;
                sfxSource2.mute = false;
                sfxSourcePopup.mute = false;
            }

            else
            {
                sfxSource1.mute = true;
                sfxSource2.mute = true;
                sfxSourcePopup.mute = true;
            }

        }
        else
        {
            PlayerPrefs.SetInt("sfx", 1);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void PlayMusic(string name)
    {
        Sound s = musicSounds.Find(x => x.name == name);
        
        if(s != null)
        {
            musicSource.clip = s.audioClip;
            musicSource.Play();
        }
    }

    public void PlaySFX1(string name)
    {
        Sound s = sfxSounds.Find(x => x.name == name);

        if (s != null)
        {
            sfxSource1.PlayOneShot(s.audioClip);
        }
    }

    public void PlaySFX2(string name)
    {
        Sound s = sfxSounds.Find(x => x.name == name);

        if (s != null)
        {
            sfxSource2.PlayOneShot(s.audioClip);
        }
    }

    public void PlaySFXPopUp(string name)
    {
        Sound s = sfxSounds.Find(x => x.name == name);

        if (s != null)
        {
            sfxSourcePopup.PlayOneShot(s.audioClip);
        }
    }
}
