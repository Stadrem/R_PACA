using UnityEngine;

[System.Serializable]
public class NpcInfo
{
    public enum ENpcType
    {
        None = -1,
        Human,
        Goblin,
        Elf,
        Golem,
    }

    public int id;
    public string name;
    public string description ;
    public int hitPoints ;
    public int strength ;
    public int dexterity ;

    public int backgroundPartId = -1;
    public Vector3 position = Vector3.zero;
    public ENpcType npcShapeType = ENpcType.None;
    public float yRotation = 0;
    
}