using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class LinkedBackgroundPart : MonoBehaviour, ILinkable
{
    public enum EViewType
    {
        LinkableView,
        DetailView,
    }

    public string backgroundPartName;
    public EBackgroundPartType backgroundPartType;
    public List<LinkedBackgroundPart> linkedParts;
    public GameObject detail;
    public GameObject linkable;
    public CinemachineVirtualCamera detailViewCamera;
    private EViewType viewType = EViewType.LinkableView;

    public void Init(string name, EBackgroundPartType type)
    {
        backgroundPartName = name;
        backgroundPartType = type;
        linkedParts = new List<LinkedBackgroundPart>();
        switch (type)
        {
            case EBackgroundPartType.None:
                break;
            case EBackgroundPartType.Town:
                var townPreset = Resources.Load<GameObject>("BackgroundPart/TownDetailPreset");
                var go = Instantiate(townPreset, detail.transform);
                go.transform.localPosition = Vector3.zero;
                break;
            case EBackgroundPartType.Dungeon:
                var dungeonPreset = Resources.Load<GameObject>("BackgroundPart/DungeonDetailPreset");
                var dungeon = Instantiate(dungeonPreset, detail.transform);
                dungeon.transform.localPosition = Vector3.zero;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Link(ILinkable linkable)
    {
        var backgroundPart = linkable as LinkedBackgroundPart;
        if (backgroundPart == null) return;

        if (linkedParts.Contains(backgroundPart)) return;

        linkedParts.Add(backgroundPart);
    }

    public void ChangeViewType(EViewType newViewType)
    {
        this.viewType = newViewType;
        switch (newViewType)
        {
            case EViewType.LinkableView:
                detail.SetActive(false);
                linkable.SetActive(true);
                break;
            case EViewType.DetailView:
                detail.SetActive(true);
                linkable.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newViewType), newViewType, null);
        }
    }
}

public enum EBackgroundPartType
{
    None = -1,
    Town = 0,
    Dungeon = 1,
}