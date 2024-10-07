using System.Collections.Generic;
using UnityEngine;

public class BackgroundPartLinker : MonoBehaviour
{
    public List<BackgroundPart> BackgroundParts = new List<BackgroundPart>();

    public void Create(string backgroundName, EBackgroundParkType type)
    {
        if(BackgroundParts.Exists(x => x.Name == backgroundName)) return;
        
        BackgroundParts.Add(new BackgroundPart
        {
            Name = backgroundName,
            Type = type,
            LinkedParts = new List<BackgroundPart>()
        });
    }
    
    public void UpdatePart(string originalName, string backgroundName, EBackgroundParkType type)
    {
        
        var backgroundPart = BackgroundParts.Find(x => x.Name == originalName);
        if (backgroundPart == null) return;
        
        backgroundPart.Name = backgroundName;
        backgroundPart.Type = type;
    }
    
    public void Delete(string backgroundName)
    {
        var backgroundPart = BackgroundParts.Find(x => x.Name == backgroundName);
        
        foreach (var part in backgroundPart.LinkedParts)
        {
            part.LinkedParts.Remove(backgroundPart);
        }
        
        BackgroundParts.Remove(BackgroundParts.Find(x => x.Name == backgroundName));
    }
    
    public void Link(BackgroundPart current, BackgroundPart next)
    {
        if (current.LinkedParts.Contains(next)) return;
        current.LinkedParts.Add(next);
        next.LinkedParts.Add(current);
    }
    
    public void Unlink(BackgroundPart current, BackgroundPart next)
    {
        if (!current.LinkedParts.Contains(next)) return;
        current.LinkedParts.Remove(next);
        next.LinkedParts.Remove(current);
    }
}