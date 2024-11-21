using System;
using Data.Models.Universe.Characters;
using Photon.Pun;
using UnityEngine;
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
            RegisterPlayerStats();
        }
    }

    public void Initialize(int health, int strength, int dexterity)
    {
        userHealth = health;
        userStrength = strength;
        userDexterity = dexterity;
    }

    private void OnEnable()
    {
        // SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     // 씬 이름이 Town일때만
    //     if (scene.name == "Town") 
    //     {
    //         if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
    //         {
    //             RegisterPlayerStats();
    //         }
    //     }
    // }

    public void RegisterPlayerStats()
    {
        userNickname = photonView.Owner.NickName;

        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            // PlayerBatList의 PhotonView를 찾고 자신을 리스트에 등록
            Debug.Log($"RUN");
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
                    userDexterity
                );
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        userHealth = Convert.ToInt32(info.photonView.InstantiationData[0]);
        userStrength = Convert.ToInt32(info.photonView.InstantiationData[1]);
        userDexterity = Convert.ToInt32(info.photonView.InstantiationData[2]);

        var userCode = Convert.ToInt32(
            PhotonNetwork.LocalPlayer.CustomProperties[PunPropertyNames.Player.PlayerUserCode]
        );
        ViewModelManager.Instance.UniversePlayViewModel.UpdateStatByUserCodeWithoutRemote(
            userCode,
            new CharacterStats(userHealth, userStrength, userDexterity)
        );
    }
}