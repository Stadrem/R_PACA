using UnityEngine;
using UnityEngine.UI;

public class BackgroundEditUIController : MonoBehaviour
{
    public GameObject root;
    public BackgroundCreateUIController backgroundCreateUIController;

    public Button createButton;
    public Button backButton;

    private UniverseEditViewModel viewModel;

    private void OnEnable()
    {
        root = gameObject;
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        backgroundCreateUIController.gameObject.SetActive(false);


        backButton.onClick.AddListener(
            () => { UniverseEditUIFlowManager.Instance.ShowCreateUniverse(); }
        );
        createButton.onClick.AddListener(
            () => { backgroundCreateUIController.gameObject.SetActive(true); }
        );
    }

    public void ShowUI()
    {
        root.SetActive(true);
    }

    public void HideUI()
    {
        root.SetActive(false);
    }
}