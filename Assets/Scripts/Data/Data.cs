using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int level;
    public float maxCamera;
    public int maxMap;
    public List<Map> map;
    public int color;
}

public class Map
{
    public int rows, cols;
    public int move_count;
    public List<List<int>> grid;
    public List<List<int>> grid_number;
    public List<List<int>> grid_rotate;

}
