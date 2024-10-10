using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarPresetSettings : MonoBehaviour
{
    [System.Serializable]
    public struct AvatarParts
    {
        [Header("itemCard 배열")]
        public AvatarItems[] avatarItems;
    }

    [System.Serializable]
    public struct AvatarItems
    {
        public string name;
        public SkinnedMeshRenderer mesh;
        public Material material;
        public Sprite icon;
    }

    [Header("Content 배열")]
    public AvatarParts[] avatarParts;
}
