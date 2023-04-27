using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIName
{
    LobbyUI,
}

public enum SceneType
{
    Lobby = 0,
    Battle,
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private Dictionary<string, UIBase> openingUIList;
    private List<GameObject> additionalUIList;
    private short renderIndex = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        openingUIList = new Dictionary<string, UIBase>();
        additionalUIList = new List<GameObject>();

        DontDestroyOnLoad(this);
    }

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
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void RegisterUI(string name, UIBase ui)
    {
        openingUIList.Add(name, ui);
    }

    public GameObject InstaniateAdditionUI(string path)
    {
        GameObject uiObj = Instantiate(Resources.Load(path), transform) as GameObject;

        additionalUIList.Add(uiObj);

        return uiObj;
    }

    public void ChangeScene(SceneType scene)
    {
        var keyList = openingUIList.Keys.ToList();
        foreach (var key in keyList)
        {
            Destroy(openingUIList[key].gameObject);
        }

        openingUIList.Clear();

        foreach (var obj in additionalUIList)
        {
            Destroy(obj);
        }

        additionalUIList.Clear();

        SceneManager.LoadScene((int)scene);
    }
}
