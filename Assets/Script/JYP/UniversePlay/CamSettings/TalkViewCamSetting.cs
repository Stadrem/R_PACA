using Cinemachine;
using Data.Models.Universe;
using UnityEngine;
using ViewModels;

public class TalkViewCamSetting : ICamSettingState
{
    
    private CinemachineVirtualCamera Vcam => PlayUniverseManager.Instance.NpcManager.CurrentNpcVcam;
    public TalkViewCamSetting()
    {
    }
    public void OnEnter()
    {
        if (!Vcam.gameObject.activeSelf)
        {
            Vcam.gameObject.SetActive(true);
        }
        
        Vcam.Priority = 10;
        // Vcam.transform.localPosition = new Vector3(0, 6, -6);
        
        PlayUniverseManager.Instance.NpcChatUIManager.Show();
        ViewModelManager.Instance.UniversePlayViewModel.AddHUDState(EHUDState.Chat);
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        Vcam.gameObject.SetActive(false);
        Vcam.enabled = false;
        PlayUniverseManager.Instance.NpcChatUIManager.Hide();
        ViewModelManager.Instance.UniversePlayViewModel.RemoveHUDState(EHUDState.Chat);
        Vcam.Priority = 0;
    }
}