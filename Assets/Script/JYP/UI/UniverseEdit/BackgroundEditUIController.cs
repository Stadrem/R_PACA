using UnityEngine;
using UnityEngine.UI;
using UniverseEdit;
using ViewModels;

public class BackgroundEditUIController : MonoBehaviour
{
    public GameObject root;
    public BackgroundCreateUIController backgroundCreateUIController;
    public RectTransform backgroundLinkMenuContainer;
    public Button createButton;
    public Button backButton;
    public BackgroundPartLinkManager backgroundPartLinkManager;
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

    public void SetDetailNpcMode()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(
            () => { backgroundPartLinkManager.ExitDetailMode(); }
        );
        createButton.gameObject.SetActive(false);
        backgroundLinkMenuContainer.gameObject.SetActive(false);
    }

    public void SetLinkMode()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(
            () => { UniverseEditUIFlowManager.Instance.ShowCreateUniverse(); }
        );
        createButton.gameObject.SetActive(true);
        backgroundLinkMenuContainer.gameObject.SetActive(true);
    }
}