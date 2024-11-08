using UnityEngine;

public class CharacterInfo
{
    public int id;
    public int backgroundPartId;
    public string name;
    public NpcInfo.ENpcType shapeType;
    public string description;
    public int hitPoints;
    public int strength;
    public int dexterity;
    public bool isPlayable;

    public Vector3 position;
    public float yRotation;
    

    public CharacterInfo()
    {
    }
    
    public CharacterInfo(CharacterInfo ci)
    {
        id = ci.id;
        backgroundPartId = ci.backgroundPartId;
        name = ci.name;
        shapeType = ci.shapeType;
        description = ci.description;
        hitPoints = ci.hitPoints;
        strength = ci.strength;
        dexterity = ci.dexterity;
        isPlayable = ci.isPlayable;
        position = ci.position;
        yRotation = ci.yRotation;
    }
    
}