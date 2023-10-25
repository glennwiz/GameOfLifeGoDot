using Godot;

namespace GameOfLife;

public partial class GameGridHandler : Node2D 
{
	private readonly MatrixManipulation _matrixManipulation = new();
	
	private Grid _grid;
	private GridController _controller;
	private InputManager _inputManager;
	private PatternCreator _patternCreator;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{  
		var gridSize = GetViewportRect().Size;
		_patternCreator = new PatternCreator();
		_grid = new Grid(_matrixManipulation, gridSize);
		_controller = new GridController(_grid);
		_inputManager = new InputManager(_grid);
		
		AddChild(_grid);
		AddChild(_controller);
		AddChild(_inputManager);
		
		PrintAllNodes(this);	
	}
	
	void PrintAllNodes(Node node)
	{
		GD.Print(node.Name);
		foreach (var child in node.GetChildren())
		{
			PrintAllNodes(child);
		}
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
		if (_grid.CurrentStateIndex >= 0 && _grid.CurrentStateIndex < _grid.GridCells.Count)
		{
			var currentGridState = _grid.GridCells[_grid.CurrentStateIndex];

			for (var x = 0; x < _grid.GridWidth; x++)
			{
				for (var y = 0; y < _grid.GridHeight; y++)
				{
					// Check if there is a cell at this position
					if (currentGridState[x, y] == null)
					{
						continue;
					}

					if (_grid.DrawDeadCell)
					{
						// Use the cell's color when drawing
						DrawBox(x, y, currentGridState[x, y].Color);
					}
					else
					{
						if (currentGridState[x, y].IsAlive)
						{
							// Use the cell's color when drawing
							DrawBox(x, y, currentGridState[x, y].Color);
						}
					}

					if (_grid.DrawCopyBox)
					{
						//draw a red square around the mouse position
						var mousePosition = GetGlobalMousePosition();
						DrawRect(new Rect2(mousePosition, new Vector2(80, 80)), Colors.Red, false);
					}
				}
			}
		}

		if (_grid.DebugState)
		{
			for (var x = 0; x <= _grid.GridWidth; x++)
			{
				DrawLine(new Vector2(x * _grid.BoxSize, 0), new Vector2(x * _grid.BoxSize, _grid.GridHeight * _grid.BoxSize),
					new Color(0, 0, 0));
			}

			for (var y = 0; y <= _grid.GridHeight; y++)
			{
				DrawLine(new Vector2(0, y * _grid.BoxSize), new Vector2(_grid.GridWidth * _grid.BoxSize, y * _grid.BoxSize),
					new Color(0, 0, 0));
			}
		}
	}
	
	public void DrawBox(int x, int y, Color color)
	{
		if (x < 0 || x >= _grid.GridWidth || y < 0 || y >= _grid.GridHeight)
		{
			GD.PrintErr("Coordinates out of bounds");
			return;
		}

		DrawRect(new Rect2(x * _grid.BoxSize, y * _grid.BoxSize, _grid.BoxSize, _grid.BoxSize), color);
	}
}