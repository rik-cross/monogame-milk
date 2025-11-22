using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace milk.Core;

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

    public void Update()
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

    public bool IsKeyDown(Keys key)
    {
        if (_currentKeyboardState.IsKeyDown(key))
            return true;
        else
            return false;
    }

    public bool IsKeyPressed(Keys key)
    {
        if (_currentKeyboardState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key))
            return true;
        else
            return false;
    }

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

    public bool IsButtonDown(Buttons button, int controllerNumber = 0)
    {
        if (_currentGamePadState[controllerNumber].IsButtonDown(button))
            return true;
        else
            return false;
    }

    public bool IsButtonPressed(Buttons button, int controllerNumber = 0)
    {
        if (_currentGamePadState[controllerNumber].IsButtonDown(button) && !_previousGamePadState[controllerNumber].IsButtonDown(button))
            return true;
        else
            return false;
    }

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

    public bool IsControllerConnected() {
        return GamePad.GetState(0).IsConnected;
    }

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