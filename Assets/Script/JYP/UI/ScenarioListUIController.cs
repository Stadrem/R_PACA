using System;
using System.Collections.Generic;
using Data.Remote;
using Data.Remote.Dtos.Response;
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

    //todo data container전용 클래스로 옮기기
    private List<ScenarioListItemResponseDto> scenarioList;

    public Action<ScenarioListItemResponseDto> OnScenarioCreated;


    /// <summary>
    /// 시나리오 리스트를 받아와서 UI에 보여준다
    /// </summary>
    public void Show()
    {
        LoadData(ShowUI);
    }

    private void ShowUI()
    {

        foreach (var scenario in scenarioList)
        {
            var entry = Instantiate(scenarioEntryPrefab, scrollContainer).GetComponent<ScenarioListEntryUIController>();
            entry.BindData(scenario);
            entry.OnClickCreateButton = () => OnScenarioCreated?.Invoke(scenario);
        }
    }

    private void LoadData(Action onLoaded)
    {
        StartCoroutine(
            ScenarioApi.GetScenarioList(
                (list) =>
                {
                    if (list.IsSuccess)
                    {
                        scenarioList = list.value;
                        onLoaded();
                    }
                }
            )
        );
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
}