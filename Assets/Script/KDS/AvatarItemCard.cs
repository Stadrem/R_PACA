using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarItemCard : MonoBehaviour
{
    public int part = 0;

    public int itemNum = 0;

    public string itemName = "";

    public SkinnedMeshRenderer mesh = null;

    public Material material = null;

    public Sprite sprite;

    public void OnClickGender()
    {
        AvatarCanvasManager.instance.PushAvatarCode(part, itemNum);
    }
}
