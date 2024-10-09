using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class LinkedBackgroundPart : MonoBehaviour, ILinkable
{
    public enum EViewType
    {
        LinkableView,
        DetailView,
    }

    public string Name;
    public EBackgroundPartType Type;
    public List<LinkedBackgroundPart> LinkedParts;
    public GameObject detail;
    public GameObject linkable;
    public CinemachineVirtualCamera detailViewCamera;
    private EViewType viewType = EViewType.LinkableView;

    public void Init(string name, EBackgroundPartType type)
    {
        Name = name;
        Type = type;
        LinkedParts = new List<LinkedBackgroundPart>();
    }

    public void Link(ILinkable linkable)
    {
        var backgroundPart = linkable as LinkedBackgroundPart;
        if (backgroundPart == null) return;

        if (LinkedParts.Contains(backgroundPart)) return;

        LinkedParts.Add(backgroundPart);
    }

    public void ChangeViewType(EViewType newViewType)
    {
        this.viewType = newViewType;
        switch (newViewType)
        {
            case EViewType.LinkableView:
                detail.SetActive(true);
                linkable.SetActive(false);
                break;
            case EViewType.DetailView:
                detail.SetActive(false);
                linkable.SetActive(true);
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