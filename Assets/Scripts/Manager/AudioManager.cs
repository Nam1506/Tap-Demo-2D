using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public List<Sound> musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource1, sfxSource2, sfxSourcePopup;

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
