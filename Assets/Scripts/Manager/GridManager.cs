using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] public int rows = 5;
    [SerializeField] public int cols = 8;
    [SerializeField] private float tileSize = 1;
    public List<GameObject> slots;

    [SerializeField] CameraController cameraController;
    [SerializeField] Canvas canva;

    public TMP_InputField rowInputField;
    public TMP_InputField colInputField;

    public List<Sprite> spritesTool;
    public List<Sprite> sprites;

    public List<GameObject> listItem;
    public List<GameObject> listRotate;
    public List<GameObject> listHiddenNumber;
    public GameObject blockedTile;

    public bool haveSaw;
    public string moveCount;

    public GameObject breakPrefab;

    [SerializeField] float TIME_FADE_HIDDEN;
    [SerializeField] TextMeshProUGUI moveText;
    public bool checkRotation = false;

    public Color startColor;
    public Color endColor;




    private void Awake()
    {
        Instance = this;

        if (!GameManager.Instance.isSceneGame())
        {
            rowInputField = GameObject.Find("Row").GetComponent<TMP_InputField>();
            colInputField = GameObject.Find("Col").GetComponent<TMP_InputField>();
        }

        moveText = GameObject.Find("Move").GetComponent<TextMeshProUGUI>();
        cameraController = Camera.main.GetComponent<CameraController>();

    }



    public void DestroyMap()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        slots.Clear();
        GameManager.Instance.listHiddenNumber.Clear();
        GameManager.Instance.listItem.Clear();
        GameManager.Instance.listRotate.Clear();

        if (GameManager.Instance.isSceneGame())
        {
            AudioManager.Instance.sfxSource1.pitch = 1;
            AudioManager.Instance.sfxSource2.pitch = 1;
            AudioManager.Instance.musicSource.pitch = 1;
        }


        transform.position = Vector3.zero;
    }

    public void GenerateGrid(GridManager gridManager)
    {
        if (SceneManager.GetActiveScene().name != JSONSystem.Instance.gameScene)
        {
            rows = int.Parse(rowInputField.text);
            cols = int.Parse(colInputField.text);
        }

        else
        {
            rows = JSONSystem.Instance.rows;
            cols = JSONSystem.Instance.cols;
        }


        GameObject refer = (GameObject)Instantiate(Resources.Load("GrassTile"));

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(refer, gridManager.transform);
                TileSlot tilePref = tile.GetComponent<TileSlot>();

                tilePref.rowPos = row;
                tilePref.colPos = col;

                float posX = col * tileSize;
                float posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);

                if (SceneManager.GetActiveScene().name == JSONSystem.Instance.gameScene)
                {
                    tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                }

                gridManager.slots.Add(tile);
            }
        }

        Destroy(refer);

        float gridW = cols * tileSize;
        float gridH = rows * tileSize;

        gridManager.transform.position = new Vector2(-gridW / 2 + tileSize / 2, gridH / 2 - tileSize / 2);
    }

    public void LoadGenerateGrid(GridManager gridManager)
    {
        rows = JSONSystem.Instance.rows;
        cols = JSONSystem.Instance.cols;

        GameObject refer = (GameObject)Instantiate(Resources.Load("GrassTile"));

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                GameObject tile = (GameObject)Instantiate(refer, gridManager.transform);
                TileSlot tilePref = tile.GetComponent<TileSlot>();

                tilePref.rowPos = row;
                tilePref.colPos = col;

                float posX = col * tileSize;
                float posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);

                if (SceneManager.GetActiveScene().name == JSONSystem.Instance.gameScene)
                {
                    tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                }

                gridManager.slots.Add(tile);
            }
        }

        Destroy(refer);

        float gridW = cols * tileSize;
        float gridH = rows * tileSize;

        gridManager.transform.position = new Vector2(-gridW / 2 + tileSize / 2, gridH / 2 - tileSize / 2);
    }

    public GameObject GetTileSlot(int posX, int posY)
    {
        return slots.Find(item => item.GetComponent<TileSlot>().rowPos == posX && item.GetComponent<TileSlot>().colPos == posY);
    }

    public int GetTileSlot(GameObject slot)
    {
        int index = slots.IndexOf(slot);

        return index;
    }

    public void DecreamentNumber()
    {
        List<int> deleteList = new List<int>();
        foreach (GameObject go in listHiddenNumber)
        {
            TextMeshProUGUI number = go.GetComponentInChildren<TextMeshProUGUI>();
            int target = int.Parse(number.text) - 1;
            number.text = target.ToString();

            if (target == 0)
            {
                number.text = "";
                GameObject item = go.transform.parent.GetChild(0).gameObject;
                int index = listHiddenNumber.IndexOf(go);
                deleteList.Add(index);
                item.transform.localScale = Vector3.one * 1.7f;
                item.transform.GetChild(0).localScale = Vector3.one * 1.2f;

                go.transform.SetParent(transform.root);

                go.GetComponent<SpriteRenderer>().DOFade(0f, TIME_FADE_HIDDEN)
                    .OnComplete(() =>
                    {
                        Destroy(go.gameObject);
                    });
            }
        };

        for (int i = deleteList.Count - 1; i >= 0; i--)
        {
            listHiddenNumber.RemoveAt(deleteList[i]);
        }
    }

    public string GetMovesCount()
    {
        string[] arr = moveText.text.Split(' ');
        return arr[0];
    }

    public void DecreamentMovesCount()
    {
        string number = moveCount;
        moveText.text = (int.Parse(number) - 1).ToString() + " Moves";
        moveCount = GetMovesCount();
    }

    public bool CheckLoseGame()
    {

        if (listItem.Count == 0)
        {
            return false;
        }

        if (int.Parse(moveCount) == 0)
        {
            return true;
        }


        for (int i = 0; i < listRotate.Count; i++)
        {

            GameObject rotateSlot = listRotate[i];
            TileSlot rotateTileSlot = rotateSlot.GetComponent<TileSlot>();

            if (rotateTileSlot.dictionary["Up"].Count == 0 && rotateTileSlot.dictionary["Right"].Count == 0 && rotateTileSlot.dictionary["Down"].Count == 0 && rotateTileSlot.dictionary["Left"].Count == 0)
            {
                break;
            }

            if (this.haveSaw)
            {
                return false;
            }


            if (checkRotation)
            {
                checkRotation = false;
                return false;
            }

        }

        for (int i = 0; i < listItem.Count; i++)
        {

            GameObject tileSlotGO = listItem[i].transform.parent.parent.gameObject;


            if (tileSlotGO.transform.childCount != 1)
            {
                continue;
            }


            TileSlot tileSlot = tileSlotGO.GetComponent<TileSlot>();

            Item item = listItem[i].GetComponent<Item>();


            string typeItem = item.type;

            tileSlot.item = item;
            tileSlot.container = listItem[i].transform.parent.gameObject;


            if (typeItem == Item.Instance.up)
            {

                if (tileSlot.CheckUpDirection())
                {
                    return false;
                }
            }

            if (typeItem == Item.Instance.down)
            {
                if (tileSlot.CheckDownDirection())
                {
                    return false;

                }
            }

            if (typeItem == Item.Instance.left)
            {

                if (tileSlot.CheckLeftDirection())
                {
                    return false;
                }
            }

            if (typeItem == Item.Instance.right)
            {

                if (tileSlot.CheckRightDirection())
                {
                    return false;
                }

            }
        }

        return true;

    }

}
