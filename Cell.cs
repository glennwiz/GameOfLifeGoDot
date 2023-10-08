using Godot;

public class Cell
{
    public bool IsAlive { get; set; }
    public Vector2 Position { get; set; }
    public Color Color { get; set; }
    public float TargetAlpha { get; set; }
    public float CurrentAlpha { get; set; }
    public int LiveNeighbors { get; set; }

    public void UpdateAlpha(double delta)
    {
        // Interpolate the alpha value towards the target alpha
        CurrentAlpha = Mathf.Lerp(CurrentAlpha, TargetAlpha, (float)delta);
        var color = Color;
        color.A = CurrentAlpha;
        
        Color = color;
    }
}