using Cinemachine;
using TMPro;
using UnityEngine;

public class NpcInPlay : MonoBehaviour
{
    private string _npcName;
    public string NpcName => _npcName;
    
    public CinemachineVirtualCamera ncVcam;
    public TMP_Text npcNameText;
    public void Init(string npcName)
    {
        npcNameText.text = npcName;
        this._npcName = npcName;
    }
}