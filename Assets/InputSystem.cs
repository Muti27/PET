using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputSystem : MonoBehaviour
{
    public Vector2 move;
    public Vector2 look;

    public bool isRolling = false;
    public bool isAttack = false;
    public bool cursorLocked = false;

    public void OnMove(InputValue value)
    {
        Debug.Log($"{value.Get<Vector2>()}");
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        Debug.Log($"{value.Get<Vector2>()}");
        look = value.Get<Vector2>();
    }

    public void OnRoll(InputValue value)
    {
        isRolling = value.isPressed;
    }

    public void OnAttack(InputValue value)
    {
        isAttack = value.isPressed;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
