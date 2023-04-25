using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pet;
    [SerializeField] private GameObject boss;

    private void Awake()
    {
        PhotonNetwork.Instantiate("Battle/PET", Vector3.zero, Quaternion.identity);
        //PhotonNetwork.InstantiateRoomObject();
    }
}
