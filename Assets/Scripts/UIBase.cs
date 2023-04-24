using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    private void Awake()
    {
        RegisterEvent();
    }

    protected abstract void RegisterEvent();

    public virtual void SendMsg(params object[] command)
    {
        switch(command[0])
        {
            case "Open":
                SetActive(true);
                break;
        }
    }

    protected virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
