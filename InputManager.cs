using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife;

public class InputManager
{
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
	public override void _Input(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Space))
		{
			_isPaused = !_isPaused;
		}
		
		if (_isPaused) {
			if (Input.IsKeyPressed(Key.Left)) {
				Rewind();
			}
			if (Input.IsKeyPressed(Key.Right)) {
				StepForward(); // You'll need to implement this
			}
		}

		// Handle 'S' key press to clear cells and reset speed
		if (Input.IsKeyPressed(Key.S))
		{
			_matrixManipulation.ClearGrid(_gridCells);
			ResetUpdateTickRate();

			QueueRedraw();
		}

		if (Input.IsKeyPressed(Key.Q))
		{
			_drawDeadCell = !_drawDeadCell;
		}

		if (Input.IsKeyPressed(Key.Up))
		{
			_updateTickRate -= 0.1f;
		}

		if (Input.IsKeyPressed(Key.Down))
		{
			_updateTickRate += 0.1f;
		}

		// Handle mouse input to toggle cells
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.DoubleClick)
			{
				_isMouseDown = false;
				return;
			}

			if (mouseButtonEvent.Pressed)
			{
				_isMouseDown = mouseButtonEvent.Pressed;
				if (_isMouseDown)
				{
					var mousePosition = mouseButtonEvent.Position;
					var x = (int) (mousePosition.X / _boxSize);
					var y = (int) (mousePosition.Y / _boxSize);
					ToggleCell(new Vector2(x, y));
					QueueRedraw();
				}

				if (!_isMouseDown)
				{
					_isMouseDown = false;
				}
			}
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

			var glider = new GridManager.Pattern(3, 3, gliderCells);
			DrawPattern(glider);
		}

		if (Input.IsKeyPressed(Key.Key2))
		{
			const bool f = false; //too easier read the pattern in the grid
		
			// create a Gosper's glider gun at mouse position
			var gosperCells =  new[,]
			{
				{f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , f   , f   , f   , f   , f   , f   , f    , f , f , f   , f   , f   , f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
				{f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , f   , f   , f   , f   , f   , f   , f    , f , f , f   , f   , true, f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
				{f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , f   , true, true, f   , f   , f   , f    , f , f , true, true, f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
				{f 	  , f   , f , f	, f	, f	, f	, f , f , f , f   , true, f   , f   , f   , true, f   , f    , f , f , true, true, f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , true, true },
				{true , true, f , f	, f	, f	, f	, f , f , f , true, f   , f   , f   , f   , f   , true, f    , f , f , true, true, f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , true, true },
				{true , true, f , f	, f	, f	, f	, f , f , f , true, f   , f   , f   , true, f   , true, true , f , f , f   , f   , true, f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
				{f    , f 	, f , f	, f	, f	, f	, f , f , f , true, f   , f   , f   , f   , f   , true, f    , f , f , f   , f   , f   , f , true, f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
				{f    , f 	, f , f	, f	, f	, f	, f , f , f , f   , true, f   , f   , f   , true, f   , f    , f , f , f   , f   , f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , f   , f    },
				{f    , f 	, f , f	, f	, f	, f	, f , f , f , f   , f   , true, true, f   , f   , f   , f    , f , f , f   , f   , f   , f , f   , f  , f	, f	, f	, f	, f	, f , f , f , f   , f    }
			};

			var rotated = MatrixManipulation.RotateMatrix90(gosperCells);
			
			var gosperGliderGun = new GridManager.Pattern(36, 9, rotated);
			DrawPattern(gosperGliderGun);
		}

		if (Input.IsKeyPressed(Key.Right))
		{
			GD.Print("Right");
		}
	}
}