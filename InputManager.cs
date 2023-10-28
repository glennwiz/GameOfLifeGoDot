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
            HandleTKeyPress();
            HandleNumberKeyPresses();
            HandleRightKeyPress();
        }

        private void HandleSpaceKeyPress()
        {
            if (Input.IsKeyPressed(Key.Space)) 
            {
                ResetHigherGridArray();
                TogglePause();
            }
        }
        
        private void ResetHigherGridArray()
        {
            if (!_grid.ResetHigherGridArray) return;
            
            var currentStateIndex = _grid.CurrentStateIndex;
            _grid.ListOfCellArrayStates.RemoveRange(currentStateIndex + 1, _grid.ListOfCellArrayStates.Count - currentStateIndex - 1);
        }
        
        private void TogglePause() => _grid.IsPaused = !_grid.IsPaused;
        
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

        private void HideRichTextLabel()
        {
            NodePath path = "../RichTextLabel";
            var childNode = GetNode<RichTextLabel>(path);
            childNode.Visible = false;
            GD.Print(childNode.Name);
        }
        
        private void HandleDownKeyPress()
        {
            if (Input.IsKeyPressed(Key.Down))
            {
                IncreaseTickRate();
            }
        }

        private void HandleTKeyPress()
        {
            if (Input.IsKeyPressed(Key.T))
            {
                HandleMirrorAndShift();
            }
        }

        private void HandleMouseClicked()
        {
            var randomPattern = PatternCreator.CreateRandomPattern(30, 30);
            _grid.DrawPattern(randomPattern);
        }

        private void HandleRightKeyPress()
        {
            if (!Input.IsKeyPressed(Key.Right)) return;
            GD.Print("Right");
        }
        
        private void HandleNumberKeyPresses()
        {
            if (Input.IsKeyPressed(Key.Key1)) DrawGliderPattern();
            if (Input.IsKeyPressed(Key.Key2)) DrawGosperGliderGunPattern();
            if (Input.IsKeyPressed(Key.Key3)) DrawPulsarPattern();
            
            HandleKey4KeyPress();
        }

        private void HandleKey4KeyPress()
        {
            if (!Input.IsKeyPressed(Key.Key4)) return;
            
            var p = new PatternCreator.Pattern(PatternCreator.Pattern.CustomPattern);
            _grid.DrawPattern(p);
        }

        private void HandleRKeyPress()
        {
            if (!Input.IsKeyPressed(Key.R)) return;
            
            var randomPattern = PatternCreator.CreateRandomPattern(30, 30);
            _grid.DrawPattern(randomPattern);
        }

        private void HandleMouseInputEvents(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton mouseButtonEvent)) return;
            HandleMouseClick(mouseButtonEvent);
        }

        private void HandleMouseClick(InputEventMouseButton mouseButtonEvent)
        {
            ProcessMouseRelease(mouseButtonEvent);
            ProcessMousePress(mouseButtonEvent);
        }

        private void ProcessMouseRelease(InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.DoubleClick) 
            {
                _grid.IsMouseDown = false;
                QueueRedraw();
            }
        }

        private void ProcessMousePress(InputEventMouseButton mouseButtonEvent)
        {
            if (!mouseButtonEvent.Pressed) return;
            
            _grid.IsMouseDown = true;
            ToggleCell(mouseButtonEvent.Position);
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
    }
}