using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupController : MonoBehaviour
{
    public static PopupController Instance;

    [SerializeField] GameObject popUpGame;
    [SerializeField] GameObject popUpChangeNumber;
    [SerializeField] GameObject popUpSetting;
    [SerializeField] GameObject popUpLose;
    [SerializeField] GameObject popUpWin;
    [SerializeField] Canvas canva;

    [SerializeField] GameObject coinPrefab;

    [SerializeField] GameObject fireWorkBase;
    [SerializeField] List<ParticleSystem> fireWork;
    [SerializeField] float speedRandom;

    public List<Image> sprites;
    public List<TextMeshProUGUI> multipleText;
    public int index = 0;

    public TextMeshProUGUI numberX;
    public TextMeshProUGUI coinMultiple;
    public TextMeshProUGUI coinGet;
    public TextMeshProUGUI coin;
    public TextMeshProUGUI coinMiddle;

    public GameObject targetTransformCoin;

    private bool canRandom = false;
    private Coroutine coroutineStartNumber;

    [SerializeField] float jumpPower;
    [SerializeField] float durationJump;
    [SerializeField] int numberCoin;


    //--------Config--------
    private float timeFade = 0.35f;
    [SerializeField] float timeCoin;

    private const float radius = 3f;


    private void Awake()
    {
        Instance = this;
    }

    public GameObject GetPopUpChangeNumber()
    {
        return popUpChangeNumber;
    }

    public void TurnOnPopUpChangeNumber()
    {
        popUpChangeNumber.SetActive(true);

        GameManager.Instance.state = GameManager.State.Pause;
    }

    public void TurnOffPopUpChangeNumber()
    {
        popUpChangeNumber.SetActive(false);

        GameManager.Instance.state = GameManager.State.Play;

    }

    public void TurnOnPopUpSetting()
    {
        AudioManager.Instance.PlaySFXPopUp("Open");
        popUpSetting.SetActive(true);
        popUpSetting.GetComponent<CanvasGroup>().DOFade(1, timeFade);
        GameManager.Instance.state = GameManager.State.Pause;
    }

    public void TurnOffPopUpSetting()
    {
        AudioManager.Instance.PlaySFXPopUp("Close");

        popUpSetting.GetComponent<CanvasGroup>().DOFade(0, timeFade)
            .OnComplete(() =>
            {
                popUpSetting.SetActive(false);
                GameManager.Instance.state = GameManager.State.Play;
            });
    }

    public void TurnOnPopupLose()
    {
        popUpLose.SetActive(true);
        popUpLose.GetComponent<CanvasGroup>().DOFade(1f, timeFade);
        GameManager.Instance.state = GameManager.State.Pause;
    }

    public void TurnOffPopupLose()
    {
        foreach (GridManager grid in GameManager.Instance.listGrid)
        {
            Destroy(grid.gameObject);
        }

        GameManager.Instance.listGrid.Clear();

        JSONSystem.Instance.LoadLevel(PlayerPrefs.GetInt("level"));
        popUpLose.GetComponent<CanvasGroup>().DOFade(0, timeFade)
            .OnComplete(() =>
            {
                popUpLose.SetActive(false);
                GameManager.Instance.state = GameManager.State.Play;
            });
    }

    public void TurnOnPopupWin()
    {
        popUpWin.SetActive(true);
        fireWorkBase.SetActive(true);

        popUpWin.GetComponent<CanvasGroup>().DOFade(1f, timeFade)
            .OnComplete(() =>
            {
                index = 0;
                canRandom = true;
                coroutineStartNumber = StartCoroutine(RandomNumber());
            });


        GameManager.Instance.state = GameManager.State.Pause;
    }

    public void TurnOffPopupWin()
    {
        fireWorkBase.SetActive(false);

        foreach (GridManager grid in GameManager.Instance.listGrid)
        {
            Destroy(grid.gameObject);
        }

        GameManager.Instance.listGrid.Clear();

        JSONSystem.Instance.LoadLevel(PlayerPrefs.GetInt("level") + 1);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);

        popUpWin.GetComponent<CanvasGroup>().DOFade(0, timeFade)
            .OnComplete(() =>
            {
                popUpWin.SetActive(false);
                GameManager.Instance.state = GameManager.State.Play;
                canRandom = false;
            });
    }

    public void GetCoinButton()
    {
        if (!canRandom)
        {
            return;
        }

        StartCoroutine(IncreamentCoin(int.Parse(coinGet.text)));
    }

    public void GetCoinMultipleButton()
    {
        if (!canRandom)
        {
            return;
        }

        StartCoroutine(IncreamentCoin(int.Parse(coinMultiple.text)));
    }

    private IEnumerator IncreamentCoin(int addCoin)
    {
        float currentCoin = float.Parse(PlayerPrefs.GetString("coin"));
        float targetCoin = addCoin + currentCoin;
        PlayerPrefs.SetString("coin", targetCoin.ToString());

        canRandom = false;
        StopCoroutine(coroutineStartNumber);

        multipleText[index].color = Color.white;
        sprites[index].color = Color.white;

        List<GameObject> listCoin = new List<GameObject>();

        int num = 0;
        while (num < numberCoin)
        {
            Vector2 randomPoint = Random.insideUnitCircle * radius;

            GameObject coinPre = Instantiate(coinPrefab, popUpWin.transform.parent);

            coinPrefab.transform.SetAsLastSibling();

            coinPre.transform.position = new Vector3(randomPoint.x, randomPoint.y, coinPre.transform.position.z);

            coinPre.transform.localScale = Vector3.zero;

            listCoin.Add(coinPre);

            num++;
        }

        Vector3 targetJump = new Vector3(targetTransformCoin.transform.position.x, targetTransformCoin.transform.position.y, popUpWin.transform.position.z);


        foreach (GameObject coinPre in listCoin)
        {
            coinPre.transform.position = new Vector3(coinPre.transform.position.x, coinPre.transform.position.y, targetJump.z);
            coinPre.transform.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.05f);

        float time = 0f;

        float waitTime = 0.35f;

        foreach(GameObject coinPre in listCoin)
        {

            DOVirtual.DelayedCall(time, () =>
            {
                coinPre.transform.DOJump(targetJump, jumpPower, 1, durationJump);
                coinPre.transform.DOScale(0.5f, durationJump);
            });


            DOVirtual.DelayedCall(time + durationJump + 0.1f, () =>
            {
                Destroy(coinPre.gameObject);
            });

            time += 0.05f;
                
        }

        float durationWaitCoin = (waitTime + durationJump) / addCoin;
        float totalDuration = waitTime + durationJump;
        float elapsed = 0f;
        float deltaCoin = targetCoin - currentCoin;
        float process;

        while (elapsed <= totalDuration)
        {
            process = Mathf.Clamp01(elapsed / totalDuration);
            currentCoin = targetCoin - (1 - process) * deltaCoin;
            coin.text = Mathf.RoundToInt(currentCoin).ToString();

            elapsed += Time.deltaTime;
            yield return null;
        }

        coin.text = Mathf.RoundToInt(targetCoin).ToString();

        fireWorkBase.SetActive(false);

        foreach (GridManager grid in GameManager.Instance.listGrid)
        {
            Destroy(grid.gameObject);
        }

        GameManager.Instance.listGrid.Clear();

        JSONSystem.Instance.LoadLevel(PlayerPrefs.GetInt("level") + 1);
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);

        popUpWin.GetComponent<CanvasGroup>().DOFade(0, timeFade)
            .OnComplete(() =>
            {
                popUpWin.SetActive(false);
                sprites[index].color = new Color(0.6f, 0.6f, 0.6f, 1);
                multipleText[index].color = new Color(0.6f, 0.6f, 0.6f, 1);
                GameManager.Instance.state = GameManager.State.Play;
            });

    }

    private IEnumerator RandomNumber()
    {

        while (canRandom)
        {

            int pre = index;

            sprites[index].color = Color.white;
            multipleText[index].color = Color.white;

            int coinTemp;

            if (index == 0 || index == 6)
            {
                numberX.text = "Get x2";

                coinTemp = int.Parse(coinGet.text) * 2;
            }

            else if (index == 1 || index == 5)
            {
                numberX.text = "Get x3";

                coinTemp = int.Parse(coinGet.text) * 3;
            }

            else if (index == 2 || index == 4)
            {
                numberX.text = "Get x4";

                coinTemp = int.Parse(coinGet.text) * 4;
            }

            else
            {
                numberX.text = "Get x5";

                coinTemp = int.Parse(coinGet.text) * 5;
            }

            coinMultiple.text = coinTemp.ToString();
            coinMiddle.text = "+" + coinTemp;



            yield return new WaitForSeconds(speedRandom);

            if (index == sprites.Count - 1)
            {
                index = 0;
            }

            else
            {
                index++;
            }
            sprites[pre].color = new Color(0.6f, 0.6f, 0.6f, 1);
            multipleText[pre].color = new Color(0.6f, 0.6f, 0.6f, 1);

        }


    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TurnOnPopupWin();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            TurnOffPopupWin();
        }
    }

}
