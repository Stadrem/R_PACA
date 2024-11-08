using System;
using Data.Remote.Dtos.Response;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioListEntryUIController : MonoBehaviour
{
    [SerializeField]
    private Button createButton;

    [SerializeField]
    private TMP_Text titleText;

    private int id;


    public Action OnClickCreateButton
    {
        set { createButton.onClick.AddListener(() => value()); }
    }


    public void BindData(ScenarioListItemResponseDto scenario)
    {
        id = scenario.scenarioCode;
        titleText.text = scenario.scenarioTitle;
    }
}