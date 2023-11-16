using System.Collections.Generic;
using Godot;

namespace GameOfLife;
public class CellPool
{
    //th
    private Queue<Cell> availableCells = new Queue<Cell>();

    public CellPool(int initialSize)
    {
        for (var i = 0; i < initialSize; i++)
        {
            availableCells.Enqueue(CreateNewCell());
        }
    }

    private Cell CreateNewCell()
    {
        return new Cell();
    }

    private bool RandomColors = true;
    
    int debugCounter=0;
    public Cell GetCell()
    {
        
        if (availableCells.Count > 0){
            var cell = availableCells.Dequeue();

            if (RandomColors)
            {
                
                cell.Color = Color.Color8((byte)GD.RandRange(0, 255), (byte)GD.RandRange(0, 255), (byte)GD.RandRange(0, 255));
          
                debugCounter++;
                GD.Print("Pick from pool " + debugCounter);
            }
            
            return cell;
        }
        else
        {
            var cell = CreateNewCell();
            if (RandomColors)
            {
                cell.Color = Color.Color8((byte)GD.RandRange(0, 255), (byte)GD.RandRange(0, 255), (byte)GD.RandRange(0, 255));
                debugCounter++;
                GD.Print("Create New Cell " + debugCounter);
            }
            return cell;
        }     
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