using System.Collections.Generic;
using UnityEngine;

public class LinkedBackgroundPart: MonoBehaviour, ILinkable
{
    public string Name;
    public EBackgroundParkType Type;
    public List<LinkedBackgroundPart> LinkedParts;
    
    
    public void Init(string name, EBackgroundParkType type)
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
}

public enum EBackgroundParkType
{
    None = -1,
    Town = 0,
    Dungeon = 1,
}