using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        backgroundTypeDropdown.AddOptions(new List<string>() { "배경 선택..","마을", "던전" });
        backgroundTypeDropdown.onValueChanged.AddListener(
            (value) => { selectedPartType = (EBackgroundPartType)(value -1); }
        );

        createButton.onClick.AddListener(
            () =>
            {
                if (selectedPartType == EBackgroundPartType.None)
                {
                    Debug.Log("배경 종류를 선택해주세요.");
                    return;
                }

                viewModel.AddBackgroundPart(
                    backgroundNameInputField.text,
                    backgroundDescriptionInputField.text,
                    selectedPartType
                );

                //Clear
                backgroundNameInputField.text = "";
                backgroundDescriptionInputField.text = "";
                backgroundTypeDropdown.value = 0;
                selectedPartType = EBackgroundPartType.None;

                gameObject.SetActive(false);
            }
        );
    }

    private void OnDisable()
    {
        backgroundTypeDropdown.onValueChanged.RemoveAllListeners();
        createButton.onClick.RemoveAllListeners();
    }
}