using Godot;
using System;
using System.Collections.Generic;

public partial class GridManager : Node2D
{
	private bool debug = true;
	private int gridWidth = 100;
	private int gridHeight = 100;
	private int boxSize = 10;
	private List<bool[,]> gridStates = new List<bool[,]>();
	private int currentStateIndex = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Ready!!");
		InitGrid();
	}
	
	private void InitGrid()
	{
		 var viewportSize = GetViewportRect().Size;
		gridWidth = (int)viewportSize.X / boxSize;
		gridHeight = (int)viewportSize.Y / boxSize;
		var initialState = new bool[gridWidth, gridHeight];
		initialState[5, 5] = true;  // This will set a box to be drawn at grid coordinates (5, 5).
		gridStates.Add(initialState);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		 GD.Print("Process!!");
	}
	
	 public void DrawBox(int x, int y)
	{
		if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
		{
			GD.PrintErr("Coordinates out of bounds");
			return;
		}

		// Update the current grid state
		var currentGridState = gridStates[currentStateIndex];
		currentGridState[x, y] = true;

		// Convert grid coordinates to screen coordinates
		var screenX = x * boxSize;
		var screenY = y * boxSize;

		// Draw a 10x10 pixel box at the specified grid coordinates
		DrawRect(new Rect2(screenX, screenY, boxSize, boxSize), new Color(1, 0, 0));
	}

	public void SaveState()
	{
		var currentGridState = gridStates[currentStateIndex];
		var newGridState = (bool[,]) currentGridState.Clone();
		gridStates.Add(newGridState);
		currentStateIndex++;

		// Optionally, limit the history to the last 100 states
		if (gridStates.Count > 100)
		{
			gridStates.RemoveAt(0);
			currentStateIndex--;
		}
	}
	
	public override void _Draw()
	{
		var currentGridState = gridStates[currentStateIndex];
		for (int x = 0; x < gridWidth; x++)
		{
			for (int y = 0; y < gridHeight; y++)
			{
				if (currentGridState[x, y])
				{
					DrawBox(x, y);
				}
			}
		}

		if (debug)
		{
			for (int x = 0; x <= gridWidth; x++)
			{
				DrawLine(new Vector2(x * boxSize, 0), new Vector2(x * boxSize, gridHeight * boxSize), new Color(0, 0, 0));
			}

			for (int y = 0; y <= gridHeight; y++)
			{
				DrawLine(new Vector2(0, y * boxSize), new Vector2(gridWidth * boxSize, y * boxSize), new Color(0, 0, 0));
			}
		}
	}
}
