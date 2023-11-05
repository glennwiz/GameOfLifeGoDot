using System;
using Godot;

namespace GameOfLife;

public partial class GameGridHandler : Node2D
{
    private readonly MatrixManipulation _matrixManipulation = new();

    private Grid _grid;
    private GridController _controller;
    private InputManager _inputManager;
    private PatternCreator _patternCreator;

    public override void _Ready()
    { 
        InitComponents();
        AddComponentsToScene();
        DebugPrintAllNodes(this);
    }

    public override void _Process(double delta)
    {
        if (_grid.IsPaused)
        {
            QueueRedraw();
            return;
        }

        _controller.GridProcess(delta);
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_grid.CurrentStateIndex < 0 || _grid.CurrentStateIndex >= _grid.ListOfCellArrayStates.Count)
        {
            return;
        }
            
        DrawCells();
        DrawDebugLinesIfEnabled();
    }

    private void DrawBox(int x, int y, Color color)
    {
        if (IsOutOfBounds(x, y))
        {
            GD.PrintErr("Coordinates out of bounds");
            return;
        }

        DrawRect(new Rect2(x * _grid.BoxSize, y * _grid.BoxSize, _grid.BoxSize, _grid.BoxSize), color);
    }

    private void InitComponents()
    {
        var gridSize = GetViewportRect().Size;

        _patternCreator = new PatternCreator();
        _grid = new Grid(_matrixManipulation, gridSize);
        _controller = new GridController(_grid);
        _inputManager = new InputManager(_grid);
    }

    private void AddComponentsToScene()
    {
        AddChild(_grid);
        AddChild(_controller);
        AddChild(_inputManager);
    }

    private bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || x >= _grid.GridWidth || y < 0 || y >= _grid.GridHeight;
    }

    private void DrawCells()
    {
        var currentGridState = _grid.ListOfCellArrayStates[_grid.CurrentStateIndex];
        int gridStateWidth = currentGridState.GetLength(0);
        int gridStateHeight = currentGridState.GetLength(1);
        int gridWidth = Math.Min(_grid.GridWidth, gridStateWidth);
        int gridHeight = Math.Min(_grid.GridHeight, gridStateHeight);

        Vector2 rectSize = new Vector2(100, 100);
        Rect2 drawRect = _grid.DrawCopyBox ? new Rect2(new Vector2(GetGlobalMousePosition().X - 50,GetGlobalMousePosition().Y - 50), rectSize) : new Rect2();

        for (var x = 0; x < gridWidth; x++)
        {
            for (var y = 0; y < gridHeight; y++)
            {
                if (currentGridState[x, y] != null && (_grid.DrawDeadCell || currentGridState[x, y].IsAlive))
                {
                    // Use the cell's color when drawing
                    DrawBox(x, y, currentGridState[x, y].Color);
                }
            }
        }

        if (_grid.DrawCopyBox)
        {
            DrawRect(drawRect, Colors.Red, false);
        }
    }

    private void DrawDebugLinesIfEnabled()
    {
        if (!_grid.DebugState)
        {
            return;
        }

        for (var x = 0; x <= _grid.GridWidth; x++)
        {
            DrawLine(
                new Vector2(x * _grid.BoxSize, 0),
                new Vector2(x * _grid.BoxSize, _grid.GridHeight * _grid.BoxSize),
                new Color(0, 0, 0)
            );
        }

        for (var y = 0; y <= _grid.GridHeight; y++)
        {
            DrawLine(
                new Vector2(0, y * _grid.BoxSize),
                new Vector2(_grid.GridWidth * _grid.BoxSize, y * _grid.BoxSize),
                new Color(0, 0, 0)
            );
        }
    }
    
    private void DebugPrintAllNodes(Node node)
    {
        GD.Print(node.Name);

        foreach (Node child in node.GetChildren())
        {
            DebugPrintAllNodes(child);
        }
    }
}