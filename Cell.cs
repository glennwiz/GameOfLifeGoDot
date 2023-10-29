using Godot;

namespace GameOfLife;

public class Cell
{
    public bool IsAlive { get; set; }
    public Vector2 Position { get; set; }
    public Color Color { get; set; }
    public int LiveNeighbors { get; set; }
}