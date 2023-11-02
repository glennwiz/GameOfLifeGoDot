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
        foreach (var cells in grid.ListOfCellArrayStates)
        {
            // Clear all cells 
            for (var x = 0; x <cells.GetLength(0) ; x++)
            {
                for (var y = 0; y < cells.GetLength(1); y++)
                {
                    cells[x, y] = null;
                }
            }
        }
    }

    public static bool[,] MirrorMatrix(Cell[,] cellsInsideBox)
    {
        var mirroredCells = new Cell[cellsInsideBox.GetLength(0), cellsInsideBox.GetLength(1)];
        for (var x = 0; x < cellsInsideBox.GetLength(0); x++)
        {
            for (var y = 0; y < cellsInsideBox.GetLength(1); y++)
            {
                mirroredCells[x, y] = cellsInsideBox[cellsInsideBox.GetLength(0) - 1 - x, y];
            }
        }
        
        var retunedCells = new bool[mirroredCells.GetLength(0), mirroredCells.GetLength(1)];
        for (var x = 0; x < mirroredCells.GetLength(0); x++)
        {
            for (var y = 0; y < mirroredCells.GetLength(1); y++)
            {
                if (mirroredCells[x, y] == null) continue;
                retunedCells[x, y] = mirroredCells[x, y].IsAlive;
            }
        }
        
        return retunedCells;
    }
}