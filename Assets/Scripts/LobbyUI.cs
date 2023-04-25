using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : UIBase
{
    [SerializeField] private Button createBtn;
    [SerializeField] private Button connectBtn;
    [SerializeField] private Button battleBtn;

    [SerializeField] private GameObject namePopup;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmBtn;

    [SerializeField] private TextMeshProUGUI connectStateText;
    [SerializeField] private Transform playerListTrans;

    private List<PlayerEle> playerEleList;

    public void Awake()
    {
        playerEleList = new List<PlayerEle>();

        RegisterEvent();
    }

    protected override void RegisterEvent()
    {
        createBtn.onClick.AddListener(OnClickCreate);
        confirmBtn.onClick.AddListener(OnClickConfirmName);
        connectBtn.onClick.AddListener(OnClickConnectLobby);
        battleBtn.onClick.AddListener(OnClickBattle);
    }

    public override void SendMsg(params object[] command)
    {
        string msg = command[0].ToString();

        switch (msg)
        {
            case "UpdatePlayerList":
                UpdatePlayerList((Player[])command[1]);
                break;
            case "SetBattle":
                battleBtn.gameObject.SetActive(true);
                break;
            case "Open":
                SetActive(true);
                break;
        }
    }

    private void Update()
    {
        connectStateText.text = $"Connect State:{PhotonNetwork.NetworkClientState.ToString()}";
    }

    private void OnClickCreate()
    {
        namePopup.SetActive(true);
    }

    private void OnClickConfirmName()
    {
        if (string.IsNullOrEmpty(nameInput.text))
            return;

        if (MainController.Instance.CreatePET(nameInput.text))
        {
            createBtn.gameObject.SetActive(false);
            connectBtn.gameObject.SetActive(true);

            namePopup.SetActive(false);
        }
    }

    private void OnClickConnectLobby()
    {
        MainController.Instance.ConnectToMaster(() => { connectBtn.gameObject.SetActive(false); });
    }

    private void OnClickBattle()
    {
        UIManager.Instance.ChangeScene(SceneType.Battle);
    }

    public void UpdatePlayerList(Player[] playerList)
    {
        int index = 0;
        foreach(var player in playerEleList)
        {
            player.UpdateInfo(string.Empty);
        }

        foreach(var player in playerList)
        {
            if (index >= playerEleList.Count)
            {
                GameObject nameObj = Instantiate(Resources.Load($"PlayerEle"), playerListTrans) as GameObject;
                PlayerEle ele = nameObj.GetComponent<PlayerEle>();
                playerEleList.Add(ele);
            }

            playerEleList[index].UpdateInfo(player.NickName);
            index++;
        }
    }
}
