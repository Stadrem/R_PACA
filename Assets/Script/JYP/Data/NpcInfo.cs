using UnityEngine;

public class NpcInfo : ICharacterData
{
    public enum ENPCType
    {
        None = -1,
        Human,
        Goblin,
        Elf,
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Hp { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }

    public int BackgroundPartId = -1;
    public Vector3 Position = Vector3.zero;
    public ENPCType Type = ENPCType.None;
}