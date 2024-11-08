using System;
using System.Collections.Generic;
using Data.Remote;
using Data.Remote.Dtos.Response;
using JetBrains.Annotations;
using UnityEngine;

public class ScenarioListUIController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]
    private RectTransform scrollContainer;

    [Space]
    [Header("Prefabs")]
    [SerializeField]
    private GameObject scenarioEntryPrefab;

    [Space]
    [Header("UI Flow Manage")]
    
    [Tooltip("방 생성을 위한 데이터 입력을 위한 팝업")]
    [SerializeField]
    private CanvasActive roomCreatePopupCanvasActive;

    
    //todo data container전용 클래스로 옮기기
    private List<ScenarioListItemResponseDto> scenarioList;
    private int selectedIndex = -1;

    /// <summary>
    /// 시나리오 리스트를 받아와서 UI에 보여준다
    /// </summary>
    public void Init()
    {
        LoadData(ShowUI);
    }

    private void ShowUI()
    {
        foreach (var scenario in scenarioList)
        {
            var entry = Instantiate(scenarioEntryPrefab, scrollContainer).GetComponent<ScenarioListEntryUIController>();
            entry.BindData(scenario);
            entry.OnClickSelectButton = () => Select(scenario);
        }
    }

    /// <summary>
    /// 닫는다.
    /// </summary>
    public void Hide()
    {
        //clear data
        scenarioList = null;

        //clear ui
        foreach (Transform child in scrollContainer)
        {
            Destroy(child.gameObject);
        }
    }

    [CanBeNull]
    public ScenarioListItemResponseDto GetSelectedScenario()
    {
        if (selectedIndex < 0)
        {
            return null;
        }

        return scenarioList[selectedIndex];
    }


    private void Select(ScenarioListItemResponseDto scenario)
    {
        selectedIndex = scenarioList.IndexOf(scenario);
        roomCreatePopupCanvasActive.OnClickPop();
    }

    private void LoadData(Action onLoaded)
    {
        StartCoroutine(
            ScenarioApi.GetScenarioList(
                (list) =>
                {
                    if (list.IsSuccess)
                    {
                        print($"loaded: {list.value.Count}");
                        scenarioList = list.value;
                        onLoaded();
                    }
                }
            )
        );
    }
}