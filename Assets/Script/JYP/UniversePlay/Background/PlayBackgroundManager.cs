using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using ViewModels;

public class PlayBackgroundManager : MonoBehaviourPun
{
    private List<BackgroundPartInfo> backgroundPartDataList;

    private Background currentBackground = new Background();
    
    public string CurrentBackgroundName => currentBackground.Name;
    private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

    private void Start()
    {
        ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager Start");
    }

    private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.CurrentBackgroundId))
        {
            MoveTo(ViewModel.CurrentBackgroundId);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager OnSceneLoaded");
        if(arg0.name == "WaitingScene") return;
        var background =
            ViewModel.UniverseData.backgroundPartDataList.Find((t) => t.ID == ViewModel.CurrentBackgroundId);
        currentBackground.Init(background);
        currentBackground.LoadParts();
        PlayUniverseManager.Instance.NpcManager.LoadNpcList(background.NpcList);
    }
    
    

    private void OnEnable()
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager OnEnabled");
    }

    public void Init()
    {
        var background = ViewModel.UniverseData.backgroundPartDataList.First();
        ViewModel.CurrentBackgroundId = background.ID;
        LoadScene(background);
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
        var background = backgroundPartDataList.Find(x => x.ID == backgroundId);
        LoadScene(background);
    }
}