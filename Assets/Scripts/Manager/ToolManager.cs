using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolManager : MonoBehaviour
{

    public static ToolManager Instance;

    public Color normalColor;
    public Color selectedColor;

    [HideInInspector] public GameObject itemSelected;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject hiddenPrefab;

    public List<Button> buttons = new List<Button>();
    private bool isDeleteHidden = false;
    private bool isDeleteItem = false;
    [SerializeField] private Vector3 rotatePosition;

    public GameObject lineRotate;
    private GameObject rotateItem;
    public GameObject currentRotate;

    // --------CONFIG--------

    private const string garbage = "Garbage";
    private const string hidden = "Hidden";
    private const string hiddenGameObject = "Hidden(Clone)";
    private const string rotate = "Rotate";

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        foreach (Button b in buttons)
        {
            b.onClick.AddListener(() => HandleClick(b));
            b.image.color = normalColor;
        }
    }


    private int GetNumberHidden()
    {
        GameObject button = buttons.Find(b => b.name == hidden).gameObject;
        int number = int.Parse(button.GetComponentInChildren<TextMeshProUGUI>().text);

        return number;
    }

    void HandleClick(Button clickedButton)
    {
        clickedButton.image.color = selectedColor;
        itemSelected = clickedButton.gameObject;

        foreach (Button b in buttons)
        {
            if (b != clickedButton)
            {
                b.image.color = normalColor;
            }
        }
    }

    void DeselectButton()
    {
        foreach (Button b in buttons)
        {
            b.image.color = normalColor;
        }
    }

    public void SaveNumber()
    {
        GameObject popUp = PopupController.Instance.GetPopUpChangeNumber();
        string number = popUp.GetComponentInChildren<TMP_InputField>().text;

        GameObject button = buttons.Find(b => b.name == hidden).gameObject;
        button.GetComponentInChildren<TextMeshProUGUI>().text = number;

        PopupController.Instance.TurnOffPopUpChangeNumber();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            PopupController.Instance.TurnOnPopUpChangeNumber();
        }

        if (GameManager.Instance.state == GameManager.State.Pause)
        {
            return;
        }

        // Sinh Item
        if (itemSelected != null && Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {

                GameObject slot = hit.collider.gameObject;

                if (itemSelected.name == garbage)
                {

                    if (slot.transform.childCount != 0)
                    {
                        Destroy(slot.transform.GetChild(0).gameObject);
                    }
                }

                else if (itemSelected.name == hidden)
                {

                    if (slot.transform.childCount == 0)
                    {
                        return;
                    }

                    int number = GetNumberHidden();

                    if (slot.GetComponentInChildren<Canvas>() == null)
                    {
                        GameObject hiddenItem = Instantiate(hiddenPrefab, slot.transform);
                        slot.transform.GetChild(0).transform.localScale = Vector3.zero;

                        hiddenItem.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
                    }

                    else
                    {
                        slot.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
                    }

                }

                else if (slot.GetComponent<TileSlot>() != null)
                {
                    if (slot.transform.childCount != 0)
                    {
                        foreach (Transform transform in slot.transform)
                        {
                            Destroy(transform.gameObject);
                        }
                    }

                    GameObject item = Instantiate(itemPrefab, slot.transform);
                    item.GetComponent<Item>().spriteRenderer.sprite = itemSelected.GetComponent<Image>().sprite;
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localScale = Vector3.one * 0.8f;

                }

            }
        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {

                GameObject slot = hit.collider.gameObject;

                if (slot.GetComponentInChildren<Canvas>() != null && isDeleteItem == false)
                {
                    Destroy(slot.transform.GetChild(1).gameObject);
                    slot.transform.GetChild(0).transform.localScale = Vector3.one * 0.8f;
                    isDeleteHidden = true;
                }

                else
                {
                    if (slot.GetComponentInChildren<Canvas>() == null && slot.transform.childCount != 0 && isDeleteHidden == false)
                    {
                        foreach (Transform transform in slot.transform)
                        {
                            Destroy(transform.gameObject);
                        }

                        isDeleteItem = true;

                    }

                }

            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDeleteItem = false;
            isDeleteHidden = false;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                GameObject slot = hit.collider.gameObject;

                if (slot.transform.childCount == 1)
                {
                    GameObject inventoryItem = slot.transform.GetChild(0).gameObject;
                    Item item = inventoryItem.GetComponent<Item>();

                    if (item.type == rotate)
                    {
                        Debug.Log("Enter");
                        rotateItem = slot;
                        item.GetComponent<SpriteRenderer>().color = Color.red;

                        if (currentRotate == null)
                        {
                            currentRotate = item.gameObject;
                        }

                        else
                        {
                            currentRotate.GetComponent<SpriteRenderer>().color = Color.white;
                            currentRotate = item.gameObject;
                        }
                        DeselectButton();
                        rotatePosition = new Vector3(slot.transform.localPosition.x + GameManager.Instance.currentGrid.transform.position.x, slot.transform.localPosition.y + GameManager.Instance.currentGrid.transform.position.y, -1);
                    }

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                GameObject slot = hit.collider.gameObject;

                if (slot.transform.childCount == 1)
                {
                    if ( slot.GetComponentInChildren<Item>().canConnect() && slot.GetComponentInChildren<LineRenderer>() == null)
                    {

                        GameObject line = Instantiate(lineRotate, slot.transform);
                        LineRenderer lineRenderer = line.GetComponent<LineRenderer>();

                        Line lineComponent = line.GetComponent<Line>();
                        lineComponent.posRotateParent = GameManager.Instance.currentGrid.GetTileSlot(rotateItem);

                        Vector3 targetPosition = new Vector3(slot.transform.localPosition.x + GameManager.Instance.currentGrid.transform.position.x, slot.transform.localPosition.y + GameManager.Instance.currentGrid.transform.position.y, -1);
                        lineRenderer.SetPosition(0, rotatePosition);
                        lineRenderer.SetPosition(1, targetPosition);

                    }

                }
                else if(slot.transform.childCount == 2 && slot.transform.GetComponentInChildren<TextMeshProUGUI>() == null)
                {
                    Destroy(slot.transform.GetComponentInChildren<LineRenderer>().gameObject);
                }

            }
        }



    }
}
