using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    public static BlockController Instance;

    public List<Sprite> skins;
    public int index;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        
    }

    public Sprite GetSkin()
    {
        return skins[index];
    }

}
