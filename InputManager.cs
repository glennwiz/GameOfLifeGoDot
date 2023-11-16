using System;
using System.Collections.Generic;
using Godot;

namespace GameOfLife;

public partial class InputManager : Node2D 
{
	private readonly Grid _grid;
	private readonly PatternCreator _patternCreator;
	private void TogglePause() => _grid.IsPaused = !_grid.IsPaused;
	private Dictionary<Key, Action> _keyActions;

	bool CopyBox = false;
	
	public InputManager(Grid grid, PatternCreator patternCreator) 
	{
		_grid = grid;
		_patternCreator = patternCreator;
		InitializeKeyActions();
	}

	private void InitializeKeyActions()
	{
		_keyActions = new Dictionary<Key, Action>
		{
			{Key.Space, () => TogglePause()},
			{Key.S, () => HandleGridReset()},
			{Key.Q, () => ToggleDrawDeadCell()},
			{Key.Up, () =>
				{ 		
					HideRichTextLabel();
					DecreaseTickRate(); 
				}
				
			},
			{Key.Down, () => IncreaseTickRate()},
			{Key.Left, () => _grid.Rewind()},
			{Key.Right, () => _grid.StepForward()},
			{Key.C, () => ZoomOut()},
			{Key.V, () => ZoomIn()},
			{Key.T, () =>
				{
					CopyBox = !CopyBox;
					Mirror();
				}
			},
			{Key.O, () =>
				{
						_grid.ToggleDrawCopyBox();
				}
			},
			
			{Key.H, () => _grid.SetRandomColors()},
			
			{Key.Key1, () => _patternCreator.DrawGliderPattern()},
			{Key.Key2, () => _patternCreator.DrawGosperGliderGunPattern()},
			{Key.Key3, () => _patternCreator.DrawPulsarPattern()},
			{Key.Key4, () => _patternCreator.DrawOwlPattern()},
			{Key.Key5, () => _patternCreator.DrawRabbit01Pattern()},
			{Key.R, () => _patternCreator.DrawRandomPattern()},
		};
	}
	
	private void ToggleDrawDeadCell() => _grid.DrawDeadCell = !_grid.DrawDeadCell;
	private void DecreaseTickRate() => _grid.UpdateTickRate -= 0.02f;
	private void IncreaseTickRate() => _grid.UpdateTickRate += 0.02f;

	public override void _Input(InputEvent @event) 
	{
		if (@event is InputEventKey eventKey && eventKey.Pressed)
		{
			if (_keyActions.TryGetValue(eventKey.Keycode, out var action))
			{
				action.Invoke();
			}
		}
	
		HandleMouseInputEvents(@event);
	}

	private void ZoomIn()
	{
		_grid.BoxSize += 1;
		_grid.GridWidth = (int)GetViewportRect().Size.X / (int)_grid.BoxSize;
		_grid.GridHeight = (int)GetViewportRect().Size.Y / (int)_grid.BoxSize;
	}

	private void ZoomOut()
	{
		if (_grid.BoxSize <= 1)
		{
			return;
		}
		_grid.BoxSize -= 1;
		_grid.GridWidth = (int)GetViewportRect().Size.X / (int)_grid.BoxSize;
		_grid.GridHeight = (int)GetViewportRect().Size.Y / (int)_grid.BoxSize;
	}
	
	private void Mirror()
	{
		var currentPattern = _grid.ListOfCellArrayStates[_grid.CurrentStateIndex];
		var mousePosition = GetGlobalMousePosition();
		var mousePositionX = (int) (mousePosition.X / _grid.BoxSize);
		var mousePositionY = (int) (mousePosition.Y / _grid.BoxSize);
		const int redBoxSize = 10;
		
		//Log the box
		GD.Print(mousePositionX + " | " + mousePositionY);
		
		// Determine the region of the original 2D array to sample
		var redBoxHalf = redBoxSize != 0 ? redBoxSize / 2 : 0;
		var startX = Math.Max(mousePositionX - redBoxHalf, 0);
		var startY = Math.Max(mousePositionY - redBoxHalf, 0);
		var endX = Math.Min(startX + redBoxSize, currentPattern.GetLength(0));
		var endY = Math.Min(startY + redBoxSize, currentPattern.GetLength(1));
		
		GD.Print(startX + " | " + startY + " | " + endX + " | " + endY);

		//grab the cells inside the box
		var cellsInsideBox = new Cell[redBoxSize, redBoxSize];
		for (var x = startX; x < endX && x - startX < redBoxSize; x++)
		{
			for (var y = startY; y < endY && y - startY < redBoxSize; y++)
			{
				cellsInsideBox[x - startX , y - startY] = currentPattern[x, y];
				var cell = cellsInsideBox[x - startX , y - startY];
	
				if(cell == null)
				{
					continue;
				}

				if(cell.IsAlive)
				{
					GD.Print("Alive");
				}
			}
		}
		
		// mirror the cellsInsideBox
		var mirroredCells = MatrixManipulation.MirrorMatrix(cellsInsideBox);
		var boolGrid = MatrixManipulation.MakeBoolGrid(mirroredCells);
		//draw the mirrored cells
		var pattern = new PatternCreator.Pattern(boolGrid);
		_grid.DrawPattern(pattern);
		
		//TODO: Fix the bug where the Mirroed cells is not drawn correctly and the already existing cells are removed, we should only add new cells
	}

	private void HandleMouseClicked()
	{
		var randomPattern = PatternCreator.CreateRandomPattern(30, 30, 0.1f);
		_grid.DrawPattern(randomPattern);
	}
		
	private void HandleMouseInputEvents(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent) HandleMouseClick(mouseButtonEvent);
	}

	private void HandleMouseClick(InputEventMouseButton mouseButtonEvent)
	{
		ProcessMousePress(mouseButtonEvent);
		ProcessMouseRelease(mouseButtonEvent);
	}

	private void ProcessMouseRelease(InputEventMouseButton mouseButtonEvent)
	{
		if (mouseButtonEvent.DoubleClick) 
		{
			GD.Print("!Double click!");
			_grid.IsMouseDown = false;
			QueueRedraw();
		}
	}

	private void ProcessMousePress(InputEventMouseButton mouseButtonEvent)
	{
		if (mouseButtonEvent.Pressed)
		{
			_grid.IsMouseDown = true;
			ToggleCell(mouseButtonEvent.Position);
		}
	}

	private void HandleGridReset()
	{
		_grid.MatrixManipulation.ClearGrid(_grid);
		_grid.ResetUpdateTickRate();
		QueueRedraw();
	}


	private void ToggleCell(Vector2 position)
	{
		var x = (int)(position.X / _grid.BoxSize);
		var y = (int)(position.Y / _grid.BoxSize);
		_grid.ToggleCell(new Vector2(x, y));
		QueueRedraw();
	}

	private void HideRichTextLabel()
	{
		NodePath path = "../RichTextLabel";
		var childNode = GetNode<RichTextLabel>(path);
		childNode.Visible = false;
		GD.Print(childNode.Name);
	}
}
