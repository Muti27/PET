using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviourPunCallbacks
{
    public void Awake()
    {
        if (PhotonNetwork.OfflineMode)
        {
            Instantiate(Resources.Load(BattleController.PLAYER_BATTLE_PET), transform);
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = Guid.NewGuid().ToString();
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        PhotonNetwork.Instantiate(BattleController.PLAYER_BATTLE_PET, Vector3.zero, Quaternion.identity);
    }
}
