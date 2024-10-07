using System.Collections.Generic;

public class BackgroundPart
{
    public string Name;
    public EBackgroundParkType Type;
    public List<BackgroundPart> LinkedParts;
}

public enum EBackgroundParkType
{
    None = -1,
    Town = 0,
    Dungeon = 1,
}