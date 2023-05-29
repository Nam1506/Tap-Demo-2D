using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class TileSlot : MonoBehaviour
{
    [SerializeField] GridManager gridManager;
    public int rowPos;
    public int colPos;

    [HideInInspector] public Item item;
    [HideInInspector] public GameObject container;

    [SerializeField] Ease ease = Ease.InCubic;

    [SerializeField] float speed = 1f;

    public bool canMove = true;
    public bool haveBoom = false;

    [SerializeField] BoxCollider2D boxTileSlot;

    [ShowInInspector]
    public Dictionary<string, List<Item>> dictionary = new Dictionary<string, List<Item>>();

    [SerializeField] float speedRotate;

    public GameObject rotateParent;

    [SerializeField] float timeScaleBlock;
    [SerializeField] float targetScaleBlock;

    float durationCheck = 0f;

    // ----------------------------------------Config--------------------------------
    private const float TIME_BREAK = 0.15f;
    private const float TIME_COLOR = 0.25f;
    private const float DISTANCE_MOVE = 0.1f;
    private const float TIME_DISTANCE_MOVE = 0.05f;
    private const float TIME_WAIT_WIN = 1f;
    private const float TIME_WAIT_FADE_GRID = 0.5f;
    private const float TIME_FADE_GRID = 0.5f;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        Vibration.Init();
    }


    private void OnMouseDown()
    {
        Debug.Log("1");
        if (!GameManager.Instance.isSceneGame())
        {
            return;
        }
        Debug.Log("1");

        if (GameManager.Instance.state == GameManager.State.Pause)
        {
            return;
        }

        Debug.Log("1");

        if (GameManager.Instance.currentGrid.moveCount == "0")
        {
            return;
        }
        Debug.Log("1");


        if (this.transform.childCount != 1)
        {
            if (GameManager.Instance.boomClicking)
            {
                GameObject itemm = Instantiate(JSONSystem.Instance.boomObject, this.transform);

            }

            return;
        }

        if (GameManager.Instance.isRotating)
        {
            return;
        }
        Debug.Log("1");

        haveBoom = false;

        durationCheck = timeScaleBlock * 2;

        container = this.transform.GetChild(0).gameObject;
        item = container.GetComponentInChildren<Item>();

        string nameItem = item.type;

        if (nameItem == item.saw || nameItem == item.boom)
        {
            return;
        }

        if (nameItem == item.up)
        {
            MoveUpDirection();
            VibrateNope();
        }

        if (nameItem == item.down)
        {
            MoveDownDirection();
            VibrateNope();

        }

        if (nameItem == item.left)
        {
            MoveLeftDirection();
            VibrateNope();

        }

        if (nameItem == item.right)
        {
            MoveRightDirection();
            VibrateNope();

        }

        int degree = 0;

        if (nameItem == item.rotate)
        {
            canMove = false;
            degree = CheckRotate();
            Debug.Log("Degree: " + degree);
            DoRotate(degree);
        }


        GameManager.Instance.currentGrid.DecreamentMovesCount();


        if (canMove == true)
        {

            if (GameManager.Instance.combo >= 1)
            {
                AudioManager.Instance.PlaySFX2("SFX");
            }
            else
            {
                AudioManager.Instance.PlaySFX1("SFX");
            }

            GameManager.Instance.combo++;
            AudioManager.Instance.sfxSource2.pitch += 0.1f;
            GameManager.Instance.currentGrid.DecreamentNumber();
        }

        else
        {
            if (nameItem != item.rotate)
            {
                GameManager.Instance.combo = 0;
                AudioManager.Instance.sfxSource2.pitch = 1;
            }

            canMove = true;
        }

        if (nameItem == item.rotate)
        {
            bool isOver = GameManager.Instance.currentGrid.CheckLoseGame();

            DOVirtual.DelayedCall(0.1f, () =>
            {
                Debug.Log("isOver1: " + isOver);

                if (isOver == true)
                {
                    GameManager.Instance.state = GameManager.State.Pause;

                    if (degree == 0)
                    {

                        Debug.Log("Check = 0");

                        GridManager gridCurrent = GameManager.Instance.currentGrid;

                        float time = 0f;

                        float duration = GameManager.Instance.timeCountDown;

                        for (int i = 0; i < gridCurrent.listItem.Count; i++)
                        {
                            SpriteRenderer spriteRenderer = gridCurrent.listItem[i].transform.parent.GetComponent<SpriteRenderer>();

                            DOVirtual.DelayedCall(time + duration, () =>
                            {
                                spriteRenderer.DOColor(new Color(1, 0, 0, 1), duration * 2)
                                    .OnComplete(() =>
                                    {
                                        spriteRenderer.DOColor(new Color(1, 1, 1, 1), duration * 2);
                                    });
                            });

                            time += duration;

                        }

                        DOVirtual.DelayedCall(time + duration * 2, () =>
                        {
                            AudioManager.Instance.sfxSource2.pitch = 1;
                            GameManager.Instance.combo = 0;
                            PopupController.Instance.TurnOnPopupLose();
                        });
                    }

                }
            });
        }

        if (nameItem != item.rotate && !haveBoom)
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {
                bool isOver = GameManager.Instance.currentGrid.CheckLoseGame();

                if (isOver == true)
                {
                    GameManager.Instance.state = GameManager.State.Pause;

                    GridManager gridCurrent = GameManager.Instance.currentGrid;

                    float time = 0f;

                    float duration;
                    if (gridCurrent.listItem.Count != 0)
                    {
                        duration = GameManager.Instance.timeCountDown;

                        for (int i = 0; i < gridCurrent.listItem.Count; i++)
                        {
                            SpriteRenderer spriteRenderer = gridCurrent.listItem[i].transform.parent.GetComponent<SpriteRenderer>();

                            DOVirtual.DelayedCall(time + duration, () =>
                            {
                                spriteRenderer.DOColor(new Color(1, 0, 0, 1), duration * 2)
                                    .OnComplete(() =>
                                    {
                                        spriteRenderer.DOColor(new Color(1, 1, 1, 1), duration * 2);
                                    });
                            });

                            time += duration;

                        }

                    }

                    else
                    {
                        duration = 0;
                    }


                    DOVirtual.DelayedCall(time + duration * 2, () =>
                    {
                        AudioManager.Instance.sfxSource2.pitch = 1;
                        GameManager.Instance.combo = 0;
                        PopupController.Instance.TurnOnPopupLose();
                    });
                }

                if (isOver == false)
                {
                    if (GameManager.Instance.currentGrid.listItem.Count == 0)
                    {

                        GameManager.Instance.state = GameManager.State.Pause;

                        int index = GameManager.Instance.currentIndexGrid;

                        if (index == GameManager.Instance.listGrid.Count - 1)
                        {
                            DOVirtual.DelayedCall(TIME_WAIT_WIN, () =>
                            {
                                AudioManager.Instance.sfxSource2.pitch = 1;
                                GameManager.Instance.combo = 0;

                                PopupController.Instance.TurnOnPopupWin();
                            });
                            return;
                        }

                        DOVirtual.DelayedCall(TIME_WAIT_FADE_GRID, () =>
                        {
                            foreach (SpriteRenderer spritePrefab in GameManager.Instance.listGrid[index].GetComponentsInChildren<SpriteRenderer>())
                            {
                                spritePrefab.DOFade(0f, TIME_FADE_GRID)
                                    .OnComplete(() =>
                                    {
                                        AudioManager.Instance.sfxSource2.pitch = 1;
                                        GameManager.Instance.combo = 0;

                                        GameManager.Instance.state = GameManager.State.Play;
                                        GameManager.Instance.listGrid[index].gameObject.SetActive(false);
                                    });
                            }

                            GameManager.Instance.currentIndexGrid++;
                            GameManager.Instance.currentGrid = GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid];
                            GameManager.Instance.moveText.text = GameManager.Instance.currentGrid.moveCount + " Moves";

                            foreach (GameObject slot in GameManager.Instance.currentGrid.slots)
                            {
                                slot.GetComponent<BoxCollider2D>().enabled = true;
                            }

                        });

                    }
                }

            });

        }


    }

    private bool CheckChildrenOfRotation(TileSlot tileSlot)
    {
        Item item = tileSlot.GetComponentInChildren<Item>();

        if (item.rotateController == null)
        {
            return false;
        }

        if (item.type == item.saw)
        {
            LineRenderer line = item.GetComponentInChildren<LineRenderer>();
            Destroy(line.gameObject);
        }

        else
        {
            LineRenderer line = item.transform.parent.GetComponentInChildren<LineRenderer>();
            Destroy(line.gameObject);
        }

        TileSlot rotation = item.rotateController;

        if (rotation.dictionary["Up"].Contains(item))
        {
            rotation.dictionary["Up"].Remove(item);
        }

        else if (rotation.dictionary["Right"].Contains(item))
        {
            rotation.dictionary["Right"].Remove(item);
        }

        else if (rotation.dictionary["Down"].Contains(item))
        {
            rotation.dictionary["Down"].Remove(item);
        }

        else if (rotation.dictionary["Left"].Contains(item))
        {
            rotation.dictionary["Left"].Remove(item);
        }

        return true;

    }

    private void VibratePeek()
    {
        if (PlayerPrefs.GetInt("vibrate") == 1)
        {
            DOVirtual.DelayedCall(0.1f, () =>
            {

                Vibration.VibratePeek();
            });
        }
    }

    private void VibrateNope()
    {
        if (PlayerPrefs.GetInt("vibrate") == 1)
        {
            Vibration.VibrateNope();
        }
    }


    private void MoveUpDirection()
    {
        bool hasItemAbove = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;
        var duration = 0f;

        int posX = -1;

        if (rowPos == 0)
        {
            hasItemAbove = false;
        }
        else
        {
            for (int i = rowPos - 1; i >= 0; i--)
            {

                GameObject prefab = gridManager.GetTileSlot(i, colPos);

                if (prefab.transform.childCount == 0)
                {
                    hasItemAbove = false;
                    posX = i;
                }

                else
                {

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().type == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == rowPos - 1)
                    {
                        posX = i + 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().type == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemAbove = true;
                    break;
                }

            }
        }

        if (!hasItemAbove)
        {
            CheckChildrenOfRotation(this);

            canMove = true;
            int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
            GameManager.Instance.currentGrid.listItem.RemoveAt(index);

            var floatEnd = (Screen.height / 2 + (2.56f * Camera.main.orthographicSize) * 2f) / CameraExtension.PixelsPerUnit(Camera.main);
            duration = Vector2.Distance(this.transform.position, new Vector2(this.transform.position.x, floatEnd)) / speed;

            durationCheck = duration;

            this.container.transform.SetParent(transform.root);

            GameObject away = this.container;



            float scale = away.transform.localScale.x;

            away.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                .OnComplete(() =>
                {
                    away.transform.DOScale(scale, timeScaleBlock)
                    .OnComplete(() =>
                    {
                        away.transform.DOMoveY(floatEnd, duration, false).SetEase(ease)
                           .OnComplete(() =>
                           {
                               Destroy(away.gameObject);
                           });
                    });
                });

        }

        else
        {
            if (posX != -1)
            {
                if (isBoom)
                {
                    haveBoom = true;
                }

                GameObject targetTileSlot = gridManager.GetTileSlot(posX, colPos);
                GameObject colorTileSlot = gridManager.GetTileSlot(posX - 1, colPos);

                if (isNextTo && !isBoom && !isSaw)
                {
                    SpriteRenderer colorTileSlotSprite = colorTileSlot.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    colorTileSlotSprite.color = new Color(1, 0, 0, 1);
                    DOVirtual.DelayedCall(TIME_COLOR, () =>
                    {
                        colorTileSlotSprite.color = new Color(1, 1, 1, 1);
                    });

                    //My
                    List<BoxCollider2D> listBox = new List<BoxCollider2D>();

                    GameObject targetBlockSlot = targetTileSlot.transform.GetChild(0).gameObject;
                    BoxCollider2D boxCollider = targetTileSlot.GetComponent<BoxCollider2D>();

                    boxCollider.enabled = false;
                    listBox.Add(boxCollider);

                    float scale = targetBlockSlot.transform.localScale.x;

                    targetBlockSlot.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            targetBlockSlot.transform.DOScale(scale, timeScaleBlock)
                            .OnComplete(() =>
                            {
                                float y1Original = targetTileSlot.transform.position.y;
                                targetBlockSlot.transform.DOMoveY(y1Original + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                    .OnComplete(() =>
                                    {
                                        targetBlockSlot.transform.DOMoveY(y1Original, TIME_DISTANCE_MOVE).SetEase(ease);
                                    });
                            });
                        });

                    //Other
                    float x = 0;


                    for (int i = posX - 1; i >= 0; i--)
                    {

                        if (gridManager.GetTileSlot(i, colPos).transform.childCount == 0)
                        {
                            break;
                        }

                        if (gridManager.GetTileSlot(i, colPos).transform.GetChild(0).transform.childCount == 0)
                        {
                            continue;
                        }

                        GameObject tileSlot = gridManager.GetTileSlot(i, colPos);
                        GameObject blockSlot = tileSlot.transform.GetChild(0).gameObject;
                        BoxCollider2D boxColorCollider = tileSlot.GetComponent<BoxCollider2D>();

                        boxColorCollider.enabled = false;
                        listBox.Add(boxColorCollider);

                        //float yOriginal = blockSlot.transform.position.y;
                        float yOriginal = tileSlot.transform.position.y;
                        DOVirtual.DelayedCall(TIME_DISTANCE_MOVE + x, () =>
                        {
                            blockSlot.transform.DOMoveY(yOriginal + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                            .OnComplete(() =>
                            {
                                blockSlot.transform.DOMoveY(yOriginal, TIME_DISTANCE_MOVE).SetEase(ease);
                            });
                        });
                        x += TIME_DISTANCE_MOVE;
                    }

                    DOVirtual.DelayedCall(x + TIME_DISTANCE_MOVE * 2, () =>
                    {
                        foreach (BoxCollider2D box in listBox)
                        {
                            box.enabled = true;
                        }
                    });

                    VibratePeek();
                    canMove = false;
                }

                else
                {
                    CheckChildrenOfRotation(this);

                    GameObject moveBlock = this.container;

                    if (isSaw)
                    {
                        moveBlock.transform.SetParent(transform.root);
                        int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
                        GameManager.Instance.currentGrid.listItem.RemoveAt(index);
                    }
                    else
                    {
                        moveBlock.transform.SetParent(targetTileSlot.transform);
                    }

                    TileSlot target = targetTileSlot.GetComponent<TileSlot>();
                    target.boxTileSlot.enabled = false;

                    float scale = moveBlock.transform.localScale.x;

                    var floatEnd = targetTileSlot.transform.position.y;

                    duration = Vector2.Distance(this.transform.position, new Vector2(this.transform.position.x, floatEnd)) / speed;

                    durationCheck = duration;

                    moveBlock.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            moveBlock.transform.DOScale(scale, timeScaleBlock)
                                    .OnComplete(() =>
                                    {

                                        TileSlot beside = gridManager.GetTileSlot(posX - 1, colPos).GetComponent<TileSlot>();

                                        if (beside.gameObject.transform.GetChild(0).transform.childCount != 0)
                                        {
                                            moveBlock.transform.DOMoveY(floatEnd, duration).SetEase(ease)
                                           .OnComplete(() =>
                                           {

                                               BoxCollider2D besideBox = beside.gameObject.GetComponent<BoxCollider2D>();

                                               float yOriginal = moveBlock.transform.position.y;
                                               float yOriginalBeside = beside.transform.position.y;

                                               if (beside.transform.childCount != 0)
                                               {

                                                   moveBlock.transform.DOMoveY(yOriginal + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                                       .OnComplete(() =>
                                                       {
                                                           moveBlock.transform.DOMoveY(yOriginal, TIME_DISTANCE_MOVE).SetEase(ease)
                                                               .OnComplete(() =>
                                                               {
                                                                   target.boxTileSlot.enabled = true;
                                                               });
                                                       });

                                                   besideBox.enabled = false;

                                                   beside.transform.DOMoveY(yOriginalBeside + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                                   .OnComplete(() =>
                                                   {
                                                       beside.transform.DOMoveY(yOriginalBeside, TIME_DISTANCE_MOVE).SetEase(ease)
                                                           .OnComplete(() =>
                                                           {
                                                               besideBox.enabled = true;
                                                           });
                                                   });
                                               }
                                               else
                                               {
                                                   target.boxTileSlot.enabled = true;
                                               }

                                           });
                                        }

                                        else
                                        {
                                            moveBlock.transform.DOMoveY(floatEnd, duration).SetEase(ease);
                                        }


                                        if (isSaw)
                                        {


                                            if (isNextTo)
                                            {

                                                Destroy(moveBlock.gameObject);

                                                AudioManager.Instance.PlaySFX1("Break");

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });

                                            }

                                            else
                                            {
                                                DOVirtual.DelayedCall(duration, () =>
                                                {
                                                    AudioManager.Instance.PlaySFX1("Break");

                                                    Destroy(moveBlock.gameObject);

                                                    GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                    breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                    ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                    float timeBreak = main.duration;

                                                    DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                    {
                                                        Destroy(breakPre.gameObject);
                                                    });


                                                });
                                            }

                                        }

                                        if (isBoom)
                                        {
                                            if (isNextTo)
                                            {

                                                GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                                explosionPre.transform.position = colorTileSlot.transform.position;
                                                AudioManager.Instance.PlaySFX1("Explosion");

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });

                                                DOVirtual.DelayedCall(1f, () =>
                                                {
                                                    Destroy(explosionPre.gameObject);
                                                });

                                                Explosion(posX - 1, colPos);
                                            }

                                            else
                                            {
                                                DOVirtual.DelayedCall(duration, () =>
                                                {
                                                    AudioManager.Instance.PlaySFX1("Explosion");

                                                    GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                    breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                    ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                    float timeBreak = main.duration;

                                                    DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                    {
                                                        Destroy(breakPre.gameObject);
                                                    });

                                                    GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                                    explosionPre.transform.position = colorTileSlot.transform.position;
                                                    Explosion(posX - 1, colPos);

                                                    DOVirtual.DelayedCall(1f, () =>
                                                    {
                                                        Destroy(explosionPre.gameObject);
                                                        
                                                    });
                                                });
                                            }
                                        }
                                    });
                        });

                }

            }
        }

    }

    private void MoveDownDirection()
    {
        bool hasItemUnder = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;
        var duration = 0f;

        int posX = -1;

        if (rowPos == gridManager.rows - 1)
        {
            hasItemUnder = false;
        }
        else
        {
            for (int i = rowPos + 1; i < gridManager.rows; i++)
            {
                GameObject prefab = gridManager.GetTileSlot(i, colPos);

                if (prefab.transform.childCount == 0)
                {
                    hasItemUnder = false;
                    posX = i;
                }

                else
                {
                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.saw)
                    {
                        isSaw = true;
                    }

                    if (i == rowPos + 1)
                    {
                        posX = i - 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemUnder = true;
                    break;
                }

            }
        }

        if (!hasItemUnder)
        {
            CheckChildrenOfRotation(this);

            canMove = true;

            this.container.transform.SetParent(transform.root);

            int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
            GameManager.Instance.currentGrid.listItem.RemoveAt(index);

            var floatEnd = (Screen.height / 2 + (2.56f * Camera.main.orthographicSize) * 2f) / CameraExtension.PixelsPerUnit(Camera.main);

            duration = Vector2.Distance(this.transform.position, new Vector2(this.transform.position.x, -floatEnd)) / speed;

            durationCheck = duration;

            GameObject away = this.container;


            float scale = away.transform.localScale.x;

            away.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                .OnComplete(() =>
                {
                    away.transform.DOScale(scale, timeScaleBlock)
                    .OnComplete(() =>
                    {
                        away.transform.DOMoveY(-floatEnd, duration, false).SetEase(ease)
                            .OnComplete(() =>
                            {
                                Destroy(away.gameObject);
                            });
                    });
                });



        }

        else
        {
            if (posX != -1)
            {
                List<BoxCollider2D> listBox = new List<BoxCollider2D>();
                GameObject targetTileSlot = gridManager.GetTileSlot(posX, colPos);
                GameObject colorTileSlot = gridManager.GetTileSlot(posX + 1, colPos);

                if (isBoom)
                {
                    haveBoom = true;
                }

                if (isNextTo && !isBoom && !isSaw)
                {
                    SpriteRenderer colorTileSlotSprite = colorTileSlot.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    colorTileSlotSprite.color = new Color(1, 0, 0, 1);

                    DOVirtual.DelayedCall(TIME_COLOR, () =>
                    {
                        colorTileSlotSprite.color = new Color(1, 1, 1, 1);
                    });

                    GameObject targetBlockSlot = targetTileSlot.transform.GetChild(0).gameObject;
                    BoxCollider2D boxCollider = targetTileSlot.GetComponent<BoxCollider2D>();

                    boxCollider.enabled = false;
                    listBox.Add(boxCollider);


                    float scale = targetBlockSlot.transform.localScale.x;

                    targetBlockSlot.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            targetBlockSlot.transform.DOScale(scale, timeScaleBlock)
                            .OnComplete(() =>
                            {
                                float y1Original = targetTileSlot.transform.position.y;

                                targetBlockSlot.transform.DOMoveY(y1Original - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                    .OnComplete(() =>
                                    {
                                        targetBlockSlot.transform.DOMoveY(y1Original, TIME_DISTANCE_MOVE).SetEase(ease);

                                    });
                            });
                        });


                    float x = 0;
                    for (int i = posX + 1; i < gridManager.rows; i++)
                    {
                        if (gridManager.GetTileSlot(i, colPos).transform.childCount == 0)
                        {
                            break;
                        }

                        if (gridManager.GetTileSlot(i, colPos).transform.GetChild(0).transform.childCount == 0)
                        {
                            continue;
                        }

                        GameObject tileSlot = gridManager.GetTileSlot(i, colPos);
                        GameObject blockSlot = tileSlot.transform.GetChild(0).gameObject;
                        BoxCollider2D boxColorCollider = tileSlot.GetComponent<BoxCollider2D>();

                        boxColorCollider.enabled = false;
                        listBox.Add(boxColorCollider);

                        //float yOriginal = blockSlot.transform.position.y;
                        float yOriginal = tileSlot.transform.position.y;

                        DOVirtual.DelayedCall(TIME_DISTANCE_MOVE + x, () =>
                        {
                            blockSlot.transform.DOMoveY(yOriginal - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                .OnComplete(() =>
                                {
                                    blockSlot.transform.DOMoveY(yOriginal, TIME_DISTANCE_MOVE).SetEase(ease);

                                });
                        });
                        x += TIME_DISTANCE_MOVE;
                    }

                    DOVirtual.DelayedCall(x + TIME_DISTANCE_MOVE * 2, () =>
                    {
                        foreach (BoxCollider2D box in listBox)
                        {
                            box.enabled = true;
                        }
                    });

                    VibratePeek();
                    canMove = false;
                }
                else
                {
                    CheckChildrenOfRotation(this);

                    GameObject moveBlock = this.container;

                    if (isSaw)
                    {
                        moveBlock.transform.SetParent(transform.root);
                        int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
                        GameManager.Instance.currentGrid.listItem.RemoveAt(index);
                    }

                    else
                    {
                        moveBlock.transform.SetParent(targetTileSlot.transform);
                    }

                    var floatEnd = gridManager.GetTileSlot(posX, colPos).transform.position.y;

                    duration = Vector2.Distance(this.transform.position, new Vector2(this.transform.position.x, floatEnd)) / speed;

                    durationCheck = duration;

                    TileSlot target = gridManager.GetTileSlot(posX, colPos).GetComponent<TileSlot>();
                    target.boxTileSlot.enabled = false;

                    float scale = moveBlock.transform.localScale.x;

                    moveBlock.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            moveBlock.transform.DOScale(scale, timeScaleBlock)
                                .OnComplete(() =>
                                {
                                    TileSlot beside = gridManager.GetTileSlot(posX + 1, colPos).GetComponent<TileSlot>();

                                    if (beside.gameObject.transform.GetChild(0).transform.childCount != 0)
                                    {

                                        moveBlock.transform.DOMoveY(floatEnd, duration).SetEase(ease)
                                       .OnComplete(() =>
                                       {

                                           BoxCollider2D besideBox = beside.gameObject.GetComponent<BoxCollider2D>();

                                           float yOriginal = moveBlock.transform.position.y;
                                           float yOriginalBeside = beside.transform.position.y;

                                           if (beside.transform.childCount != 0)
                                           {

                                               moveBlock.transform.DOMoveY(yOriginal - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                                   .OnComplete(() =>
                                                   {
                                                       moveBlock.transform.DOMoveY(yOriginal, TIME_DISTANCE_MOVE).SetEase(ease)
                                                           .OnComplete(() =>
                                                           {
                                                               target.boxTileSlot.enabled = true;
                                                           });
                                                   });

                                               besideBox.enabled = false;

                                               beside.transform.DOMoveY(yOriginalBeside - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                               .OnComplete(() =>
                                               {
                                                   beside.transform.DOMoveY(yOriginalBeside, TIME_DISTANCE_MOVE).SetEase(ease)
                                                       .OnComplete(() =>
                                                       {
                                                           besideBox.enabled = true;
                                                       });
                                               });
                                           }
                                           else
                                           {
                                               target.boxTileSlot.enabled = true;
                                           }

                                       });
                                    }

                                    else
                                    {
                                        moveBlock.transform.DOMoveY(floatEnd, duration).SetEase(ease);
                                    }

                                    if (isSaw)
                                    {

                                        if (isNextTo)
                                        {
                                            AudioManager.Instance.PlaySFX1("Break");


                                            Destroy(moveBlock.gameObject);

                                            GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                            breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                            ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                            float timeBreak = main.duration;

                                            DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                            {
                                                Destroy(breakPre.gameObject);
                                            });

                                        }

                                        else
                                        {
                                            DOVirtual.DelayedCall(duration, () =>
                                            {
                                                AudioManager.Instance.PlaySFX1("Break");

                                                Destroy(moveBlock.gameObject);

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });


                                            });

                                        }

                                    }

                                    if (isBoom)
                                    {
                                        if (isNextTo)
                                        {
                                            AudioManager.Instance.PlaySFX1("Explosion");

                                            GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                            breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                            ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                            float timeBreak = main.duration;

                                            DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                            {
                                                Destroy(breakPre.gameObject);
                                            });

                                            GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                            explosionPre.transform.position = colorTileSlot.transform.position;

                                            DOVirtual.DelayedCall(1f, () =>
                                            {
                                                Destroy(explosionPre.gameObject);
                                            });

                                            Explosion(posX + 1, colPos);
                                        }

                                        else
                                        {
                                            DOVirtual.DelayedCall(duration, () =>
                                            {
                                                AudioManager.Instance.PlaySFX1("Explosion");

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });

                                                GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                                explosionPre.transform.position = colorTileSlot.transform.position;
                                                Explosion(posX + 1, colPos);

                                                DOVirtual.DelayedCall(1f, () =>
                                                {
                                                    Destroy(explosionPre.gameObject);

                                                });
                                            });
                                        }
                                    }
                                });
                        });



                }
            }


        }
    }

    private void MoveLeftDirection()
    {
        bool hasItemLeft = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;
        var duration = 0f;

        int posY = -1;

        if (colPos == 0)
        {
            hasItemLeft = false;
        }
        else
        {
            for (int i = colPos - 1; i >= 0; i--)
            {
                GameObject prefab = gridManager.GetTileSlot(rowPos, i);

                if (prefab.transform.childCount == 0)
                {
                    hasItemLeft = false;
                    posY = i;
                }

                else
                {
                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == colPos - 1)
                    {
                        posY = i + 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemLeft = true;
                    break;
                }

            }
        }

        if (!hasItemLeft)
        {
            CheckChildrenOfRotation(this);

            canMove = true;

            this.container.transform.SetParent(transform.root);

            int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
            GameManager.Instance.currentGrid.listItem.RemoveAt(index);

            var floatEnd = (Screen.width + (2.56f * Camera.main.orthographicSize) * 2f) / CameraExtension.PixelsPerUnit(Camera.main);

            duration = Vector2.Distance(this.transform.position, new Vector2(-floatEnd, this.transform.position.y)) / speed;

            durationCheck += duration;

            GameObject away = this.container;

            float scale = away.transform.localScale.x;

            away.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                .OnComplete(() =>
                {
                    away.transform.DOScale(scale, timeScaleBlock)
                    .OnComplete(() =>
                     {
                         away.transform.DOMoveX(-floatEnd, duration, false).SetEase(ease).OnComplete(() =>
                         {

                             Destroy(away.gameObject);
                         });
                     });
                });


        }
        else
        {
            if (posY != -1)
            {
                List<BoxCollider2D> listBox = new List<BoxCollider2D>();
                GameObject targetTileSlot = gridManager.GetTileSlot(rowPos, posY);
                GameObject colorTileSlot = gridManager.GetTileSlot(rowPos, posY - 1);

                if (isBoom)
                {
                    haveBoom = true;
                }

                if (isNextTo && !isBoom && !isSaw)
                {
                    SpriteRenderer colorTileSlotSprite = colorTileSlot.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    colorTileSlotSprite.color = new Color(1, 0, 0, 1);

                    DOVirtual.DelayedCall(TIME_COLOR, () =>
                    {
                        colorTileSlotSprite.color = new Color(1, 1, 1, 1);
                    });

                    GameObject targetBlockSlot = targetTileSlot.transform.GetChild(0).gameObject;
                    BoxCollider2D boxCollider = targetTileSlot.GetComponent<BoxCollider2D>();

                    boxCollider.enabled = false;
                    listBox.Add(boxCollider);

                    float scale = targetBlockSlot.transform.localScale.x;

                    targetBlockSlot.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            targetBlockSlot.transform.DOScale(scale, timeScaleBlock)
                            .OnComplete(() =>
                            {
                                float y1Original = targetTileSlot.transform.position.x;


                                targetBlockSlot.transform.DOMoveX(y1Original - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                    .OnComplete(() =>
                                    {
                                        targetBlockSlot.transform.DOMoveX(y1Original, TIME_DISTANCE_MOVE).SetEase(ease);

                                    });
                            });
                        });

                    float x = 0;
                    for (int i = posY - 1; i >= 0; i--)
                    {
                        if (gridManager.GetTileSlot(rowPos, i).transform.childCount == 0)
                        {
                            break;
                        }

                        if (gridManager.GetTileSlot(rowPos, i).transform.GetChild(0).transform.childCount == 0)
                        {
                            continue;
                        }

                        GameObject tileSlot = gridManager.GetTileSlot(rowPos, i);
                        GameObject blockSlot = tileSlot.transform.GetChild(0).gameObject;
                        BoxCollider2D boxColorCollider = tileSlot.GetComponent<BoxCollider2D>();

                        boxColorCollider.enabled = false;
                        listBox.Add(boxColorCollider);

                        //float yOriginal = blockSlot.transform.position.x;
                        float yOriginal = tileSlot.transform.position.x;

                        DOVirtual.DelayedCall(TIME_DISTANCE_MOVE + x, () =>
                        {
                            blockSlot.transform.DOMoveX(yOriginal - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                .OnComplete(() =>
                                {
                                    blockSlot.transform.DOMoveX(yOriginal, TIME_DISTANCE_MOVE).SetEase(ease);
                                });
                        });
                        x += TIME_DISTANCE_MOVE;
                    }

                    DOVirtual.DelayedCall(x + TIME_DISTANCE_MOVE * 2, () =>
                    {
                        foreach (BoxCollider2D box in listBox)
                        {
                            box.enabled = true;
                        }
                    });

                    VibratePeek();
                    canMove = false;
                }

                else
                {
                    CheckChildrenOfRotation(this);

                    GameObject moveBlock = this.container;

                    if (isSaw)
                    {
                        moveBlock.transform.SetParent(transform.root);
                        int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
                        GameManager.Instance.currentGrid.listItem.RemoveAt(index);
                    }
                    else
                    {
                        moveBlock.transform.SetParent(targetTileSlot.transform);

                    }

                    var floatEnd = gridManager.GetTileSlot(rowPos, posY).transform.position.x;

                    duration = Vector2.Distance(this.transform.position, new Vector2(floatEnd, this.transform.position.y)) / speed;

                    TileSlot target = gridManager.GetTileSlot(rowPos, posY).GetComponent<TileSlot>();
                    target.boxTileSlot.enabled = false;

                    float scale = moveBlock.transform.localScale.x;

                    moveBlock.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            moveBlock.transform.DOScale(scale, timeScaleBlock)
                                .OnComplete(() =>
                                {
                                    TileSlot beside = gridManager.GetTileSlot(rowPos, posY - 1).GetComponent<TileSlot>();

                                    if (beside.gameObject.transform.GetChild(0).transform.childCount != 0)
                                    {
                                        moveBlock.transform.DOMoveX(floatEnd, duration).SetEase(ease)
                                       .OnComplete(() =>
                                       {

                                           BoxCollider2D besideBox = beside.gameObject.GetComponent<BoxCollider2D>();

                                           float xOriginal = moveBlock.transform.position.x;
                                           float xOriginalBeside = beside.transform.position.x;

                                           if (beside.transform.childCount != 0)
                                           {

                                               moveBlock.transform.DOMoveX(xOriginal - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                                   .OnComplete(() =>
                                                   {
                                                       moveBlock.transform.DOMoveX(xOriginal, TIME_DISTANCE_MOVE).SetEase(ease)
                                                           .OnComplete(() =>
                                                           {
                                                               target.boxTileSlot.enabled = true;
                                                           });
                                                   });

                                               besideBox.enabled = false;

                                               beside.transform.DOMoveX(xOriginalBeside - DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                               .OnComplete(() =>
                                               {
                                                   beside.transform.DOMoveX(xOriginalBeside, TIME_DISTANCE_MOVE).SetEase(ease)
                                                       .OnComplete(() =>
                                                       {
                                                           besideBox.enabled = true;
                                                       });
                                               });
                                           }
                                           else
                                           {
                                               target.boxTileSlot.enabled = true;
                                           }

                                       });
                                    }

                                    else
                                    {
                                        moveBlock.transform.DOMoveX(floatEnd, duration).SetEase(ease);
                                    }

                                    if (isSaw)
                                    {

                                        if (isNextTo)
                                        {

                                            AudioManager.Instance.PlaySFX1("Break");

                                            Destroy(moveBlock.gameObject);

                                            GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                            breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                            ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                            float timeBreak = main.duration;

                                            DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                            {
                                                Destroy(breakPre.gameObject);
                                            });

                                        }

                                        else
                                        {

                                            DOVirtual.DelayedCall(duration, () =>
                                            {
                                                AudioManager.Instance.PlaySFX1("Break");

                                                Destroy(moveBlock.gameObject);

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });
                                            });


                                        }

                                    }
                                    if (isBoom)
                                    {
                                        if (isNextTo)
                                        {
                                            AudioManager.Instance.PlaySFX1("Explosion");

                                            GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                            breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                            ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                            float timeBreak = main.duration;

                                            DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                            {
                                                Destroy(breakPre.gameObject);
                                            });

                                            GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                            explosionPre.transform.position = colorTileSlot.transform.position;

                                            DOVirtual.DelayedCall(1f, () =>
                                            {
                                                Destroy(explosionPre.gameObject);

                                                target.boxTileSlot.enabled = true;
                                            });

                                            Explosion(rowPos, posY - 1);
                                        }

                                        else
                                        {
                                            DOVirtual.DelayedCall(duration, () =>
                                            {
                                                AudioManager.Instance.PlaySFX1("Explosion");

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });

                                                GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                                explosionPre.transform.position = colorTileSlot.transform.position;
                                                Explosion(rowPos, posY - 1);

                                                DOVirtual.DelayedCall(1f, () =>
                                                {
                                                    Destroy(explosionPre.gameObject);
                                                    target.boxTileSlot.enabled = true;

                                                });
                                            });
                                        }
                                    }
                                });
                        });



                }
            }


        }
    }

    private void MoveRightDirection()
    {
        bool hasItemRight = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;
        var duration = 0f;

        int posY = -1;

        if (colPos == gridManager.cols - 1)
        {
            hasItemRight = false;
        }

        else
        {
            for (int i = colPos + 1; i < gridManager.cols; i++)
            {
                GameObject prefab = gridManager.GetTileSlot(rowPos, i);

                if (prefab.transform.childCount == 0)
                {
                    hasItemRight = false;
                    posY = i;
                }

                else
                {

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == colPos + 1)
                    {
                        posY = i - 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemRight = true;
                    break;
                }

            }
        }

        if (!hasItemRight)
        {

            CheckChildrenOfRotation(this);


            canMove = true;

            int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
            GameManager.Instance.currentGrid.listItem.RemoveAt(index);

            this.container.transform.SetParent(transform.root);

            var floatEnd = (Screen.width + (2.56f * Camera.main.orthographicSize) * 2f) / CameraExtension.PixelsPerUnit(Camera.main);
            duration = Vector2.Distance(this.transform.position, new Vector2(floatEnd, this.transform.position.y)) / speed;

            durationCheck = duration;

            GameObject away = this.container;


            float scale = away.transform.localScale.x;

            away.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                .OnComplete(() =>
                {
                    away.transform.DOScale(scale, timeScaleBlock)
                    .OnComplete(() =>
                    {
                        away.transform.DOMoveX(floatEnd, duration, false).SetEase(ease).OnComplete(() =>
                        {

                            Destroy(away.gameObject);
                        });
                    });
                });

        }

        else
        {
            if (posY != -1)
            {
                List<BoxCollider2D> listBox = new List<BoxCollider2D>();
                GameObject targetTileSlot = gridManager.GetTileSlot(rowPos, posY);
                GameObject colorTileSlot = gridManager.GetTileSlot(rowPos, posY + 1);

                if (isBoom)
                {
                    haveBoom = true;
                }

                if (isNextTo && !isBoom && !isSaw)
                {
                    SpriteRenderer colorTileSlotSprite = colorTileSlot.transform.GetChild(0).GetComponent<SpriteRenderer>();
                    colorTileSlotSprite.color = new Color(1, 0, 0, 1);

                    DOVirtual.DelayedCall(TIME_COLOR, () =>
                    {
                        colorTileSlotSprite.color = new Color(1, 1, 1, 1);
                    });

                    GameObject targetBlockSlot = targetTileSlot.transform.GetChild(0).gameObject;
                    BoxCollider2D boxCollider = targetTileSlot.GetComponent<BoxCollider2D>();

                    boxCollider.enabled = false;
                    listBox.Add(boxCollider);

                    float scale = targetBlockSlot.transform.localScale.x;

                    targetBlockSlot.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            targetBlockSlot.transform.DOScale(scale, timeScaleBlock)
                            .OnComplete(() =>
                            {
                                float y1Original = targetTileSlot.transform.position.x;

                                targetBlockSlot.transform.DOMoveX(y1Original + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                    .OnComplete(() =>
                                    {
                                        targetBlockSlot.transform.DOMoveX(y1Original, TIME_DISTANCE_MOVE).SetEase(ease);
                                    });
                            });
                        });


                    float x = 0;
                    for (int i = posY + 1; i < gridManager.cols; i++)
                    {

                        if (gridManager.GetTileSlot(rowPos, i).transform.childCount == 0)
                        {
                            break;
                        }

                        if (gridManager.GetTileSlot(rowPos, i).transform.GetChild(0).transform.childCount == 0)
                        {
                            continue;
                        }

                        GameObject tileSlot = gridManager.GetTileSlot(rowPos, i);
                        GameObject blockSlot = tileSlot.transform.GetChild(0).gameObject;
                        BoxCollider2D boxColorCollider = tileSlot.GetComponent<BoxCollider2D>();

                        boxColorCollider.enabled = false;
                        listBox.Add(boxColorCollider);


                        //float yOriginal = blockSlot.transform.position.x;
                        float yOriginal = tileSlot.transform.position.x;

                        DOVirtual.DelayedCall(TIME_DISTANCE_MOVE + x, () =>
                        {
                            blockSlot.transform.DOMoveX(yOriginal + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                .OnComplete(() =>
                                {
                                    blockSlot.transform.DOMoveX(yOriginal, TIME_DISTANCE_MOVE).SetEase(ease);
                                });
                        });
                        x += TIME_DISTANCE_MOVE;
                    }

                    DOVirtual.DelayedCall(x + TIME_DISTANCE_MOVE * 2, () =>
                    {
                        foreach (BoxCollider2D box in listBox)
                        {
                            box.enabled = true;
                        }
                    });

                    VibratePeek();
                    canMove = false;
                }

                else
                {

                    CheckChildrenOfRotation(this);

                    GameObject moveBlock = this.container;

                    if (isSaw)
                    {
                        moveBlock.transform.SetParent(transform.root);

                        int index = GameManager.Instance.currentGrid.listItem.IndexOf(this.item.gameObject);
                        GameManager.Instance.currentGrid.listItem.RemoveAt(index);

                    }
                    else
                    {
                        moveBlock.transform.SetParent(targetTileSlot.transform);
                    }

                    var floatEnd = gridManager.GetTileSlot(rowPos, posY).transform.position.x;
                    duration = Vector2.Distance(this.transform.position, new Vector2(floatEnd, this.transform.position.y)) / speed;

                    durationCheck = duration;

                    Debug.Log("DurationCheckIn: " + durationCheck);

                    TileSlot target = gridManager.GetTileSlot(rowPos, posY).GetComponent<TileSlot>();
                    target.boxTileSlot.enabled = false;

                    float scale = moveBlock.transform.localScale.x;

                    moveBlock.transform.DOScale(scale * targetScaleBlock, timeScaleBlock)
                        .OnComplete(() =>
                        {
                            moveBlock.transform.DOScale(scale, timeScaleBlock)
                                .OnComplete(() =>
                                {
                                    TileSlot beside = gridManager.GetTileSlot(rowPos, posY + 1).GetComponent<TileSlot>();

                                    if (beside.gameObject.transform.GetChild(0).transform.childCount != 0)
                                    {
                                        moveBlock.transform.DOMoveX(floatEnd, duration).SetEase(ease)
                                       .OnComplete(() =>
                                       {

                                           BoxCollider2D besideBox = beside.gameObject.GetComponent<BoxCollider2D>();

                                           float xOriginal = moveBlock.transform.position.x;
                                           float xOriginalBeside = beside.transform.position.x;

                                           if (beside.transform.childCount != 0)
                                           {

                                               moveBlock.transform.DOMoveX(xOriginal + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                                   .OnComplete(() =>
                                                   {
                                                       moveBlock.transform.DOMoveX(xOriginal, TIME_DISTANCE_MOVE).SetEase(ease)
                                                           .OnComplete(() =>
                                                           {
                                                               target.boxTileSlot.enabled = true;
                                                           });
                                                   });

                                               besideBox.enabled = false;

                                               beside.transform.DOMoveX(xOriginalBeside + DISTANCE_MOVE, TIME_DISTANCE_MOVE).SetEase(ease)
                                               .OnComplete(() =>
                                               {
                                                   beside.transform.DOMoveX(xOriginalBeside, TIME_DISTANCE_MOVE).SetEase(ease)
                                                       .OnComplete(() =>
                                                       {
                                                           besideBox.enabled = true;
                                                       });
                                               });
                                           }
                                           else
                                           {
                                               target.boxTileSlot.enabled = true;
                                           }

                                       });
                                    }

                                    else
                                    {
                                        moveBlock.transform.DOMoveX(floatEnd, duration).SetEase(ease);
                                    }

                                    if (isSaw)
                                    {

                                        if (isNextTo)
                                        {
                                            AudioManager.Instance.PlaySFX1("Break");

                                            Destroy(moveBlock.gameObject);

                                            GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                            breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                            ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                            float timeBreak = main.duration;

                                            DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                            {
                                                Destroy(breakPre.gameObject);
                                            });


                                        }
                                        else
                                        {

                                            DOVirtual.DelayedCall(duration, () =>
                                            {
                                                AudioManager.Instance.PlaySFX1("Break");

                                                Destroy(moveBlock.gameObject);

                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });

                                            });


                                        }

                                    }
                                    if (isBoom)
                                    {
                                        if (isNextTo)
                                        {
                                            AudioManager.Instance.PlaySFX1("Explosion");

                                            GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                            breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                            ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                            float timeBreak = main.duration;

                                            DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                            {
                                                Destroy(breakPre.gameObject);
                                            });

                                            GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                            explosionPre.transform.position = colorTileSlot.transform.position;

                                            DOVirtual.DelayedCall(1f, () =>
                                            {
                                                Destroy(explosionPre.gameObject);
                                            });

                                            Explosion(rowPos, posY + 1);
                                        }

                                        else
                                        {
                                            AudioManager.Instance.PlaySFX1("Explosion");

                                            DOVirtual.DelayedCall(duration, () =>
                                            {
                                                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                                                breakPre.transform.position = new Vector3(targetTileSlot.transform.position.x, targetTileSlot.transform.position.y, targetTileSlot.transform.position.z);

                                                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                                                float timeBreak = main.duration;

                                                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                                                {
                                                    Destroy(breakPre.gameObject);
                                                });

                                                GameObject explosionPre = Instantiate(GameManager.Instance.explosionPrefab);
                                                explosionPre.transform.position = colorTileSlot.transform.position;
                                                Explosion(rowPos, posY + 1);

                                                DOVirtual.DelayedCall(1f, () =>
                                                {
                                                    Destroy(explosionPre.gameObject);

                                                });
                                            });

                                        }
                                    }
                                });
                        });


                }

            }
        }
    }

    public bool CheckUpDirection()
    {
        if (this.GetComponentInChildren<LineRenderer>() != null)
        {
            return true;
        }

        bool hasItemAbove = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;

        int posX = -1;

        if (rowPos == 0)
        {
            hasItemAbove = false;
            return true;
        }
        else
        {
            for (int i = rowPos - 1; i >= 0; i--)
            {

                GameObject prefab = gridManager.GetTileSlot(i, colPos);

                if (prefab.transform.childCount == 0)
                {
                    hasItemAbove = false;
                    posX = i;
                }

                else
                {

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().type == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == rowPos - 1)
                    {
                        posX = i + 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().type == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemAbove = true;
                    break;
                }

            }

            if (!hasItemAbove)
            {
                return true;
            }

            if (posX != -1)
            {
                if (!isNextTo)
                {
                    return true;
                }

                else
                {
                    if (isSaw || isBoom)
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    public bool CheckDownDirection()
    {
        if (this.GetComponentInChildren<LineRenderer>() != null)
        {
            return true;
        }

        bool hasItemUnder = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;

        int posX = -1;

        if (rowPos == gridManager.rows - 1)
        {
            hasItemUnder = false;
            return true;
        }
        else
        {
            for (int i = rowPos + 1; i < gridManager.rows; i++)
            {
                GameObject prefab = gridManager.GetTileSlot(i, colPos);

                if (prefab.transform.childCount == 0)
                {
                    hasItemUnder = false;
                    posX = i;
                }

                else
                {
                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == rowPos + 1)
                    {
                        posX = i - 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemUnder = true;
                    break;
                }

            }
        }
        if (!hasItemUnder)
        {
            return true;
        }

        if (posX != -1)
        {
            if (!isNextTo)
            {
                return true;
            }

            else
            {
                if (isSaw || isBoom)
                {
                    return true;
                }
            }

        }
        return false;
    }

    public bool CheckLeftDirection()
    {
        if (this.GetComponentInChildren<LineRenderer>() != null)
        {
            return true;
        }

        bool hasItemLeft = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;

        int posY = -1;

        if (colPos == 0)
        {
            hasItemLeft = false;
            return true;
        }
        else
        {
            for (int i = colPos - 1; i >= 0; i--)
            {
                GameObject prefab = gridManager.GetTileSlot(rowPos, i);

                if (prefab.transform.childCount == 0)
                {
                    hasItemLeft = false;
                    posY = i;
                }

                else
                {
                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == colPos - 1)
                    {
                        posY = i + 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemLeft = true;
                    break;
                }

            }
        }

        if (!hasItemLeft)
        {
            return true;
        }

        if (posY != -1)
        {
            if (!isNextTo)
            {
                return true;
            }

            else
            {
                if (isSaw || isBoom)
                {
                    return true;
                }
            }

        }
        return false;
    }

    public bool CheckRightDirection()
    {
        if (this.GetComponentInChildren<LineRenderer>() != null)
        {
            return true;
        }

        bool hasItemRight = false;
        bool isSaw = false;
        bool isNextTo = false;
        bool isBoom = false;

        int posY = -1;

        if (colPos == gridManager.cols - 1)
        {
            hasItemRight = false;
            return true;
        }

        else
        {
            for (int i = colPos + 1; i < gridManager.cols; i++)
            {
                GameObject prefab = gridManager.GetTileSlot(rowPos, i);

                if (prefab.transform.childCount == 0)
                {
                    hasItemRight = false;
                    posY = i;
                }

                else
                {

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.saw)
                    {
                        isSaw = true;

                    }

                    if (i == colPos + 1)
                    {
                        posY = i - 1;
                        isNextTo = true;
                    }

                    if (prefab.transform.GetChild(0).GetComponentInChildren<Item>().GetNameSprite() == item.boom)
                    {
                        isBoom = true;
                    }

                    hasItemRight = true;
                    break;
                }

            }
        }
        if (!hasItemRight)
        {
            return true;
        }

        if (posY != -1)
        {
            if (!isNextTo)
            {
                return true;
            }

            else
            {
                if (isSaw || isBoom)
                {
                    return true;
                }
            }

        }
        return false;
    }

    private void Explosion(int i, int j)
    {
        for (int x = -1; x <= 1; x++)
        {

            for (int y = -1; y <= 1; y++)
            {
                int posX = i + x;
                int posY = j + y;

                if (posX < 0 || posY < 0 || posX >= gridManager.rows || posY >= gridManager.cols)
                {
                    continue;
                }

                GameObject prefab = gridManager.GetTileSlot(posX, posY);

                if (prefab.transform.childCount != 0 && prefab.GetComponentInChildren<Item>().type != "Rotate")
                {
                    CheckChildrenOfRotation(prefab.GetComponent<TileSlot>());

                    if (prefab.transform.GetChild(0).childCount != 0)
                    {
                        GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                        breakPre.transform.position = new Vector3(prefab.transform.position.x, prefab.transform.position.y, prefab.transform.position.z);

                        ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                        float timeBreak = main.duration;

                        DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                        {
                            Destroy(breakPre.gameObject);
                        });

                        GameManager.Instance.currentGrid.listItem.RemoveAt(GameManager.Instance.currentGrid.listItem.IndexOf(prefab.transform.GetChild(0).GetComponentInChildren<Item>().gameObject));
                    }

                    //else
                    //{
                    //    GameManager.Instance.listItem.RemoveAt(GameManager.Instance.listItem.IndexOf(prefab.transform.GetChild(0).GetComponentInChildren<Item>().gameObject));
                    //}


                    DOVirtual.DelayedCall(0f, () =>
                    {
                        foreach (Transform transform in prefab.transform)
                        {
                            if (transform.GetComponentInChildren<TextMeshProUGUI>() != null)
                            {
                                transform.GetComponentInChildren<TextMeshProUGUI>().text = "";
                            }

                            Destroy(transform.gameObject);
                        }
                    });


                }
            }
        }
        bool isOver = GameManager.Instance.currentGrid.CheckLoseGame();

        DOVirtual.DelayedCall(0.1f, () =>
        {

            if (isOver == true)
            {
                GameManager.Instance.state = GameManager.State.Pause;

                GridManager gridCurrent = GameManager.Instance.currentGrid;

                float time = 0f;

                float duration;
                if (gridCurrent.listItem.Count != 0)
                {
                    duration = GameManager.Instance.timeCountDown;

                    for (int i = 0; i < gridCurrent.listItem.Count; i++)
                    {
                        SpriteRenderer spriteRenderer = gridCurrent.listItem[i].transform.parent.GetComponent<SpriteRenderer>();

                        DOVirtual.DelayedCall(time + duration, () =>
                        {
                            spriteRenderer.DOColor(new Color(1, 0, 0, 1), duration * 2)
                                .OnComplete(() =>
                                {
                                    spriteRenderer.DOColor(new Color(1, 1, 1, 1), duration * 2);
                                });
                        });

                        time += duration;

                    }

                }

                else
                {
                    duration = 0;
                }

                DOVirtual.DelayedCall(time + duration * 2, () =>
                {
                    AudioManager.Instance.sfxSource2.pitch = 1;
                    GameManager.Instance.combo = 0;
                    PopupController.Instance.TurnOnPopupLose();
                });
            }
        });

        if (isOver == false)
        {
            if (GameManager.Instance.currentGrid.listItem.Count == 0)
            {

                GameManager.Instance.state = GameManager.State.Pause;

                int index = GameManager.Instance.currentIndexGrid;

                if (index == GameManager.Instance.listGrid.Count - 1)
                {
                    DOVirtual.DelayedCall(TIME_WAIT_WIN, () =>
                    {
                        AudioManager.Instance.sfxSource2.pitch = 1;
                        GameManager.Instance.combo = 0;

                        PopupController.Instance.TurnOnPopupWin();
                    });
                    return;
                }

                DOVirtual.DelayedCall(TIME_WAIT_FADE_GRID, () =>
                {
                    foreach (SpriteRenderer spritePrefab in GameManager.Instance.listGrid[index].GetComponentsInChildren<SpriteRenderer>())
                    {
                        spritePrefab.DOFade(0f, TIME_FADE_GRID)
                            .OnComplete(() =>
                            {
                                AudioManager.Instance.sfxSource2.pitch = 1;
                                GameManager.Instance.combo = 0;

                                GameManager.Instance.state = GameManager.State.Play;
                                GameManager.Instance.listGrid[index].gameObject.SetActive(false);
                            });
                    }

                });

                DOVirtual.DelayedCall(TIME_WAIT_FADE_GRID + TIME_WAIT_FADE_GRID, () =>
                {
                    GameManager.Instance.currentIndexGrid++;
                    GameManager.Instance.currentGrid = GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid];
                    Debug.Log("moveCount: " + GameManager.Instance.currentGrid.moveCount);
                    GameManager.Instance.moveText.text = GameManager.Instance.currentGrid.moveCount + " Moves";

                    foreach (GameObject slot in GameManager.Instance.currentGrid.slots)
                    {
                        slot.GetComponent<BoxCollider2D>().enabled = true;
                    }
                });


            }
        }
    }

    public int CheckRotate()
    {
        bool _90_up = false;
        bool _90_right = false;
        bool _90_down = false;
        bool _90_left = false;

        bool _180_up = false;
        bool _180_right = false;
        bool _180_down = false;
        bool _180_left = false;

        bool _270_up = false;
        bool _270_right = false;
        bool _270_down = false;
        bool _270_left = false;

        List<Item> upItems = dictionary["Up"];
        List<Item> rightItems = dictionary["Right"];
        List<Item> downItems = dictionary["Down"];
        List<Item> leftItems = dictionary["Left"];


        bool haveUp = upItems.Count == 0 ? false : true;
        bool haveRight = rightItems.Count == 0 ? false : true;
        bool haveDown = downItems.Count == 0 ? false : true;
        bool haveLeft = leftItems.Count == 0 ? false : true;




        if (haveUp)
        {
            bool check = true;

            foreach (Item item in upItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = this.rowPos - slot.GetComponent<TileSlot>().rowPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos + index);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (rightItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;
                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _90_up = true;
            }
        }

        else
        {
            _90_up = true;
        }

        if (haveRight)
        {
            bool check = true;

            foreach (Item item in rightItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = slot.GetComponent<TileSlot>().colPos - this.colPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos + index, this.colPos);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (downItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {

                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;


                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _90_right = true;
            }
        }

        else
        {
            _90_right = true;
        }

        if (haveDown)
        {
            bool check = true;

            foreach (Item item in downItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = slot.GetComponent<TileSlot>().rowPos - this.rowPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos - index);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (leftItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;


                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _90_down = true;
            }
        }

        else
        {
            _90_down = true;
        }

        if (haveLeft)
        {
            bool check = true;

            foreach (Item item in leftItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = this.colPos - slot.GetComponent<TileSlot>().colPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos - index, this.colPos);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (upItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;


                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _90_left = true;
            }
        }

        else
        {
            _90_left = true;
        }



        if (_90_up && _90_right && _90_down && _90_left)
        {
            Debug.Log("Can Rotate 90");
            return 90;
        }

        // ------------------------------------------------------------------- Check 180 degree -------------------------------------------------------

        if (haveUp)
        {
            bool check = true;

            foreach (Item item in upItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = this.rowPos - slot.GetComponent<TileSlot>().rowPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos + index, this.colPos);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (downItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _180_up = true;
            }
        }

        else
        {
            _180_up = true;
        }

        if (haveRight)
        {
            bool check = true;

            foreach (Item item in rightItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = slot.GetComponent<TileSlot>().colPos - this.colPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos - index);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (leftItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _180_right = true;
            }
        }

        else
        {
            _180_right = true;
        }

        if (haveDown)
        {
            bool check = true;

            foreach (Item item in downItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = slot.GetComponent<TileSlot>().rowPos - this.rowPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos - index, this.colPos);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (upItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _180_down = true;
            }
        }

        else
        {
            _180_down = true;
        }

        if (haveLeft)
        {
            bool check = true;

            foreach (Item item in leftItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = this.colPos - slot.GetComponent<TileSlot>().colPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos + index);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (rightItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _180_left = true;
            }
        }

        else
        {
            _180_left = true;
        }


        if (_180_up && _180_right && _180_down && _180_left)
        {
            Debug.Log("Can Rotate 180");
            return 180;
        }
        // ------------------------------------------------------------------- Check 270 degree ------------------------------------------------------------------------
        if (haveUp)
        {
            bool check = true;

            foreach (Item item in upItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = this.rowPos - slot.GetComponent<TileSlot>().rowPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos - index);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (leftItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _270_up = true;
            }
        }

        else
        {
            _270_up = true;
        }

        if (haveRight)
        {
            bool check = true;

            foreach (Item item in rightItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = slot.GetComponent<TileSlot>().colPos - this.colPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos - index, this.colPos);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (upItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _270_right = true;
            }
        }

        else
        {
            _270_right = true;
        }

        if (haveDown)
        {
            bool check = true;

            foreach (Item item in downItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = slot.GetComponent<TileSlot>().rowPos - this.rowPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos + index);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (rightItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _270_down = true;
            }
        }

        else
        {
            _270_down = true;
        }

        if (haveLeft)
        {
            bool check = true;

            foreach (Item item in leftItems)
            {
                if (item.type == item.saw)
                {
                    continue;
                }

                GameObject slot;

                if (item.isArrow())
                {
                    slot = item.transform.parent.parent.gameObject;
                }
                else
                {
                    slot = item.transform.parent.gameObject;
                }

                int index = this.colPos - slot.GetComponent<TileSlot>().colPos;

                GameObject targetSlot = gridManager.GetTileSlot(this.rowPos + index, this.colPos);

                if (targetSlot.GetComponentInChildren<Item>() != null)
                {
                    Item targetItem = targetSlot.GetComponentInChildren<Item>();

                    if (downItems.Contains(targetItem))
                    {
                        continue;
                    }

                    else
                    {
                        if (targetItem.type != item.saw)
                        {
                            GameManager.Instance.currentGrid.blockedTile = targetItem.transform.parent.gameObject;

                            check = false;
                            break;
                        }
                    }
                }

            }

            if (check)
            {
                _270_left = true;
            }
        }

        else
        {
            _270_left = true;
        }

        if (_270_up && _270_right && _270_down && _270_left)
        {
            Debug.Log("Can Rotate 270");
            return 270;
        }
        Debug.Log("haveUp: " + haveUp);
        Debug.Log("haveRight: " + haveRight);
        Debug.Log("haveDown: " + haveDown);
        Debug.Log("haveLeft: " + haveLeft);


        Debug.Log("_90_up: " + _90_up);
        Debug.Log("_90_right: " + _90_right);
        Debug.Log("_90_down: " + _90_down);
        Debug.Log("_90_left: " + _90_left);


        return 0;
    }

    private void DoRotate(int degree)
    {
        if (degree == 0)
        {

            if (GameManager.Instance.currentGrid.blockedTile != null)
            {
                Item targetItem = GameManager.Instance.currentGrid.blockedTile.transform.parent.GetComponentInChildren<Item>();

                if (targetItem.type != item.rotate)
                {
                    targetItem.transform.parent.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                    GameManager.Instance.state = GameManager.State.Pause;
                    VibratePeek();

                    DOVirtual.DelayedCall(GameManager.Instance.timeWaitBlockRotate, () =>
                    {
                        AudioManager.Instance.sfxSource2.pitch = 1;
                        GameManager.Instance.combo = 0;
                        GameManager.Instance.state = GameManager.State.Play;
                        targetItem.transform.parent.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                        GameManager.Instance.currentGrid.blockedTile = null;
                    });
                }
            }

            return;
        }

        GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid].checkRotation = true;

        List<Item> upItems = dictionary["Up"];
        List<Item> rightItems = dictionary["Right"];
        List<Item> downItems = dictionary["Down"];
        List<Item> leftItems = dictionary["Left"];


        bool haveUp = upItems.Count == 0 ? false : true;
        bool haveRight = rightItems.Count == 0 ? false : true;
        bool haveDown = downItems.Count == 0 ? false : true;
        bool haveLeft = leftItems.Count == 0 ? false : true;

        List<Item> upItemsTemp = new List<Item>();
        List<Item> rightItemsTemp = new List<Item>();
        List<Item> downItemsTemp = new List<Item>();
        List<Item> leftItemsTemp = new List<Item>();



        if (degree == 90)
        {
            if (haveUp)
            {
                foreach (Item item in upItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = this.rowPos - slot.GetComponent<TileSlot>().rowPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos + index);

                    rightItemsTemp.Add(item);


                    StartCoroutine(Rotate(itemRotate, 90, this.transform.position, targetSlot, rightItems));
                }
            }

            if (haveRight)
            {
                foreach (Item item in rightItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = slot.GetComponent<TileSlot>().colPos - this.colPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos + index, this.colPos);

                    downItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 90, this.transform.position, targetSlot, downItems));
                }
            }

            if (haveDown)
            {
                foreach (Item item in downItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = slot.GetComponent<TileSlot>().rowPos - this.rowPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos - index);

                    leftItemsTemp.Add(item);

                    Cover cover = this.GetComponentInChildren<Cover>();


                    StartCoroutine(Rotate(itemRotate, 90, this.transform.position, targetSlot, leftItems));

                }
            }

            if (haveLeft)
            {
                foreach (Item item in leftItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = this.colPos - slot.GetComponent<TileSlot>().colPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos - index, this.colPos);

                    upItemsTemp.Add(item);

                    Cover cover = this.GetComponentInChildren<Cover>();

                    StartCoroutine(Rotate(itemRotate, 90, this.transform.position, targetSlot, upItems));

                }
            }


            upItems.Clear();
            upItems.AddRange(upItemsTemp);

            rightItems.Clear();
            rightItems.AddRange(rightItemsTemp);

            downItems.Clear();
            downItems.AddRange(downItemsTemp);

            leftItems.Clear();
            leftItems.AddRange(leftItemsTemp);

            Debug.Log("Rotate 90 done");

            return;
        }
        // ------------------------------------------------------------------------------ Rotate 180 degree ----------------------------------------------------------------

        if (degree == 180)
        {
            if (haveUp)
            {
                foreach (Item item in upItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = this.rowPos - slot.GetComponent<TileSlot>().rowPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos + index, this.colPos);

                    downItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 180, this.transform.position, targetSlot, downItems));
                }
            }

            if (haveRight)
            {
                foreach (Item item in rightItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = slot.GetComponent<TileSlot>().colPos - this.colPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos - index);

                    leftItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 180, this.transform.position, targetSlot, leftItems));
                }
            }

            if (haveDown)
            {
                foreach (Item item in downItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = slot.GetComponent<TileSlot>().rowPos - this.rowPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos - index, this.colPos);

                    upItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 180, this.transform.position, targetSlot, upItems));
                }
            }

            if (haveLeft)
            {
                foreach (Item item in leftItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = this.colPos - slot.GetComponent<TileSlot>().colPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos + index);

                    rightItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 180, this.transform.position, targetSlot, rightItems));
                }
            }


            upItems.Clear();
            upItems.AddRange(upItemsTemp);

            rightItems.Clear();
            rightItems.AddRange(rightItemsTemp);

            downItems.Clear();
            downItems.AddRange(downItemsTemp);

            leftItems.Clear();
            leftItems.AddRange(leftItemsTemp);

            Debug.Log("Rotate 180 done");

            return;
        }

        // ------------------------------------------------------------------------------ Rotate 270 degree ----------------------------------------------------------------

        if (degree == 270)
        {
            if (haveUp)
            {
                foreach (Item item in upItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = this.rowPos - slot.GetComponent<TileSlot>().rowPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos - index);

                    leftItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 270, this.transform.position, targetSlot, leftItems));
                }
            }

            if (haveRight)
            {
                foreach (Item item in rightItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = slot.GetComponent<TileSlot>().colPos - this.colPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos - index, this.colPos);

                    upItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 270, this.transform.position, targetSlot, upItems));
                }
            }

            if (haveDown)
            {
                foreach (Item item in downItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = slot.GetComponent<TileSlot>().rowPos - this.rowPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos, this.colPos + index);

                    rightItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 270, this.transform.position, targetSlot, rightItems));
                }
            }

            if (haveLeft)
            {
                foreach (Item item in leftItems)
                {
                    GameObject slot;
                    GameObject itemRotate;

                    if (item.isArrow())
                    {
                        slot = item.transform.parent.parent.gameObject;
                        itemRotate = item.transform.parent.gameObject;
                    }
                    else
                    {
                        slot = item.transform.parent.gameObject;
                        itemRotate = item.gameObject;
                    }

                    int index = this.colPos - slot.GetComponent<TileSlot>().colPos;

                    GameObject targetSlot = gridManager.GetTileSlot(this.rowPos + index, this.colPos);

                    downItemsTemp.Add(item);

                    StartCoroutine(Rotate(itemRotate, 270, this.transform.position, targetSlot, downItems));
                }
            }


            upItems.Clear();
            upItems.AddRange(upItemsTemp);

            rightItems.Clear();
            rightItems.AddRange(rightItemsTemp);

            downItems.Clear();
            downItems.AddRange(downItemsTemp);

            leftItems.Clear();
            leftItems.AddRange(leftItemsTemp);

            Debug.Log("Rotate 270 done");

            return;
        }
    }

    private IEnumerator Rotate(GameObject block, float anglePoint, Vector3 centerPos, GameObject targetSlot, List<Item> listItems)
    {
        float angle = 0f;
        Vector3 offset = block.transform.position - centerPos;
        Debug.Log("Centerpos1: " + centerPos);

        GameManager.Instance.isRotating = true;
        float zOriginal = block.GetComponentInChildren<LineRenderer>().GetPosition(1).z;


        TrailRenderer tempTrailRender = new TrailRenderer();

        Cover cover = this.GetComponentInChildren<Cover>();

        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid].transform.position.z - 0.2f);

        if (block.GetComponentInChildren<TrailRenderer>() != null)
        {
            tempTrailRender = block.GetComponentInChildren<TrailRenderer>();
            tempTrailRender.enabled = false;
        }

        centerPos = new Vector3(centerPos.x, centerPos.y + GameManager.Instance.heightRotate, centerPos.z);

        Debug.Log("Centerpos2: " + centerPos);
        bool stop = false;


        cover.transform.DOMoveY(cover.transform.position.y + GameManager.Instance.heightRotate, GameManager.Instance.timeHeightRotate)
            .OnComplete(() =>
            {
                stop = true;
            });


        block.transform.DOMoveY(block.transform.position.y + GameManager.Instance.heightRotate, GameManager.Instance.timeHeightRotate);

        while (!stop)
        {
            block.GetComponentInChildren<LineRenderer>().SetPosition(0, new Vector3(block.transform.position.x, block.transform.position.y, zOriginal));
            block.GetComponentInChildren<LineRenderer>().SetPosition(1, new Vector3(cover.transform.position.x, cover.transform.position.y - 0.25f, zOriginal));
            yield return null;
        }


        while (angle < anglePoint)
        {

            Vector3 newPos = centerPos + Quaternion.Euler(0, 0, -angle) * offset;

            block.transform.position = new Vector3(newPos.x, newPos.y, GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid].transform.position.z - 0.2f);

            block.GetComponentInChildren<LineRenderer>().SetPosition(0, new Vector3(block.transform.position.x, block.transform.position.y, zOriginal));

            if (block.GetComponent<Item>() != null)
            {
                GameObject shadow = block.GetComponent<Item>().shadow;
                shadow.transform.position = new Vector3(block.transform.position.x, block.transform.position.y - 0.1f, shadow.transform.position.z);

            }

            angle += Time.deltaTime * speedRotate;

            yield return null;
        }

        Vector3 newPos1 = centerPos + Quaternion.Euler(0, 0, -anglePoint) * offset;

        block.transform.position = new Vector3(newPos1.x, newPos1.y, GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid].transform.position.z - 0.2f);

        block.GetComponentInChildren<LineRenderer>().SetPosition(0, block.transform.position);

        stop = false;

        DOVirtual.DelayedCall(0.1f, () =>
        {
            cover.transform.DOMoveY(cover.transform.position.y - GameManager.Instance.heightRotate, GameManager.Instance.timeHeightRotate)
                .OnComplete(() =>
                {
                    GameManager.Instance.isRotating = false;
                    stop = true;

                    DOVirtual.DelayedCall(0.01f, () =>
                    {
                        if (tempTrailRender != null)
                        {
                            tempTrailRender.enabled = true;
                        }
                    });

                });
            block.transform.DOMoveY(block.transform.position.y - GameManager.Instance.heightRotate, GameManager.Instance.timeHeightRotate);

        });


        while (!stop)
        {
            block.GetComponentInChildren<LineRenderer>().SetPosition(0, new Vector3(block.transform.position.x, block.transform.position.y, zOriginal));
            block.GetComponentInChildren<LineRenderer>().SetPosition(1, new Vector3(cover.transform.position.x, cover.transform.position.y - 0.25f, zOriginal));
            yield return null;
        }

        bool checkBlock = true;

        if (block.GetComponent<Item>() != null)
        {
            bool checkBreak = false;

            foreach (Transform transform in targetSlot.transform)
            {
                if (targetSlot.transform.childCount != 0)
                {
                    Item temp = targetSlot.GetComponentInChildren<Item>();
                    GameManager.Instance.currentGrid.listItem.Remove(temp.gameObject);
                    checkBreak = true;
                }

                Destroy(transform.gameObject);
            }

            if (checkBreak)
            {
                AudioManager.Instance.PlaySFX1("Break");

                GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                breakPre.transform.position = new Vector3(targetSlot.transform.position.x, targetSlot.transform.position.y, targetSlot.transform.position.z);

                ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                float timeBreak = main.duration;

                DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                {
                    Destroy(breakPre.gameObject);
                });
            }

        }

        else
        {
            if (targetSlot.GetComponentInChildren<Item>() != null)
            {
                Item item = targetSlot.GetComponentInChildren<Item>();

                if (!dictionary["Up"].Contains(item) && !dictionary["Right"].Contains(item) && !dictionary["Down"].Contains(item) && !dictionary["Left"].Contains(item))
                {
                    checkBlock = false;

                    Item temp = block.GetComponentInChildren<Item>();
                    GameManager.Instance.currentGrid.listItem.Remove(temp.gameObject);

                    DOVirtual.DelayedCall(0.05f, () =>
                    {
                        if (dictionary["Up"].Contains(temp))
                        {
                            dictionary["Up"].Remove(temp);
                        }

                        else if (dictionary["Right"].Contains(temp))
                        {
                            dictionary["Right"].Remove(temp);
                        }

                        else if (dictionary["Down"].Contains(temp))
                        {
                            dictionary["Down"].Remove(temp);
                        }

                        else if (dictionary["Left"].Contains(temp))
                        {
                            dictionary["Left"].Remove(temp);
                        }
                    });

                    AudioManager.Instance.PlaySFX1("Break");

                    GameObject breakPre = Instantiate(GameManager.Instance.currentGrid.breakPrefab);

                    breakPre.transform.position = new Vector3(block.transform.position.x, block.transform.position.y, block.transform.position.z);

                    ParticleSystem.MainModule main = breakPre.GetComponent<ParticleSystem>().main;

                    float timeBreak = main.duration;

                    DOVirtual.DelayedCall(timeBreak + 0.1f, () =>
                    {
                        Destroy(breakPre.gameObject);
                    });

                    Destroy(block.gameObject);

                }
            }
        }


        if (checkBlock)
        {
            block.transform.SetParent(targetSlot.transform);
            block.transform.localPosition = Vector3.zero;
            float z = block.GetComponentInChildren<LineRenderer>().GetPosition(1).z;

            Debug.Log("Z: " + z);
            block.GetComponentInChildren<LineRenderer>().SetPosition(0, new Vector3(block.transform.position.x, block.transform.position.y, z));
        }


        DOVirtual.DelayedCall(0.1f, () =>
        {
            bool isOver = GameManager.Instance.currentGrid.CheckLoseGame();

            if (isOver == true)
            {
                GameManager.Instance.state = GameManager.State.Pause;

                GridManager gridCurrent = GameManager.Instance.currentGrid;

                float time = 0f;

                float duration;
                if (gridCurrent.listItem.Count != 0)
                {
                    duration = GameManager.Instance.timeCountDown;

                    for (int i = 0; i < gridCurrent.listItem.Count; i++)
                    {
                        SpriteRenderer spriteRenderer = gridCurrent.listItem[i].transform.parent.GetComponent<SpriteRenderer>();

                        DOVirtual.DelayedCall(time + duration, () =>
                        {
                            spriteRenderer.DOColor(new Color(1, 0, 0, 1), duration * 2)
                                .OnComplete(() =>
                                {
                                    spriteRenderer.DOColor(new Color(1, 1, 1, 1), duration * 2);
                                });
                        });

                        time += duration;

                    }

                }

                else
                {
                    duration = 0;
                }

                DOVirtual.DelayedCall(time + duration * 2, () =>
                {
                    AudioManager.Instance.sfxSource2.pitch = 1;
                    GameManager.Instance.combo = 0;
                    PopupController.Instance.TurnOnPopupLose();
                });

            }

            else
            {
                if (GameManager.Instance.currentGrid.listItem.Count == 0)
                {

                    GameManager.Instance.state = GameManager.State.Pause;

                    int index = GameManager.Instance.currentIndexGrid;

                    if (index == GameManager.Instance.listGrid.Count - 1)
                    {
                        DOVirtual.DelayedCall(TIME_WAIT_WIN, () =>
                        {
                            AudioManager.Instance.sfxSource2.pitch = 1;
                            GameManager.Instance.combo = 0;

                            PopupController.Instance.TurnOnPopupWin();
                        });
                        return;
                    }

                    DOVirtual.DelayedCall(TIME_WAIT_FADE_GRID, () =>
                    {
                        foreach (SpriteRenderer spritePrefab in GameManager.Instance.listGrid[index].GetComponentsInChildren<SpriteRenderer>())
                        {
                            spritePrefab.DOFade(0f, TIME_FADE_GRID)
                                .OnComplete(() =>
                                {
                                    AudioManager.Instance.sfxSource2.pitch = 1;
                                    GameManager.Instance.combo = 0;

                                    GameManager.Instance.state = GameManager.State.Play;
                                    GameManager.Instance.listGrid[index].gameObject.SetActive(false);
                                });
                        }

                        GameManager.Instance.currentIndexGrid++;
                        GameManager.Instance.currentGrid = GameManager.Instance.listGrid[GameManager.Instance.currentIndexGrid];
                        Debug.Log("moveCount: " + GameManager.Instance.currentGrid.moveCount);
                        GameManager.Instance.moveText.text = GameManager.Instance.currentGrid.moveCount + " Moves";

                        foreach (GameObject slot in GameManager.Instance.currentGrid.slots)
                        {
                            slot.GetComponent<BoxCollider2D>().enabled = true;
                        }
                    });


                }
            }
        });


    }
}