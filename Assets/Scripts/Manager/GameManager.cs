using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public State state;

    public GameObject hiddenPrefab;
    public List<GameObject> listHiddenNumber;

    public GameObject linePrefab;
    public GameObject trailPrefab;

    public List<GameObject> listItem;
    public List<GameObject> listRotate;
    public List<GridManager> listGrid;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moveText;

    public TMP_InputField levelInputField;
    public TMP_InputField rowInputField;
    public TMP_InputField colInputField;
    public TMP_InputField moveInputField;

    public GameObject gridManager;

    public TextMeshProUGUI curCountGrid;
    public TextMeshProUGUI maxCountGrid;

    public bool isRotating = false;
    public int currentIndexGrid = -1;
    public float maxCamera = 10f;
    public float timeCountDown;
    public float timeWaitBlockRotate;
    public float heightRotate;
    public float timeHeightRotate;


    public GridManager currentGrid;

    public int combo = 0;

    public List<GameObject> listBreakPrefab;
    public GameObject explosionPrefab;

    public GameObject addMoveMask;
    public GameObject addBoomMask;

    public GameObject addMove;
    public GameObject addBoom;

    [HideInInspector] public bool moveClicking = false;
    [HideInInspector] public bool boomClicking = false;

    public float startScale;
    public float endScale;
    public float durationScale;

    public TextMeshProUGUI remainAddMove;
    public Tween myTween;

    private void Awake()
    {
        Application.runInBackground = false;
        Application.targetFrameRate = 60;

        Instance = this;
    }

    

    private void Start()
    {
        if (isSceneGame())
        {
            StartScaleEffect(addMove);
            StartScaleEffect(addBoom);


            AudioManager.Instance.PlayMusic("BG");

            if (PlayerPrefs.HasKey("level"))
            {
                if (PlayerPrefs.GetInt("level") == 0)
                {
                    JSONSystem.Instance.LoadLevel(1);
                    PlayerPrefs.SetInt("level", 1);
                }
                else
                {
                    JSONSystem.Instance.LoadLevel(PlayerPrefs.GetInt("level"));
                }
            }

            else
            {
                JSONSystem.Instance.LoadLevel(1);
                PlayerPrefs.SetInt("level", 1);
            }

            if (PlayerPrefs.HasKey("coin"))
            {
                PopupController.Instance.coin.text = PlayerPrefs.GetString("coin");
            }
            else
            {
                PlayerPrefs.SetString("coin", "0");
                PopupController.Instance.coin.text = "0";
                PopupController.Instance.coinGet.text = "20";
            }

        }

        state = State.Play;
    }

    public enum State
    {
        Pause,
        Play
    }

    public bool isSceneGame()
    {
        return SceneManager.GetActiveScene().name == JSONSystem.Instance.gameScene;
    }

    private void StartScaleEffect(GameObject gameObject)
    {
        gameObject.GetComponent<RectTransform>().DOScale(endScale, durationScale).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                gameObject.GetComponent<RectTransform>().DOScale(startScale, durationScale).SetEase(Ease.InBack);
            }).SetLoops(-1, LoopType.Yoyo);
    }
        
    public void NewMapButton()
    {

        ToolManager.Instance.currentRotate = null;

        if(listGrid.Count != 0)
        {
            GameObject preGrid = listGrid[currentIndexGrid].gameObject;
            preGrid.SetActive(false);
            preGrid.GetComponent<GridManager>().moveCount = moveInputField.text;
        }

        GameObject newGridGO = Instantiate(gridManager);
        GridManager newGrid = newGridGO.GetComponent<GridManager>();
        listGrid.Add(newGrid);

        
        currentGrid = newGrid;
        currentIndexGrid = listGrid.Count - 1;

        maxCountGrid.text = listGrid.Count.ToString();
        curCountGrid.text = listGrid.Count.ToString();

        moveInputField.text = "";
        rowInputField.text = "";
        colInputField.text = "";

    }

    public void CreateMapButton()
    {
        ToolManager.Instance.currentRotate = null;


        if (rowInputField.text == "" || colInputField.text == "")
        {
            return;
        }

        if(currentGrid.transform.childCount != 0)
        {
            currentGrid.DestroyMap();
        }

        GridManager.Instance.GenerateGrid(currentGrid);

        CameraController.Instance.SetCameraOrthographic(currentGrid);

    }

    public void BackMapButton()
    {
        if(currentIndexGrid == 0)
        {
            return;
        }

        ToolManager.Instance.currentRotate = null;


        listGrid[currentIndexGrid].gameObject.SetActive(false);
        listGrid[currentIndexGrid].moveCount = moveInputField.text;

        if(rowInputField.text == "")
        {
            listGrid[currentIndexGrid].rows = 0;
        }
        else
        {
            listGrid[currentIndexGrid].rows = int.Parse(rowInputField.text);

        }

        if (colInputField.text == "")
        {
            listGrid[currentIndexGrid].cols = 0;
        }
        else
        {
            listGrid[currentIndexGrid].cols = int.Parse(colInputField.text);

        }


        currentIndexGrid--;

        currentGrid = listGrid[currentIndexGrid];
        currentGrid.gameObject.SetActive(true);

        moveInputField.text = currentGrid.moveCount;

        curCountGrid.text = (currentIndexGrid + 1).ToString();

        rowInputField.text = currentGrid.rows.ToString();
        colInputField.text = currentGrid.cols.ToString() ;

        CameraController.Instance.SetCameraOrthographic(currentGrid);

    }

    public void NextMapButton()
    {
        if (currentIndexGrid == listGrid.Count - 1)
        {
            return;
        }

        ToolManager.Instance.currentRotate = null;

        listGrid[currentIndexGrid].gameObject.SetActive(false);
        listGrid[currentIndexGrid].moveCount = moveInputField.text;

        if (rowInputField.text == "")
        {
            listGrid[currentIndexGrid].rows = 0;
        }
        else
        {
            listGrid[currentIndexGrid].rows = int.Parse(rowInputField.text);

        }

        if (colInputField.text == "")
        {
            listGrid[currentIndexGrid].cols = 0;
        }
        else
        {
            listGrid[currentIndexGrid].cols = int.Parse(colInputField.text);

        }

        currentIndexGrid++;

        currentGrid = listGrid[currentIndexGrid];
        currentGrid.gameObject.SetActive(true);

        moveInputField.text = currentGrid.moveCount;

        curCountGrid.text = (currentIndexGrid + 1).ToString();

        rowInputField.text = currentGrid.rows.ToString();
        colInputField.text = currentGrid.cols.ToString();

        CameraController.Instance.SetCameraOrthographic(currentGrid);
    }

    public void DeleteMapButton()
    {
        if(currentIndexGrid == -1)
        {
            return;
        }

        ToolManager.Instance.currentRotate = null;

        listGrid.Remove(currentGrid);
        Destroy(currentGrid.gameObject);

        if(listGrid.Count != 0)
        {
            maxCountGrid.text = (int.Parse(maxCountGrid.text) - 1).ToString();
            curCountGrid.text = maxCountGrid.text;


            currentIndexGrid = listGrid.Count - 1;

            currentGrid = listGrid[currentIndexGrid];
            currentGrid.gameObject.SetActive(true);

            moveInputField.text = currentGrid.moveCount;
            rowInputField.text = currentGrid.rows.ToString();
            colInputField.text = currentGrid.cols.ToString();
        }

        else
        {
            maxCountGrid.text = "0";
            curCountGrid.text = "0";

            currentIndexGrid = -1;
            currentGrid = null;

            moveInputField.text = "";
            rowInputField.text = "";
            colInputField.text = "";
        }

    }

    public void NewLevelButton()
    {
        foreach(GridManager gridManager in listGrid)
        {
            Destroy(gridManager.gameObject);
        }

        ToolManager.Instance.currentRotate = null;

        rowInputField.text = "";
        colInputField.text = "";
        moveInputField.text = "";

        currentIndexGrid = -1;
        currentGrid = null;

        curCountGrid.text = "0";
        maxCountGrid.text = "0";

        int fileCount = 0;

#if UNITY_EDITOR || UNITY_ANDROID

        string folderPath = Application.dataPath + "/Resources/Levels";
        fileCount = Directory.GetFiles(folderPath).Length;
        fileCount = (fileCount) / 2;

#else
        string folderPath = "Tap Demo_Data/Resources/Levels";
        fileCount = Directory.GetFiles(folderPath).Length;
#endif
        levelInputField.text = (fileCount + 1).ToString();
        listGrid.Clear();
    }
    
    public void ReloadLevel()
    {
        foreach(GridManager grid in GameManager.Instance.listGrid)
        {
            Destroy(grid.gameObject);
        }

        DOTween.KillAll();

        AudioManager.Instance.sfxSource2.pitch = 1;
        GameManager.Instance.combo = 0;

        GameManager.Instance.listGrid.Clear();

        JSONSystem.Instance.LoadLevel(PlayerPrefs.GetInt("level"));

        GameManager.Instance.state = GameManager.State.Play;
    }

    public void ResetLevel()
    {
        if (PlayerPrefs.HasKey("level"))
        {
            PlayerPrefs.SetInt("level", 1);
        }
    }

    public void AddBoomButton()
    {
        addBoomMask.GetComponent<Image>().enabled = false;

        if (!boomClicking)
        {
            boomClicking = true;
            addBoomMask.GetComponent<Image>().enabled = true;

            foreach(TileSlot tileSlot in currentGrid.GetComponentsInChildren<TileSlot>())
            {
                if(tileSlot.transform.childCount == 0)
                {
                    tileSlot.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
                }
            }

        }

        else
        {
            boomClicking = false;

            foreach (TileSlot tileSlot in currentGrid.GetComponentsInChildren<TileSlot>())
            {
                    tileSlot.gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }
        }
    }

    public void AddMoveButton()
    {
        int remainMove = int.Parse(remainAddMove.text);

        if(remainMove > 0)
        {
            remainMove--;
            remainAddMove.text = remainMove.ToString();

            currentGrid.moveCount = (int.Parse(currentGrid.moveCount) + 1).ToString();
            currentGrid.moveText.text = currentGrid.moveCount + " Moves";

        }
    }

}