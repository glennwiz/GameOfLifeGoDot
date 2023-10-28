using System.Diagnostics.CodeAnalysis;
using Godot;

namespace GameOfLife
{
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
            HandleKeyPressEvents();
            HandleMouseInputEvents(@event);
        }

        private void HandleKeyPressEvents()
        {
            if (Input.IsKeyPressed(Key.Space))
            {
                if (_grid.ResetHigherGridArray)
                {
                    //we need to flush the higher list of states and reset the current state index
                    var currentStateIndex = _grid.CurrentStateIndex;
                    //now whe should clear out all the states that are higher than the current state index
                    _grid.ListOfCellArrayStates.RemoveRange(currentStateIndex + 1, _grid.ListOfCellArrayStates.Count - currentStateIndex - 1);
                }
                
                TogglePause();
            }

            if (_grid.IsPaused)
            {
                HandleHistoryStepping();
            }
                
            if (Input.IsKeyPressed(Key.S))
            {
                HandleGridReset();
            }

            if (Input.IsKeyPressed(Key.Q))
            {
                ToggleDrawDeadCell();
            }

            if (Input.IsKeyPressed(Key.Up))
            {
                NodePath path = "../RichTextLabel";
                var childNode = GetNode<RichTextLabel>(path);
                childNode.Visible = false;
                GD.Print(childNode.Name);
                
                DecreaseTickRate();
            }

            if (Input.IsKeyPressed(Key.Down))
            {
                IncreaseTickRate();
            }

            if (Input.IsKeyPressed(Key.T))
            {
                HandleMirrorAndShift();
            }

            if (Input.IsKeyPressed(Key.Key1))
            {
                DrawGliderPattern();
            }

            if (Input.IsKeyPressed(Key.Key2))
            {
                DrawGosperGliderGunPattern();
            }

            if (Input.IsKeyPressed(Key.Key3))
            {
                DrawPulsarPattern();
            }
            
            if (Input.IsKeyPressed(Key.Key4))
            {
                var p = new PatternCreator.Pattern(PatternCreator.Pattern.CustomPattern);
                _grid.DrawPattern(p);
                
            }
            
            if (Input.IsKeyPressed(Key.R))
            {
                var randomPattern = PatternCreator.CreateRandomPattern(30, 30);
                _grid.DrawPattern(randomPattern);
            }

            if (Input.IsKeyPressed(Key.Right))
            {
                GD.Print("Right");
            }
        }

        private void HandleMouseInputEvents(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButtonEvent)
            {
                HandleMouseClick(mouseButtonEvent);
            }
        }

        private void HandleMouseClick(InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.DoubleClick && !mouseButtonEvent.Pressed)
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
                    ToggleCell(mouseButtonEvent.Position);
                }
            }
        }

        private void TogglePause() => _grid.IsPaused = !_grid.IsPaused;
        private void HandleHistoryStepping()
        {
            if (Input.IsKeyPressed(Key.Left)) _grid.Rewind();
            if (Input.IsKeyPressed(Key.Right)) _grid.StepForward();
        }

        private void HandleGridReset()
        {
            _grid.MatrixManipulation.ClearGrid(_grid);
            _grid.ResetUpdateTickRate();
            QueueRedraw();
        }

        private void ToggleDrawDeadCell() => _grid.DrawDeadCell = !_grid.DrawDeadCell;
        private void DecreaseTickRate() => _grid.UpdateTickRate -= 0.1f;
        private void IncreaseTickRate() => _grid.UpdateTickRate += 0.1f;
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
            //I think i wold like a bool to check if we should rotate it or not, may be have a button to flip tins bool?
            
            var rotated = MatrixManipulation.RotateMatrix90(PatternCreator.Pattern.GosperGun);
            var gosperGliderGun = new PatternCreator.Pattern(rotated);
            _grid.DrawPattern(gosperGliderGun);
        }
        
        private void DrawPulsarPattern()
        {
            var rotated = MatrixManipulation.RotateMatrix90(PatternCreator.Pattern.Pulsar);
            var gosperGliderGun = new PatternCreator.Pattern(rotated);
            _grid.DrawPattern(gosperGliderGun);
        }
    }
}