using System;
using System.Collections;
using Data.Models.Universe.Characters;
using Data.Remote.Api;
using Photon.Pun;
using UnityEngine;
using UniversePlay;
using ViewModels;

public class UserStats : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    public static UserStats Instance;

    [Header("유저 스탯")]
    public string userNickname;

    public int userHealth; // 생명력
    public int userStrength; // 힘
    public int userDexterity; // 손재주


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 자신을 battlePlayers 리스트에 등록하기 위해 RPC 호출
        if (PhotonNetwork.IsConnected)
        {
            StartCoroutine(CoGetStat());

            gameObject.name = "Player_Avatar_" + photonView.Owner.NickName;
        }
    }

    private IEnumerator CoGetStat()
    {
        
        if (!PhotonNetwork.IsConnected && photonView.IsMine) yield break;
        yield return new WaitUntil(() => PlayUniverseManager.Instance != null && PlayUniverseManager.Instance.universeId != 0);
        yield return ScenarioUserSettingsApi.GetUserSetting(
            UserCodeMgr.Instance.UserCode,
            PlayUniverseManager.Instance.universeId,
            (res) =>
            {
                if (res.IsSuccess)
                {
                    var userSetting = res.value;
                    Initialize(userSetting.health, userSetting.strength, userSetting.dex);
                }
            }
        );
        
        RegisterPlayerStats();
    }

    public void Initialize(int health, int strength, int dexterity)
    {
        userHealth = health;
        userStrength = strength;
        userDexterity = dexterity;
    }

    public void RegisterPlayerStats()
    {
        userNickname = photonView.Owner.NickName;

        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            // PlayerBatList의 PhotonView를 찾고 자신을 리스트에 등록
            Debug.Log($"RUN : {userHealth} + {userStrength} + {userDexterity} + {photonView.ViewID}");
            PlayerBatList playerBatList = FindObjectOfType<PlayerBatList>();
            if (playerBatList != null)
            {
                Debug.Log($"RUN 2");
                playerBatList.photonView.RPC(
                    "RegisterPlayer",
                    RpcTarget.All,
                    userNickname,
                    userHealth,
                    userStrength,
                    userDexterity,
                    photonView.ViewID
                );
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        return;
        print("OnPhotonInstantiate");
        if (photonView.InstantiationData == null) return;
        print(userNickname + userHealth + userStrength + userDexterity + photonView.ViewID);

        print($"22222-{userNickname}-{userHealth}-{userStrength} + {photonView.ViewID}");
        Initialize(
            Convert.ToInt32(photonView.InstantiationData[0]),
            Convert.ToInt32(photonView.InstantiationData[1]),
            Convert.ToInt32(photonView.InstantiationData[2])
        );
        var userCode = Convert.ToInt32(
            PhotonNetwork.LocalPlayer.CustomProperties[PunPropertyNames.Player.PlayerUserCode]
        );
        ViewModelManager.Instance.UniversePlayViewModel.UpdateStatByUserCodeWithoutRemote(
            userCode,
            new CharacterStats(userHealth, userStrength, userDexterity)
        );

        print(userNickname + userHealth + userStrength + userDexterity + photonView.ViewID);
    }
}