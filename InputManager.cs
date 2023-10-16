using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife;

public partial class InputManager : Node2D
{
	private readonly Grid _grid;

	public InputManager(Grid grid)
	{
		_grid = grid;
	}

	[SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Space))
		{
			_grid.IsPaused = !_grid.IsPaused;
		}
		
		if (_grid.IsPaused) {
			if (Input.IsKeyPressed(Key.Left)) {
				_grid.Rewind();
			}
			if (Input.IsKeyPressed(Key.Right)) {
				_grid.StepForward();
			}
		}

		// Handle 'S' key press to clear cells and reset speed
		if (Input.IsKeyPressed(Key.S))
		{
			_grid.MatrixManipulation.ClearGrid(_grid);
			_grid.ResetUpdateTickRate();
			
			//we need to clear the whole List of grids
			

			QueueRedraw();
		}

		if (Input.IsKeyPressed(Key.Q))
		{
			_grid.DrawDeadCell = !_grid.DrawDeadCell;
		}

		if (Input.IsKeyPressed(Key.Up))
		{
			_grid.UpdateTickRate -= 0.1f;
		}

		if (Input.IsKeyPressed(Key.Down))
		{
			_grid.UpdateTickRate += 0.1f;
		}

		// Handle mouse input to toggle cells
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_grid.IsMouseDown = false;
				 QueueRedraw();
				return;
			}

			if (mouseButtonEvent.Pressed)
			{
				_grid.IsMouseDown = mouseButtonEvent.Pressed;
				if (_grid.IsMouseDown)
				{
					var mousePosition = mouseButtonEvent.Position;
					var x = (int) (mousePosition.X / _grid.BoxSize);
					var y = (int) (mousePosition.Y / _grid.BoxSize);
					_grid.ToggleCell(new Vector2(x, y));
					QueueRedraw();
				}

				if (!_grid.IsMouseDown)
				{
					_grid.IsMouseDown = false;
					QueueRedraw();
				}
			}
		}

		if (Input.IsKeyPressed(Key.R))
		{
    		_grid.MirrorAndShift();
		}
		
		if (Input.IsKeyPressed(Key.Key1))
		{
			// create the walker pattern at mouse position
			var gliderCells = new[,]
			{
				{false, true , false},
				{false, false, true},
				{true , true , true}
			};

			var glider = new PatternCreator.Pattern(3, 3, gliderCells);
			_grid.DrawPattern(glider);
		}

		if (Input.IsKeyPressed(Key.Key2))
		{
			const bool f = false; //too easier read the pattern in the grid
		
			var rotated = MatrixManipulation.RotateMatrix90(PatternCreator.Pattern.GosperGun);
			
			var gosperGliderGun = new PatternCreator.Pattern(36, 9, rotated);
			_grid.DrawPattern(gosperGliderGun);
		}

		if (Input.IsKeyPressed(Key.Right))
		{
			GD.Print("Right");
		}
	}
}