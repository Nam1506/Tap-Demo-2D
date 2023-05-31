using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public int level;
    public int maxMap;
    public List<Map> map;
}

public class Map
{
    public int rows, cols;
    public int move_count;
    public List<List<int>> grid;
    public List<List<int>> grid_number;
    public List<List<int>> grid_rotate;
}

public class TempData
{
    public string Name;
    public int LayerCount;
    public List<Grid> Grids;
}

public class Grid
{
    public int Width;
    public int Height;
    public int MovesLimit;
    public List<int> Cells;
    public List<int> Numbers;
    public List<Connection> Connections;
}

public class Connection
{
    public int Index;
    public List<int> Connects;
}
