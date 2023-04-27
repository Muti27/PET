using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun.Demo.Asteroids;
using Photon.Pun.UtilityScripts;

public class BattleController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pet;
    [SerializeField] private GameObject boss;

    private bool startTimeIsSet = false;

    public const string PLAYER_BATTLE_PET = "Battle/PET";
    public const string PLAYER_LOADED_LEVEL = "PlayerLoadedLevel";

    public override void OnEnable()
    {
        base.OnEnable();

        CountdownTimer.OnCountdownTimerHasExpired += GameStart;
    }

    private void Start()
    {      
        Hashtable props = new Hashtable
            {
                {PLAYER_LOADED_LEVEL, true}
            };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        
        CountdownTimer.OnCountdownTimerHasExpired -= GameStart;
    }

    public void GameStart()
    {
        PhotonNetwork.Instantiate(PLAYER_BATTLE_PET, Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        int timestamp = 0;
        bool startTimeIsSet = CountdownTimer.TryGetStartTime(out timestamp);

        if (changedProps.ContainsKey(PLAYER_LOADED_LEVEL))
        {
            if (CheckAllPlayerLoadedLevel())
            {
                if (!startTimeIsSet)                
                {
                    Debug.Log("Setting start time");
                    CountdownTimer.SetStartTime();
                }
            }
            else
            {
                // not all players loaded yet. wait:
                Debug.Log("setting text waiting for players! ");
                //InfoText.text = "Waiting for other players...";
            }
        }
    }

    private bool CheckAllPlayerLoadedLevel()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            object playerLoadedLevel;

            if (player.CustomProperties.TryGetValue(PLAYER_LOADED_LEVEL, out playerLoadedLevel))
            {
                if ((bool)playerLoadedLevel)
                {
                    continue;
                }
            }

            return false;
        }

        return true;
    }
}
