using System.Collections.Generic;
using UnityEngine;

public class BackgroundPart: MonoBehaviour, ILinkable
{
    public string Name;
    public EBackgroundParkType Type;
    public List<BackgroundPart> LinkedParts;
    
    
    public void Init(string name, EBackgroundParkType type)
    {
        Name = name;
        Type = type;
        LinkedParts = new List<BackgroundPart>();
    }
    public void Link(ILinkable linkable)
    {
        
    }
}

public enum EBackgroundParkType
{
    None = -1,
    Town = 0,
    Dungeon = 1,
}