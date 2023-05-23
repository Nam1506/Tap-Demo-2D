using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class AsyncLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadingSlider;
    public TextMeshProUGUI progressText;

    //Config

    private float loadingTime = 2f;
    private string gameScreenName = "Game";

    private void Start()
    {
        //loadingScreen.SetActive(true);
        StartCoroutine("LoadLevelAsync");
    }

    IEnumerator LoadLevelAsync()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(gameScreenName);
        loadOperation.allowSceneActivation = false;
        float timeLoading = 0f;
        var isDone = false;
        while (!loadOperation.isDone && !isDone)
        {
            timeLoading += Time.deltaTime;
            float progressValue = (timeLoading / loadingTime);
            loadingSlider.value = progressValue;
            progressText.text = (Mathf.Clamp01(progressValue) * 100f).ToString("F1") + "%";
            if (timeLoading >= loadingTime)
            {
                isDone = true;
                loadOperation.allowSceneActivation = true;
            }
            yield return null;

        }
    }
}
