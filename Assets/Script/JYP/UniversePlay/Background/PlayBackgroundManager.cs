using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayBackgroundManager : MonoBehaviour
{
    private List<BackgroundPartInfo> backgroundPartDataList;

    private UniverseData universeData;
    private Background currentBackground = new Background();

    private void Start()
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager Start");
    }

    private void OnEnable()
    {
        Debug.Log($"{SceneManager.GetActiveScene().name} / PlayBackgroundManager OnEnabled");
    }

    public void Init(UniverseData universe, List<BackgroundPartInfo> universeBackgroundPartDataList)
    {
        this.universeData = universe;
        this.backgroundPartDataList = universeBackgroundPartDataList;

        var background = universeBackgroundPartDataList.Find(x => x.ID == universe.startBackground.ID);
        LoadScene(background);
    }

    private void LoadScene(BackgroundPartInfo background)
    {
        string sceneName = "";
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

        StartCoroutine(
            ActionOnLoaded(
                sceneName,
                () =>
                {
                    currentBackground.Init(background);
                    currentBackground.LoadParts();
                    PlayUniverseManager.Instance.NpcManager.LoadNpcList(background.NpcList);
                }
            )
        );
    }

    private IEnumerator ActionOnLoaded(string sceneName, Action callback)
    {
        yield return new WaitUntil(
            () => SceneManager.GetActiveScene()
                      .name
                  == sceneName
        );
        Debug.Log($"{sceneName} is loaded");
        callback?.Invoke();
    }

    public void MoveTo(int backgroundId)
    {
        var background = backgroundPartDataList.Find(x => x.ID == backgroundId);
        LoadScene(background);
    }
}