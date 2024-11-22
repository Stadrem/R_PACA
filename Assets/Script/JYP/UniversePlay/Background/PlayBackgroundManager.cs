using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniversePlay;
using ViewModels;

public class PlayBackgroundManager : MonoBehaviourPun
{
    private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

    private int previousBackgroundId = -1;
    private bool isEventRegistered = false;
    private void Start()
    {
        if(!isEventRegistered)
        {
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            isEventRegistered = true;
        }
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager Start");
    }

    private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.CurrentBackgroundId) && ViewModel.CurrentBackgroundId != previousBackgroundId)
        {
            previousBackgroundId = ViewModel.CurrentBackgroundId;
            Debug.Log($"currentBackgroundId: {ViewModel.CurrentBackgroundId}");
            if(PhotonNetwork.IsMasterClient)
                MoveTo(ViewModel.CurrentBackgroundId);
        }
    }

    private void OnEnable()
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager OnEnabled");
    }

    public void StartFirstBackground()
    {
        var background = ViewModel.UniverseData.backgroundPartDataList.First();
        photonView.RPC(nameof(RPC_SetCurrentBackgroundId), RpcTarget.All, background.ID);
        // LoadScene(background);
    }


    private void LoadScene(BackgroundPartInfo background)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log($"LoadScene {background.Type}");
        var sceneName = "";
        switch (background.Type)
        {
            case EBackgroundPartType.None:
                break;
            case EBackgroundPartType.Town:
                sceneName = "Town";
                PhotonNetwork.LoadLevel("Town");
                break;
            case EBackgroundPartType.Dungeon:
                sceneName = "Dungeon";
                PhotonNetwork.LoadLevel("Dungeon");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MoveTo(int backgroundId)
    {
        var background = ViewModel.UniverseData.backgroundPartDataList.Find((t) => t.ID == backgroundId);
        LoadScene(background);
    }
    
    
    [PunRPC]
    private void RPC_SetCurrentBackgroundId(int backgroundId)
    {
        print($"RPC_SetCurrentBackgroundId {backgroundId}");
        ViewModel.CurrentBackgroundId = backgroundId;
    }
}