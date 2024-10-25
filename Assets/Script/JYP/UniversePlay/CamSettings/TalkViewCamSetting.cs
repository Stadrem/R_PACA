using Cinemachine;
using UnityEngine;

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
        PlayUniverseManager.Instance.NpcChatUIManager.Show();
    }

    public void OnUpdate()
    {
    }

    public void OnExit()
    {
        Vcam.gameObject.SetActive(false);
        Vcam.enabled = false;
        PlayUniverseManager.Instance.NpcChatUIManager.Hide();
        PlayUniverseManager.Instance.HideBattleUI();
        Vcam.Priority = 0;
    }
}