using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public static Item Instance;
    
    public SpriteRenderer spriteRenderer;

    public string type;

    public List<Item> itemRotate;

    public TileSlot rotateController;

    public GameObject shadow;

    public GameObject cover;

    // -----Config-----

    public string up = "Up";
    public string down = "Down";
    public string left = "Left";
    public string right = "Right";
    public string sand = "Sand";
    public string saw = "Saw";
    public string boom = "Boom";
    public string rotate = "Rotate";
    public string hidden = "Hidden";
    public float duration_rotate = 1f;


    private void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {

        if (this.transform.parent.name == "Tool")
        {
            return;
        }
        if(spriteRenderer == null)
        {
            type = "Boom";
            return;
        }

        type = GetNameSprite();


        if (this.GetNameSprite() == saw)
        {
            transform.DORotate(new Vector3(0f, 0f, 360f), duration_rotate, RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear)
         .      SetLoops(-1, LoopType.Incremental);
        }
    }

    public string GetNameSprite()
    {

        if(type == "Boom")
        {
            return boom;
        }

        return spriteRenderer.sprite.name;
    }

    public bool isArrow()
    {

        if(this.type == up || this.type == down || this.type == left || this.type == right)
        {
            return true;
        }

        return false;
    }

    public bool canConnect()
    {
        if (this.type == up || this.type == down || this.type == left || this.type == right || this.type == saw)
        {
            return true;
        }

        return false;
    }

    public string GetTypee()
    {
        return this.type;
    }

}
