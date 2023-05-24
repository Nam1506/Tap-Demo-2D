using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    private float screenWidthOriginial;
    private float screenHeightOriginial;
    [SerializeField] RectTransform panelTop;
    [SerializeField] RectTransform panelBot;
    [SerializeField] Canvas canva;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {

    }

    public void SetCameraOrthographic(GridManager gridManager)
    {
        //Camera.main.orthographicSize = 5;

        //// Tính kích thước 16:9
        //float targetScreenWidth = Screen.height * (9f / 16f);
        //float targetScreenHeight = Screen.height;

        //// Kiểm tra xem kích thước hiện tại có lớn hơn kích thước 16:9 không
        //if (Screen.width > targetScreenWidth)
        //{
        //    targetScreenWidth = Screen.width;
        //    targetScreenHeight = Screen.width * (16f / 9f);
        //}

        //else
        //{
        //    targetScreenWidth = Screen.width;
        //    targetScreenHeight = Screen.height;
        //}

        //float maxHorizontalSize = gridManager.cols;
        //float maxVerticalSize = gridManager.rows;

        //float panelTopHeight = (panelTop.sizeDelta.y - 30) * canva.scaleFactor;
        //float panelBotHeight = (panelBot.sizeDelta.y - 30) * canva.scaleFactor;

        //float screenWidth = (float)(targetScreenWidth - 20) / CameraExtension.PixelsPerUnit(Camera.main);
        //float screenHeight = (float)(targetScreenHeight - (panelTopHeight + panelBotHeight)) / CameraExtension.PixelsPerUnit(Camera.main);

        //float widthSize = screenWidth / maxHorizontalSize;
        //float heightSize = screenHeight / maxVerticalSize;
        //float cellSize = Mathf.Min(widthSize, heightSize);

        //float orthographicSize = 5 * (2.56f / cellSize);
        //Camera.main.orthographicSize = orthographicSize;

        Camera.main.orthographicSize = 10;

        float maxHorizontalSize = gridManager.cols;
        float maxVerticalSize = gridManager.rows;

        float panelTopHeight = (panelTop.sizeDelta.y + 30) * canva.scaleFactor;
        float panelBotHeight = (panelBot.sizeDelta.y + 30) * canva.scaleFactor;

        float screenWidth = (float)(Screen.width - 20) / CameraExtension.PixelsPerUnit(Camera.main);
        float screenHeight = (float)(Screen.height - (panelTopHeight + panelBotHeight)) / CameraExtension.PixelsPerUnit(Camera.main);

        float widthSize = screenWidth / maxHorizontalSize;
        float heightSize = screenHeight / maxVerticalSize;
        float cellSize = Mathf.Min(widthSize, heightSize);

        float size;

        if (GameManager.Instance.isSceneGame())
        {
            size = 1.6f;
        }
        else
        {
            size = 2.56f;
        }

        float orthographicSize = 10 * (size / cellSize);


        if (orthographicSize > 10)
        {
            Camera.main.orthographicSize = orthographicSize;
        }

    }
}
