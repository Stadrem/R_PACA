using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModels;


public class BackgroundCreateUIController : MonoBehaviour
{
    public TMP_Dropdown backgroundTypeDropdown;
    public TMP_InputField backgroundNameInputField;
    public TMP_InputField backgroundDescriptionInputField;
    public Button createButton;

    private UniverseEditViewModel viewModel;

    private EBackgroundPartType selectedPartType = EBackgroundPartType.None;


    private void OnEnable()
    {
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        backgroundTypeDropdown.options.Clear();
        backgroundTypeDropdown.AddOptions(new List<string>() { "배경 선택..", "마을", "던전" });
        backgroundTypeDropdown.onValueChanged.AddListener(
            (value) => { selectedPartType = (EBackgroundPartType)(value - 1); }
        );

        createButton.onClick.AddListener(
            () =>
            {
                if (selectedPartType == EBackgroundPartType.None)
                {
                    Debug.Log("배경 종류를 선택해주세요.");
                    return;
                }

                StartCoroutine(
                    viewModel.CreateBackground(
                        backgroundNameInputField.text,
                        backgroundDescriptionInputField.text,
                        selectedPartType,
                        (res) =>
                        {
                            if (res.IsSuccess)
                            {
                                //Clear
                                ClearUIData();
                                gameObject.SetActive(false);
                            }
                            else
                            {
                                //todo: show error message
                                Debug.LogError($"배경 생성 실패: {res.error}");
                            }
                        }
                    )
                );

            }
        );
    }

    private void ClearUIData()
    {
        backgroundNameInputField.text = "";
        backgroundDescriptionInputField.text = "";
        backgroundTypeDropdown.value = 0;
        selectedPartType = EBackgroundPartType.None;
    }

    private void OnDisable()
    {
        backgroundTypeDropdown.onValueChanged.RemoveAllListeners();
        createButton.onClick.RemoveAllListeners();
    }
}