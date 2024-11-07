using System;
using System.Linq;
using Cinemachine;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModels;

public class NpcInPlay : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private NpcInfo npcInfo;
    public string NpcName => npcInfo.Name;
    public int NpcId => npcInfo.Id;
    public NpcInfo.ENPCType ShapeType => npcInfo.Type;

    public CinemachineVirtualCamera ncVcam;
    public TMP_Text npcNameText;
    public Image hpBar;
    public GameObject root;
    private int currentHp;


    public void Init(NpcInfo npcInfo)
    {
        this.npcInfo = npcInfo;
        npcNameText.text = npcInfo.Name;
        currentHp = npcInfo.Hp;
    }

    /// <summary>
    /// 데미지를 받은 만큼 UI에 적용하는 함수
    /// </summary>
    /// <param name="damage">받는 데미지 수치</param>
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);
        hpBar.fillAmount = (float)currentHp / npcInfo.Hp;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var id = Convert.ToInt32(info.photonView.InstantiationData[0]);
        PlayUniverseManager.Instance.NpcManager.AddNpc(this);
        var npc = ViewModelManager.Instance.UniversePlayViewModel.UniverseData.backgroundPartDataList
            .SelectMany((t) => t.NpcList).First(t => t.Id == id);

        Init(npc);
    }
}