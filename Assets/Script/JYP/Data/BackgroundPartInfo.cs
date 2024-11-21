using System.Collections.Generic;
using Data.Models.Universe.Characters;

[System.Serializable]
public sealed class BackgroundPartInfo 
{
    public int ID;
    public int UniverseId;
    public string Description;
    public string Name;
    public bool IsRoot = false;
    public EBackgroundPartType Type;
    public BackgroundPartInfo TowardBackground = null;
    public List<PortalData> PortalList;
    public List<UniverseNpc> NpcList;

    public BackgroundPartInfo()
    {
        
    }
    public BackgroundPartInfo(BackgroundPartInfo partInfo)
    {
        ID = partInfo.ID;
        UniverseId = partInfo.UniverseId;
        Description = partInfo.Description;
        Name = partInfo.Name;
        IsRoot = partInfo.IsRoot;
        Type = partInfo.Type;
        TowardBackground = partInfo.TowardBackground;
        PortalList = partInfo.PortalList;
        NpcList = partInfo.NpcList;
    }
}