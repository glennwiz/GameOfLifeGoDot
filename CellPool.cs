using System.Collections.Generic;
using Godot;

namespace GameOfLife;
public class CellPool
{
    //th
    private Queue<Cell> availableCells = new Queue<Cell>();

    public CellPool(int initialSize)
    {
        for (int i = 0; i < initialSize; i++)
        {
            availableCells.Enqueue(CreateNewCell());
        }
    }

    private Cell CreateNewCell()
    {
        return new Cell();
    }

    public Cell GetCell()
    {
        if (availableCells.Count > 0)
            return availableCells.Dequeue();
        else
            return CreateNewCell();     
    }

    public void ReleaseCell(Cell cell)
    {
        ResetCell(cell);
        availableCells.Enqueue(cell);
    }

    private void ResetCell(Cell cell)
    {
        cell.IsAlive = false;
        cell.Position = Vector2.Zero;
        cell.Color = Color.Color8(0, 0, 0);
        cell.LiveNeighbors = 0;
        cell.State = "";
    }
}
