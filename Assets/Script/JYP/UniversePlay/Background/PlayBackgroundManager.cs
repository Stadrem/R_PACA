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
    // private List<BackgroundPartInfo> backgroundPartDataList;

    public string CurrentBackgroundName => ViewModel.UniverseData.backgroundPartDataList
        .Find((t) => t.ID == ViewModel.CurrentBackgroundId).Name;

    private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

    private void Start()
    {
        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager Start");
    }

    private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.CurrentBackgroundId))
        {
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
        ViewModel.CurrentBackgroundId = background.ID;
        // LoadScene(background);
    }


    private void LoadScene(BackgroundPartInfo background)
    {
        if (!PhotonNetwork.IsMasterClient) return;

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

    public void MoveTo(int backgroundId)
    {
        var background = ViewModel.UniverseData.backgroundPartDataList.Find((t) => t.ID == backgroundId);
        LoadScene(background);
    }
}