using UnityEngine;

public class NPCData : ICharacterData
{
    public string Name { get; set; }
    public string Description { get; set; }

    public int BackgroundPartId = -1;
    public Vector3 Position = Vector3.zero;

}