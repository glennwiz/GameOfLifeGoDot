using System.Collections.Generic;
using Godot;

namespace GameOfLife;

public class MatrixManipulation	
{
    public static bool[,] RotateMatrix90(bool[,] matrix)
    {
        var height = matrix.GetLength(0);
        var width = matrix.GetLength(1);
        var rotated = new bool[width, height];

        for (var i = 0; i < height; i++)
        {
            for (var j = 0; j < width; j++)
            {
                rotated[j, height - 1 - i] = matrix[i, j];
            }
        }

        return rotated;
    }
	
    public void ClearGrid(Grid grid)
    {
        foreach (var g in grid.GridCells)
        {
            // Clear all cells 
            for (var x = 0; x <g.GetLength(0) ; x++)
            {
                for (var y = 0; y < g.GetLength(1); y++)
                {
                    g[x, y] = new Cell
                    {
                        Color = Colors.Black,
                        IsAlive = false,
                        Position = new Vector2(x, y)
                    };
                }
            }
        }
    }
}