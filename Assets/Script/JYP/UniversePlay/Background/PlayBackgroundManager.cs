using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayBackgroundManager : MonoBehaviour
{
    private List<BackgroundPartData> backgroundPartDataList;

    private UniverseData universeData;
    private Background currentBackground;

    private void Awake()
    {
    }

    public void Init(UniverseData universe, List<BackgroundPartData> universeBackgroundPartDataList)
    {
        this.universeData = universe;
        this.backgroundPartDataList = universeBackgroundPartDataList;
        var type = universeBackgroundPartDataList.Find(x => x.id == universe.startBackgroundId).Type;
        LoadSceneByPreset(type);
    }

    private void LoadSceneByPreset(EBackgroundPartType type)
    {
        switch (type)
        {
            case EBackgroundPartType.None:
                break;
            case EBackgroundPartType.Town:
                SceneManager.LoadScene("Town");
                break;
            case EBackgroundPartType.Dungeon:
                SceneManager.LoadScene("Dungeon");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }


    public void MoveTo(int backgroundId)
    {
        var type = backgroundPartDataList.Find(x => x.id == backgroundId).Type;
        LoadSceneByPreset(type);
    }
}