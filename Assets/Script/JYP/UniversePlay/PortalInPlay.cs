using UnityEngine;
using ViewModels;

public class PortalInPlay : MonoBehaviour
{


    public void InteractByUser()
    {
        ViewModelManager.Instance.UniversePlayViewModel.GoToNextBackground();
    }
}