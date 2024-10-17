using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCProfileController : MonoBehaviour
{
    public TMP_Text nameText;
    public Image profileImage;

    public void SetProfile(NpcData npcData)
    {
        nameText.text = npcData.Name;
        // profileImage.sprite = npcData.;
    }
    
}