using Photon.Pun;
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
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
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

    public void OnExit(InputValue value)
    {
        if (value.isPressed)
        {
            if (PhotonNetwork.LeaveRoom())
            {
                UIManager.Instance.ChangeScene(SceneType.Lobby);
            }
        }
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
