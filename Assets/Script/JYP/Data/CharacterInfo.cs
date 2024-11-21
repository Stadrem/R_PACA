using System.Runtime.CompilerServices;
using Data.Models.Universe.Characters;
using UnityEngine;

public class CharacterInfo
{
    public int id;
    public int backgroundPartId;
    public string name;
    public UniverseNpc.ENpcType shapeType;
    public string description;
    public int hitPoints;
    public int strength;
    public int dexterity;
    public bool isPlayable;

    public Vector3 position; //local position
    public float yRotation;
    

    public CharacterInfo()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyFrom(CharacterInfo ci)
    {
        this.id = ci.id;
        this.backgroundPartId = ci.backgroundPartId;
        this.name = ci.name;
        this.shapeType = ci.shapeType;
        this.description = ci.description;
        this.hitPoints = ci.hitPoints;
        this.strength = ci.strength;
        this.dexterity = ci.dexterity;
        this.isPlayable = ci.isPlayable;
        this.position = ci.position;
        this.yRotation = ci.yRotation;
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