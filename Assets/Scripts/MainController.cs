using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainController : MonoBehaviourPunCallbacks
{
    public static MainController Instance;

    [SerializeField] private GameObject petObj;

    private PET playerPet;
    private Dictionary<int, Player> playerList;

    public PET PlayerPet { get { return playerPet; } }

    // Start is called before the first frame update
    private void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        PhotonNetwork.AutomaticallySyncScene = true;

        UIManager.Instance.SendMsg(UIName.LobbyUI, "Open");
    }

    public bool CreatePET(string name)
    {
        if (playerPet != null)
            return false;

        GameObject pet = Instantiate(petObj, transform);
        playerPet = pet.GetComponent<PET>();

        playerPet.Init(name);

        return true;
    }

    public void ConnectToMaster(Action callback)
    {
        PhotonNetwork.LocalPlayer.NickName = playerPet.PetName;
        if (PhotonNetwork.ConnectUsingSettings())
        {
            callback?.Invoke();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connect Server Success");

        if (!PhotonNetwork.InRoom)
        {
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = 2;

            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions:options);
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Create Room Success");

        if (playerList == null)
            playerList = new Dictionary<int, Player>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.NickName}");
            playerList.Add(player.ActorNumber, player);
        }

        UpdatePlayerList();
    }

    public override void OnJoinedRoom()
    {       
        Debug.Log($"Join Room {PhotonNetwork.CurrentRoom.Name} Success");

        if (playerList == null)
            playerList = new Dictionary<int, Player>();

        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.NickName}");
            playerList.Add(player.ActorNumber, player);
        }

        UpdatePlayerList();

        if (PhotonNetwork.IsMasterClient)
        {
            UIManager.Instance.SendMsg(UIName.LobbyUI, "SetBattle");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} Enter the Room");

        playerList.Add(newPlayer.ActorNumber, newPlayer);

        UpdatePlayerList();
    }        

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} Left the Room");

        playerList.Remove(otherPlayer.ActorNumber);

        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        UIManager.Instance.SendMsg(UIName.LobbyUI, "UpdatePlayerList", playerList.Values.ToArray());
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.Disconnect();
    }
}


