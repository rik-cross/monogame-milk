//   Monogame Intuitive Library Kit (milk)
//   A MonoGame ECS Engine, By Rik Cross
//   -- Code: github.com/rik-cross/monogame-milk
//   -- Docs: rik-cross.github.io/monogame-milk
//   -- Shared under the MIT licence

using Microsoft.Xna.Framework.Input;

namespace milk.Core;

/// <summary>
/// Used to receive and respond to player input.
/// </summary>
public class InputManager
{

    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    private List<GamePadState> _currentGamePadState = new List<GamePadState>() {
        GamePad.GetState(0), GamePad.GetState(1), GamePad.GetState(2), GamePad.GetState(3),
    };
    private List<GamePadState> _previousGamePadState = new List<GamePadState>() {
        GamePad.GetState(0), GamePad.GetState(1), GamePad.GetState(2), GamePad.GetState(3),
    };

    internal void Update()
    {
        _previousKeyboardState = _currentKeyboardState;
        _currentKeyboardState = Keyboard.GetState();

        for (int i = 0; i < 4; i++) {
            _previousGamePadState[i] = _currentGamePadState[i];
            _currentGamePadState[i] = GamePad.GetState(i);
        }
    }

    //
    // Keys
    //

    /// <summary>
    /// Returns true if the specified key is held down.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the specified key is held down.</returns>
    public bool IsKeyDown(Keys key)
    {
        if (_currentKeyboardState.IsKeyDown(key))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Returns true if the specified key has been pressed in the current frame
    /// (i.e. the key is down, but wasn't in the previous frame).
    /// </summary>
    /// <param name="key">The key to chec.k</param>
    /// <returns>True if the specified key has been pressed.</returns>
    public bool IsKeyPressed(Keys key)
    {
        if (_currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Returns true if the specified key is no longer down
    /// (i.e. is not down, but was in the previous frame).
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the specified key has been released.</returns>
    public bool IsKeyReleased(Keys key)
    {
        if (!_currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyDown(key))
            return true;
        else
            return false;
    }

    //
    // Buttons
    //

    /// <summary>
    /// Returns true if the specified button is down.
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="controllerNumber">The controller number to check (default = controller 0).</param>
    /// <returns>True if the specified button is down.</returns>
    public bool IsButtonDown(Buttons button, int controllerNumber = 0)
    {
        if (_currentGamePadState[controllerNumber].IsButtonDown(button))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Returns true if the specified button is pressed
    /// (i.e. is down, but wasn't down in the previous frame).
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="controllerNumber">The controller number to check (default = controller 0).</param>
    /// <returns>True if the specified button is pressed.</returns>
    public bool IsButtonPressed(Buttons button, int controllerNumber = 0)
    {
        if (_currentGamePadState[controllerNumber].IsButtonDown(button) && !_previousGamePadState[controllerNumber].IsButtonDown(button))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Returns true if the specified button is released
    /// (i.e. is not down, but was down in the previous frame).
    /// </summary>
    /// <param name="button">The button to check.</param>
    /// <param name="controllerNumber">The controller number to check (default = controller 0).</param>
    /// <returns>True if the specified button has been released.</returns>
    public bool IsButtonReleased(Buttons button, int controllerNumber = 0)
    {
        if (!_currentGamePadState[controllerNumber].IsButtonDown(button) && _previousGamePadState[controllerNumber].IsButtonDown(button))
            return true;
        else
            return false;
    }

    //
    // Controllers
    //

    /// <summary>
    /// Returns true if at least one controller is connected.
    /// </summary>
    /// <returns>True if at least one controller is connected.</returns>
    public bool IsControllerConnected() {
        return GamePad.GetState(0).IsConnected;
    }

    /// <summary>
    /// Returns the number of connected controllers.
    /// </summary>
    /// <returns>The number of controllers connected.</returns>
    public int NumberOfControllersConnected() {
        int returnNumber = 0;
        for (int i = 0; i < 4; i++) {
            if (GamePad.GetState(i).IsConnected) {
                returnNumber += 1;
            }
        }
        return returnNumber;
    }

}