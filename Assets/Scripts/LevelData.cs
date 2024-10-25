using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int obstacles;
    public int move_count;
    public List<string> grid;
}