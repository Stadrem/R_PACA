using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBackgroundManager : MonoBehaviour
{
    private List<BackgroundPartData> backgroundPartDataList;
    private List<Background> backgroundList;
    
    private UniverseData universeData;
    private Background currentBackground;
    
    private void Awake()
    {
        backgroundPartDataList = new List<BackgroundPartData>();
    }
    
    public void Init(UniverseData universeData,List<BackgroundPartData> backgroundPartDataList)
    {
        this.universeData = universeData;
        this.backgroundPartDataList = backgroundPartDataList;
        backgroundList = new List<Background>(backgroundPartDataList.Count);
        backgroundList.Find(x => x.Id == universeData.startBackgroundId).Show();
    }

    private IEnumerator LoadData()
    {
        for(int i = 0;i < backgroundList.Count; i++)
        {
            var backgroundPartData = backgroundPartDataList[i];
            GameObject backgroundPartPresetPrefab;

            switch (backgroundPartData.Type)
            {
                case EBackgroundPartType.None:
                    backgroundPartPresetPrefab = Resources.Load<GameObject>("BackgroundPartPresets/Default");
                    break;
                case EBackgroundPartType.Town:
                    backgroundPartPresetPrefab = Resources.Load<GameObject>("BackgroundPartPresets/Town");
                    break;
                case EBackgroundPartType.Dungeon:
                    backgroundPartPresetPrefab = Resources.Load<GameObject>("BackgroundPartPresets/Dungeon");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (backgroundPartPresetPrefab == null)
            {
                Debug.LogError($"Background part {backgroundPartData.Name} not found");
                continue;
            }
        }
        yield return null;
    }

    public void MoveTo(int backgroundId)
    {
        if (currentBackground == null) return;
        currentBackground.Hide();
        currentBackground = backgroundList.Find(x => x.Id == backgroundId);
        currentBackground.Show();
    }
}