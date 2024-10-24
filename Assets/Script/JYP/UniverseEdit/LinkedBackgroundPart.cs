using System;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;

public class LinkedBackgroundPart : MonoBehaviour, ILinkable, IDraggable
{
    public enum EViewType
    {
        LinkableView,
        DetailView,
    }

    public string backgroundPartName;
    public EBackgroundPartType backgroundPartType;
    public List<LinkedBackgroundPart> linkedParts;
    public List<Portal> portals;
    public GameObject detail;
    public Transform spawnOffset;
    public GameObject linkable;
    public CinemachineVirtualCamera detailViewCamera;
    private EViewType viewType = EViewType.LinkableView;
    public int backgroundPartId = -1;
    public TMP_Text nameText;

    public void Init(string name, EBackgroundPartType type)
    {
        backgroundPartName = name;
        backgroundPartType = type;
        nameText.text = name;
        linkedParts = new List<LinkedBackgroundPart>();
        switch (type)
        {
            case EBackgroundPartType.None:
                break;
            case EBackgroundPartType.Town:
                var townPreset = Resources.Load<GameObject>("BackgroundPart/TownDetailPreset");
                var go = Instantiate(townPreset, detail.transform);
                detailViewCamera.transform.position = go.transform.Find("DetailViewCamPos")
                    .transform.position;
                spawnOffset = go.transform.Find("SpawnOffset");
                go.transform.localPosition = Vector3.zero;
                break;
            case EBackgroundPartType.Dungeon:
                var dungeonPreset = Resources.Load<GameObject>("BackgroundPart/DungeonDetailPreset");
                var dungeon = Instantiate(dungeonPreset, detail.transform);
                detailViewCamera.transform.position = dungeon.transform.Find("DetailViewCamPos")
                    .transform.position;
                spawnOffset = dungeon.transform.Find("SpawnOffset");
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
        var portal = Instantiate(Resources.Load<GameObject>("BackgroundPart/Portal"), detail.transform)
            .GetComponent<Portal>();
        portal.transform.localPosition = Vector3.zero;
        portal.towardPart = backgroundPart;
        linkedParts.Add(backgroundPart);
        portals.Add(portal);
    }

    public void UnLink(ILinkable linkable)
    {
        var backgroundPart = linkable as LinkedBackgroundPart;
        if (backgroundPart == null) return;

        if (!linkedParts.Contains(backgroundPart)) return;

        var idx = linkedParts.IndexOf(backgroundPart);
        if (idx == -1) return;
        linkedParts.RemoveAt(idx);
        portals.RemoveAt(idx);
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

    public bool IsDraggable
    {
        get { return viewType == EViewType.LinkableView; }
    }

    public void StartDrag()
    {
        //do nothing
        return;
    }

    public void Dragging(Vector3 position)
    {
        this.transform.position = position;
    }

    public void StopDrag()
    {
        //do nothing
        return;
    }
}

public enum EBackgroundPartType
{
    None = -1,
    Town = 0,
    Dungeon = 1,
}