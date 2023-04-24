using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIName
{
    LobbyUI,
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        openingUIList = new Dictionary<string, UIBase>();
    }

    private Dictionary<string, UIBase> openingUIList;
    private short renderIndex = 0;

    public void SendMsg(UIName name, params object[] command)
    {
        try
        {
            UIBase ui = null;
            if (openingUIList.ContainsKey(name.ToString()) == false)
            {
                GameObject uiobj = Instantiate(Resources.Load<GameObject>($"UI/{name.ToString()}"), transform);
                ui = uiobj.GetComponent<UIBase>();

                RegisterUI(name.ToString(), ui);
            }
            else
            {
                ui = openingUIList[name.ToString()];
            }

            ui.SendMsg(command);
        }
        catch(Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void RegisterUI(string name, UIBase ui)
    {
        openingUIList.Add(name, ui);
    }
}
