using UnityEngine;

public class NPCData : ICharacterData
{
    public string Name { get; set; }
    public string Description { get; set; }

    public int PlacedBackgroundPartId = -1;
    public Vector3 PlacedPosition = Vector3.zero;

    public NPCData(string name, string description)
    {
        Name = name;
        Description = description;
    }
}