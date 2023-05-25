using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField] RectTransform uiHandleRectTransform;
    [SerializeField] Sprite activeSprite;
    [SerializeField] Sprite deActiveSprite;
    [SerializeField] AudioSource source;
    [SerializeField] AudioSource source1;
    [SerializeField] AudioSource sourcePopup;


    private Image image;

    Toggle toggle;

    Vector2 handlePosition;

    private void Awake()
    {

        image = GetComponent<Image>();

        toggle = GetComponent<Toggle>();

        handlePosition = uiHandleRectTransform.anchoredPosition;

        toggle.onValueChanged.AddListener(OnSwitch);

    }

    private void Start()
    {

        if (source == null)
        {
            if (PlayerPrefs.HasKey("vibrate"))
            {
                if (PlayerPrefs.GetInt("vibrate") == 1)
                {
                    OnSwitch(true);
                }

                else
                {
                    OnSwitch(false);
                }
            }
            else
            {
                PlayerPrefs.SetInt("vibrate", 1);
                OnSwitch(true);
            }
        }

        if (source == AudioManager.Instance.musicSource)
        {
            if (PlayerPrefs.HasKey("music"))
            {
                if (PlayerPrefs.GetInt("music") == 1)
                {
                    OnSwitch(true);
                }

                else
                {
                    OnSwitch(false);
                }

            }

            else
            {
                PlayerPrefs.SetInt("music", 1);
                OnSwitch(true);
            }
        }

        if (source == AudioManager.Instance.sfxSource1)
        {
            if (PlayerPrefs.HasKey("sfx"))
            {
                if (PlayerPrefs.GetInt("sfx") == 1)
                {
                    OnSwitch(true);
                }
                else
                {
                    OnSwitch(false);
                }
            }

            else
            {
                PlayerPrefs.SetInt("sfx", 1);
                OnSwitch(true);
            }

        }

    }


    void OnSwitch(bool on)
    {
        if (on)
        {

            if (source == null)
            {
                uiHandleRectTransform.DOAnchorPos(handlePosition * -1, 0.1f);

                image.sprite = activeSprite;

                toggle.isOn = true;

                PlayerPrefs.SetInt("vibrate", 1);
            }

            else
            {
                source.mute = false;

                if(source1 != null)
                {
                    source1.mute = false;
                }

                if(sourcePopup != null)
                {
                    sourcePopup.mute = false;
                }

                uiHandleRectTransform.DOAnchorPos(handlePosition * -1, 0.1f);

                image.sprite = activeSprite;

                toggle.isOn = true;

                if (source == AudioManager.Instance.musicSource)
                {
                    PlayerPrefs.SetInt("music", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("sfx", 1);
                }

            }

        }

        else
        {

            if (source == null)
            {
                uiHandleRectTransform.DOAnchorPos(handlePosition, 0.1f);

                image.sprite = deActiveSprite;

                toggle.isOn = false;

                PlayerPrefs.SetInt("vibrate", 0);
            }
            else
            {
                source.mute = true;

                if (source1 != null)
                {
                    source1.mute = true;
                }

                if (sourcePopup != null)
                {
                    sourcePopup.mute = true;
                }

                uiHandleRectTransform.DOAnchorPos(handlePosition, 0.1f);

                image.sprite = deActiveSprite;

                toggle.isOn = false;

                if (source == AudioManager.Instance.musicSource)
                {
                    PlayerPrefs.SetInt("music", 0);
                }
                else
                {
                    PlayerPrefs.SetInt("sfx", 0);
                }
            }

        }

    }
}
