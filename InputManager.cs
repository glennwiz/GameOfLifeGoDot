using System;
using Godot;

namespace GameOfLife;

public partial class InputManager : Node2D 
{
	private readonly Grid _grid;
	private void TogglePause() => _grid.IsPaused = !_grid.IsPaused;

	public InputManager(Grid grid) 
	{
		_grid = grid;
	}
		
	public override void _Input(InputEvent @event) 
	{
		HandleKeyPressEvents();
		HandleMouseInputEvents(@event);
	}

	private void HandleKeyPressEvents()
	{
		HandleSpaceKeyPress();
		HandlePauseDependentKeyPresses();
		HandleSKeyPress();
		HandleQKeyPress();
		HandleUpKeyPress();
		HandleDownKeyPress();
		HandleTheMirrorCopyButtonT();
		HandleNumberKeyPresses();
		HandleRKeyPress();
		HandleZoomPress();
	}

	private void HandleZoomPress()
	{
		if (Input.IsKeyPressed(Key.C)) ZoomOut();
		if (Input.IsKeyPressed(Key.V)) ZoomIn();
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

	private void HandleNumberKeyPresses()
	{
		if (Input.IsKeyPressed(Key.Key1)) DrawGliderPattern();
		if (Input.IsKeyPressed(Key.Key2)) DrawGosperGliderGunPattern();
		if (Input.IsKeyPressed(Key.Key3)) DrawPulsarPattern();
		if (Input.IsKeyPressed(Key.Key4)) DrawOwlPattern();
		if (Input.IsKeyPressed(Key.Key5)) DrawRabbit01Pattern();
	}

	private void DrawRabbit01Pattern() //17k pattern?
	{
		var rabbit = new PatternCreator.Pattern(PatternCreator.Pattern.Rabit01);
		_grid.DrawPattern(rabbit);
	}

	private void HandleSpaceKeyPress()
	{
		if (!Input.IsKeyPressed(Key.Space)) return;
		ResetHigherGridArray();
		TogglePause();
	}
		
	private void HandlePauseDependentKeyPresses()
	{
		if (!_grid.IsPaused) return;
		HandleKeyPressedLeftRight();
	}

	private void HandleKeyPressedLeftRight()
	{
		if (Input.IsKeyPressed(Key.Left)) _grid.Rewind();
		if (Input.IsKeyPressed(Key.Right)) _grid.StepForward();
	}
				
	private void HandleSKeyPress()
	{
		if (Input.IsKeyPressed(Key.S)) 
		{
			HandleGridReset();
		}
	}

	private void HandleQKeyPress()
	{
		if (Input.IsKeyPressed(Key.Q))
		{
			ToggleDrawDeadCell();
		}
	}

	private void HandleUpKeyPress()
	{
		if (Input.IsKeyPressed(Key.Up)) 
		{
			HideRichTextLabel();
			DecreaseTickRate();
		}
	}
		
	private void HandleDownKeyPress()
	{
		if (Input.IsKeyPressed(Key.Down))
		{
			IncreaseTickRate();
		}
	}

	bool CopyBox = false;
	private void HandleTheMirrorCopyButtonT()
	{
		if (Input.IsKeyPressed(Key.T))
		{
			CopyBox = !CopyBox;
			HandleMirrorAndShift();
		}
		
		if(CopyBox && Input.IsKeyPressed(Key.O))
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
	}

	private void HandleMouseClicked()
	{
		var randomPattern = PatternCreator.CreateRandomPattern(30, 30, 0.1f);
		_grid.DrawPattern(randomPattern);
	}
		
	private void DrawOwlPattern()
	{
		var pattern = new PatternCreator.Pattern(PatternCreator.Pattern.CustomPattern);
		_grid.DrawPattern(pattern);
	}

	private void HandleRKeyPress()
	{
		if (!Input.IsKeyPressed(Key.R)) return;
			
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

	private void ToggleDrawDeadCell() => _grid.DrawDeadCell = !_grid.DrawDeadCell;
	private void DecreaseTickRate() => _grid.UpdateTickRate -= 0.02f;
	private void IncreaseTickRate() => _grid.UpdateTickRate += 0.02f;
	private void HandleMirrorAndShift() => _grid.MirrorAndShift();

	private void ToggleCell(Vector2 position)
	{
		var x = (int)(position.X / _grid.BoxSize);
		var y = (int)(position.Y / _grid.BoxSize);
		_grid.ToggleCell(new Vector2(x, y));
		QueueRedraw();
	}

	private void DrawGliderPattern()
	{
		var gliderCells = new[,]
		{
			{false, true , false},
			{false, false, true},
			{true , true , true}
		};

		var glider = new PatternCreator.Pattern(gliderCells);
		_grid.DrawPattern(glider);
	}

	private void DrawGosperGliderGunPattern()
	{
		var rotated = MatrixManipulation.RotateMatrix90(PatternCreator.Pattern.GosperGun);
		var gosperGliderGun = new PatternCreator.Pattern(rotated);
		_grid.DrawPattern(gosperGliderGun);
	}
		
	private void DrawPulsarPattern()
	{
		var rotated = MatrixManipulation.RotateMatrix90(PatternCreator.Pattern.Pulsar);
		var pulsarPattern = new PatternCreator.Pattern(rotated);
		_grid.DrawPattern(pulsarPattern);
	}
		
	private void HideRichTextLabel()
	{
		NodePath path = "../RichTextLabel";
		var childNode = GetNode<RichTextLabel>(path);
		childNode.Visible = false;
		GD.Print(childNode.Name);
	}
				
	private void ResetHigherGridArray()
	{
		if (!_grid.ResetHigherGridArray) return;
			
		var currentStateIndex = _grid.CurrentStateIndex;
		_grid.ListOfCellArrayStates.RemoveRange(currentStateIndex + 1, _grid.ListOfCellArrayStates.Count - currentStateIndex - 1);
	}
}
