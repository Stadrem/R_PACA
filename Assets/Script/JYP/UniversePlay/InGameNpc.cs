using System;
using System.Linq;
using Cinemachine;
using Data.Models.Universe.Characters;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ViewModels;

public class InGameNpc : MonoBehaviourPun, IPunInstantiateMagicCallback
{
    private UniverseNpc universeNpcData;
    public int NpcId => universeNpcData.Id;
    public string NpcName => universeNpcData.Name;
    
    public CinemachineVirtualCamera ncVcam;
    public TMP_Text npcNameText;
    public GameObject root;


    public void Init(UniverseNpc npc)
    {
        universeNpcData = npc;
        npcNameText.text = npc.Name;
        transform.localPosition = npc.Position;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        print("OnPhotonInstantiate - " + info.photonView.InstantiationData[0]);
        var id = Convert.ToInt32(info.photonView.InstantiationData[0]);
        PlayUniverseManager.Instance.NpcManager.AddNpc(this);
        var npc = ViewModelManager.Instance.UniversePlayViewModel.CurrentMapNpcList
            .First(t => t.Id == id);
        
        Init(npc);
    }
}