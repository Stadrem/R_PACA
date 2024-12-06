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
    public Button characterSettingButton;
    public Button objectiveSettingButton;
    public BackgroundPartLinkManager backgroundPartLinkManager;

    public Sprite[] backgroundImages;
    private UniverseEditViewModel ViewModel => ViewModelManager.Instance.UniverseEditViewModel;
    private void OnEnable()
    {
        root = gameObject;
        backgroundCreateUIController.gameObject.SetActive(false);
        
        characterSettingButton.onClick.AddListener(
            () => { UniverseEditUIFlowManager.Instance.ShowCharactersEdit(); }
        );

        backButton.onClick.AddListener(
            () => { UniverseEditUIFlowManager.Instance.ShowCreateUniverse(); }
        );
        createButton.onClick.AddListener(
            () => { backgroundCreateUIController.gameObject.SetActive(true); }
        );
        
        objectiveSettingButton.onClick.AddListener(
            () => { 
                UniverseEditUIFlowManager.Instance.ShowObjectiveSelection(
                () =>
                {
                    objectiveSettingButton.image.sprite = backgroundImages[1];
                }); 
                objectiveSettingButton.image.sprite = backgroundImages[0];
            }
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