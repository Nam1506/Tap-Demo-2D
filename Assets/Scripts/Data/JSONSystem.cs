using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class JSONSystem : MonoBehaviour
{
    public static JSONSystem Instance;

    [SerializeField] GridManager gridManager;
    [SerializeField] TMP_InputField levelField;
    public TMP_InputField moveField;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject containerPrefab;
    [SerializeField] CameraController cameraController;
    [SerializeField] TMP_Dropdown dropDown;
    [SerializeField] GameObject mask;
    [SerializeField] GameObject coverPrefab;

    public int rows;
    public int cols;

    public string gameScene = "Game";

    private const int levelMax = 20;

    private void Awake()
    {
        Instance = this;
    }

    public Color HexToRGBA(string hex)
    {
        if (hex.StartsWith("#"))
            hex = hex.Substring(1);

        if (hex.Length != 6 && hex.Length != 8)
        {
            Debug.LogError("Định dạng hex không hợp lệ!");
            return Color.white;
        }

        byte red = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte green = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte blue = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        byte alpha = 255;

        if (hex.Length == 8)
            alpha = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

        float r = (float)red / 255f;
        float g = (float)green / 255f;
        float b = (float)blue / 255f;
        float a = (float)alpha / 255f;

        return new Color(r, g, b, a);
    }

    public void SaveToJson()
    {
        Data data = new Data();

        data.level = int.Parse(levelField.text);
        data.maxCamera = GameManager.Instance.maxCamera;
        data.maxMap = GameManager.Instance.listGrid.Count;
        data.map = new List<Map>();
        data.color = dropDown.value;

        SaveMap(data);
        string json = JsonConvert.SerializeObject(data);

        File.WriteAllText(Application.dataPath + "/Resources/Levels/Level" + data.level + ".json", json);

        Debug.Log("Save Successful");

        GameManager.Instance.NewLevelButton();

    }

    public void SaveMap(Data data)
    {
        foreach (GridManager grid in GameManager.Instance.listGrid)
        {
            Map map = new Map();
            map.rows = grid.rows;
            map.cols = grid.cols;
            map.move_count = int.Parse(grid.moveCount);

            if (GameManager.Instance.listGrid.IndexOf(grid) == GameManager.Instance.listGrid.Count - 1)
            {
                map.move_count = int.Parse(GameManager.Instance.moveInputField.text);
            }

            map.grid = new List<List<int>>();
            map.grid_number = new List<List<int>>();
            map.grid_rotate = new List<List<int>>();

            for (int i = 0; i < grid.rows; i++)
            {
                List<int> num = new List<int>();
                List<int> number_hidden = new List<int>();
                List<int> number_rotate = new List<int>();

                for (int j = 0; j < grid.cols; j++)
                {
                    GameObject slot = grid.slots.Find(item => item.GetComponent<TileSlot>().rowPos == i && item.GetComponent<TileSlot>().colPos == j);

                    if (slot.transform.childCount == 0)
                    {
                        num.Add(-1);
                        number_hidden.Add(-1);
                        number_rotate.Add(-1);
                    }

                    else
                    {
                        Transform child = slot.transform.GetChild(0);
                        int index = grid.spritesTool.FindIndex(item => item.name == child.GetComponent<SpriteRenderer>().sprite.name);
                        num.Add(index);

                        if (slot.GetComponentInChildren<TextMeshProUGUI>() != null)
                        {
                            number_hidden.Add(int.Parse(slot.transform.GetComponentInChildren<TextMeshProUGUI>().text));
                            number_rotate.Add(-1);
                        }

                        else
                        {
                            number_hidden.Add(-1);

                            if (slot.transform.childCount > 1)
                            {
                                Line line = slot.GetComponentInChildren<Line>();
                                number_rotate.Add(line.posRotateParent);
                            }

                            else
                            {
                                number_rotate.Add(-1);
                            }
                        }


                    }
                    //int index = gridManager.sprites.FindIndex(item => item.name ==)
                }

                map.grid.Add(num);
                map.grid_number.Add(number_hidden);
                map.grid_rotate.Add(number_rotate);
            }
            data.map.Add(map);

        }
    }

    public void LoadFromJson()
    {
        foreach (GridManager grid in GameManager.Instance.listGrid)
        {
            Destroy(grid.gameObject);
        }

        GameManager.Instance.listGrid.Clear();


        int level = int.Parse(levelField.text);

#if UNITY_EDITOR || UNITY_ANDROID
        string path = Resources.Load<TextAsset>("Levels/Level" + level).ToString();
        Data data = JsonConvert.DeserializeObject<Data>(path);
        LoadMap(data);
#else
        string path = File.ReadAllText("Tap Demo_Data/Resources/Levels/Level" + level + ".json");
        Data data = JsonConvert.DeserializeObject<Data>(path);
        LoadMap(data);
#endif
    }

    public void LoadLevel(int number)
    {
        //gridManager.DestroyMap();

        GameManager.Instance.listGrid.Clear();


        int level = number;

        if(level > levelMax)
        {
            int randomNum = Random.Range(levelMax - 5, levelMax + 1);
            level = randomNum;
        }

        string path = Resources.Load<TextAsset>("Levels/Level" + level).ToString();

        Data data = JsonConvert.DeserializeObject<Data>(path);

        LoadMap(data);

    }

    public void LoadMap(Data data)
    {

        if (SceneManager.GetActiveScene().name != gameScene)
        {
            levelField.text = data.level.ToString();
            moveField.text = data.map[0].move_count.ToString();

            GameManager.Instance.rowInputField.text = data.map[0].rows.ToString();
            GameManager.Instance.colInputField.text = data.map[0].cols.ToString();
            GameManager.Instance.curCountGrid.text = "1";
            GameManager.Instance.maxCountGrid.text = data.maxMap.ToString();
            GameManager.Instance.currentIndexGrid = 0;
        }

        if (SceneManager.GetActiveScene().name == gameScene)
        {
            BlockController.Instance.index = data.color;
            GameManager.Instance.levelText.text = "Level " + data.level.ToString();
            GameManager.Instance.moveText.text = data.map[0].move_count.ToString() + " Moves";
        }

        //gridManager.GenerateGrid();


        float distance = 0f;
        float alpha = 0.9f;

        for (int x = 0; x < data.map.Count; x++)
        {
            Map map = data.map[x];
            GridManager grid = Instantiate(GameManager.Instance.gridManager).GetComponent<GridManager>();
            GameManager.Instance.listGrid.Add(grid);
            GameManager.Instance.currentGrid = GameManager.Instance.listGrid[0];
            GameManager.Instance.currentIndexGrid = 0;

            if (GameManager.Instance.isSceneGame())
            {
                grid.breakPrefab = GameManager.Instance.listBreakPrefab[data.color];
            }

            grid.moveCount = map.move_count.ToString();

            grid.rows = map.rows;
            grid.cols = map.cols;

            rows = grid.rows;
            cols = grid.cols;


            grid.LoadGenerateGrid(grid);

            grid.transform.position = new Vector3(grid.transform.position.x, grid.transform.position.y, distance);
            distance += 2;

            if (GameManager.Instance.isSceneGame())
            {
                GameObject maskPrefab = Instantiate(mask, grid.transform);

            }

            for (int i = 0; i < grid.rows; i++)
            {
                for (int j = 0; j < grid.cols; j++)
                {
                    if (map.grid[i][j] == -1)
                    {
                        continue;
                    }

                    GameObject slot = grid.slots.Find(item => item.GetComponent<TileSlot>().rowPos == i && item.GetComponent<TileSlot>().colPos == j);

                    GameObject item;

                    if (GameManager.Instance.isSceneGame())
                    {
                        if (grid.sprites[map.grid[i][j]].name != "Up" && grid.sprites[map.grid[i][j]].name != "Down" &&
                        grid.sprites[map.grid[i][j]].name != "Left" && grid.sprites[map.grid[i][j]].name != "Right")
                        {

                            item = Instantiate(itemPrefab, slot.transform);

                            item.transform.localScale = Vector3.one * 1.2f;

                            item.GetComponent<SpriteRenderer>().sprite = grid.sprites[map.grid[i][j]];

                            if (item.GetComponent<Item>().GetNameSprite() == item.GetComponent<Item>().rotate)
                            {
                                Instantiate(coverPrefab, item.transform);
                                item.transform.localPosition = new Vector3(0, 0, -0.2f);
                            }

                        }

                        else
                        {
                            GameObject container = Instantiate(containerPrefab, slot.transform);


                            item = Instantiate(itemPrefab, container.transform);

                            container.transform.localScale = Vector3.one * 1.7f;
                            item.transform.localScale = Vector3.one * 1.2f;

                            item.transform.localPosition = new Vector3(0, 0, -0.1f);

                            item.GetComponent<SpriteRenderer>().sprite = grid.sprites[map.grid[i][j]];

                            GameObject trail = Instantiate(GameManager.Instance.trailPrefab, container.transform);
                            trail.transform.SetAsLastSibling();
                            TrailRenderer trailRenderer = trail.GetComponent<TrailRenderer>();

                            if (data.color == 0)
                            {
                                trailRenderer.startColor = HexToRGBA("68E0B8");
                                trailRenderer.endColor = HexToRGBA("5CACFD");
                            }
                            else if (data.color == 1)
                            {
                                trailRenderer.startColor = HexToRGBA("8DF151");
                                trailRenderer.endColor = HexToRGBA("61ABFF");
                            }
                            else if (data.color == 2)
                            {
                                trailRenderer.startColor = HexToRGBA("FC818B");
                                trailRenderer.endColor = HexToRGBA("E97634");
                            }
                            else
                            {
                                trailRenderer.startColor = HexToRGBA("FFF85D");
                                trailRenderer.endColor = HexToRGBA("FF5AA3");
                            }

                        }
                    }

                    else
                    {
                        item = Instantiate(itemPrefab, slot.transform);
                        item.GetComponent<SpriteRenderer>().sprite = grid.spritesTool[map.grid[i][j]];
                        item.transform.localScale = Vector3.one * 0.8f;

                    }

                    if (map.grid_rotate[i][j] != -1)
                    {
                        GameObject linePrefab;

                        if (!GameManager.Instance.isSceneGame())
                        {
                            linePrefab = Instantiate(GameManager.Instance.linePrefab, slot.transform);
                        }

                        else
                        {
                            linePrefab = Instantiate(GameManager.Instance.linePrefab, slot.transform.GetChild(0));
                        }

                        linePrefab.GetComponent<Line>().posRotateParent = map.grid_rotate[i][j];

                        LineRenderer lineRenderer = linePrefab.GetComponent<LineRenderer>();

                        lineRenderer.transform.localPosition = item.transform.localPosition;

                        GameObject rotateItem = grid.slots[map.grid_rotate[i][j]];

                        if (!grid.listRotate.Contains(rotateItem))
                        {
                            grid.listRotate.Add(rotateItem);
                        }

                        TileSlot tileRotateItem = rotateItem.GetComponent<TileSlot>();

                        Item itemSlot = slot.GetComponentInChildren<Item>();
                        itemSlot.rotateController = rotateItem.GetComponent<TileSlot>();


                        Vector3 currentPos = new Vector3(slot.transform.localPosition.x + grid.transform.position.x, slot.transform.localPosition.y + grid.transform.position.y, grid.transform.position.z - 0.3f);
                        Vector3 rotateItemPos = new Vector3(rotateItem.transform.localPosition.x + grid.transform.position.x, rotateItem.transform.localPosition.y + grid.transform.position.y, grid.transform.position.z - 0.3f);
                        lineRenderer.SetPosition(0, currentPos);
                        lineRenderer.SetPosition(1, rotateItemPos);

                        TileSlot tileSlot = slot.GetComponent<TileSlot>();

                        int rowPosRotateItem = map.grid_rotate[i][j] / grid.cols;
                        int colPosRotateItem = map.grid_rotate[i][j] % grid.cols;

                        if (!tileRotateItem.dictionary.ContainsKey("Up"))
                        {
                            tileRotateItem.dictionary.Add("Up", new List<Item>());

                        }

                        if (!tileRotateItem.dictionary.ContainsKey("Right"))
                        {
                            tileRotateItem.dictionary.Add("Right", new List<Item>());

                        }

                        if (!tileRotateItem.dictionary.ContainsKey("Down"))
                        {
                            tileRotateItem.dictionary.Add("Down", new List<Item>());

                        }

                        if (!tileRotateItem.dictionary.ContainsKey("Left"))
                        {
                            tileRotateItem.dictionary.Add("Left", new List<Item>());

                        }

                        if (tileSlot.colPos == colPosRotateItem)
                        {
                            tileSlot.rotateParent = rotateItem;

                            if (tileSlot.rowPos < rowPosRotateItem)
                            {
                                tileRotateItem.dictionary["Up"].Add(tileSlot.gameObject.GetComponentInChildren<Item>());
                            }

                            else
                            {

                                tileRotateItem.dictionary["Down"].Add(tileSlot.gameObject.GetComponentInChildren<Item>());

                            }
                        }

                        if (tileSlot.rowPos == rowPosRotateItem)
                        {
                            tileSlot.rotateParent = rotateItem;

                            if (tileSlot.colPos < colPosRotateItem)
                            {
                                tileRotateItem.dictionary["Left"].Add(tileSlot.gameObject.GetComponentInChildren<Item>());
                            }

                            else
                            {
                                tileRotateItem.dictionary["Right"].Add(tileSlot.gameObject.GetComponentInChildren<Item>());
                            }
                        }
                    }

                    if (map.grid_number[i][j] != -1)
                    {
                        GameObject hiddenPrefab = Instantiate(GameManager.Instance.hiddenPrefab, slot.transform);
                        hiddenPrefab.GetComponentInChildren<TextMeshProUGUI>().text = map.grid_number[i][j].ToString();
                        item.transform.localScale = Vector3.zero;
                        grid.listHiddenNumber.Add(hiddenPrefab);
                    }

                    Item itemm = item.GetComponent<Item>();
                    itemm.type = itemm.spriteRenderer.sprite.name;

                    if (itemm.isArrow())
                    {
                        grid.listItem.Add(item);

                    }

                }
            }
        }


        float cameraSize = 10f;


        foreach (GridManager grid in GameManager.Instance.listGrid)
        {
            CameraController.Instance.SetCameraOrthographic(grid);

            cameraSize = Mathf.Max(cameraSize, Camera.main.orthographicSize);
        }

        Camera.main.orthographicSize = cameraSize;

        if (!GameManager.Instance.isSceneGame())
        {
            for (int i = 1; i < GameManager.Instance.listGrid.Count; i++)
            {
                GameManager.Instance.listGrid[i].gameObject.SetActive(false);
            }
        }

        else
        {

            for (int i = 0; i < GameManager.Instance.listGrid.Count; i++)
            {

                GridManager grid = GameManager.Instance.listGrid[i];

                if (i >= 1)
                {
                    foreach (GameObject slot in grid.slots)
                    {
                        slot.GetComponent<BoxCollider2D>().enabled = false;
                    }
                }

                for (int j = 0; j < grid.listRotate.Count; j++)
                {
                    GameObject rotater = grid.listRotate[j];

                    if (!grid.haveSaw)
                    {
                        foreach (Item item in rotater.transform.GetComponent<TileSlot>().dictionary["Up"])
                        {
                            if (item.type == item.saw)
                            {
                                grid.haveSaw = true;
                                break;
                            }
                        }

                        foreach (Item item in rotater.transform.GetComponent<TileSlot>().dictionary["Right"])
                        {
                            if (item.type == item.saw)
                            {
                                grid.haveSaw = true;
                                break;
                            }
                        }
                        foreach (Item item in rotater.transform.GetComponent<TileSlot>().dictionary["Down"])
                        {
                            if (item.type == item.saw)
                            {
                                grid.haveSaw = true;
                                break;
                            }
                        }
                        foreach (Item item in rotater.transform.GetComponent<TileSlot>().dictionary["Left"])
                        {
                            if (item.type == item.saw)
                            {
                                grid.haveSaw = true;
                                break;
                            }
                        }
                    }
                }

            }

        }

    }
}
