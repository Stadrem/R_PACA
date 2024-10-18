using System;
using System.Collections;
using System.Collections.Generic;


[Serializable]
public class Universe
{
    public int id;
    public int onwerUserid;
    public string mainObjective;
    public List<Objective> subObjectives;
    public List<Character> characters;
    public List<Background> backgrounds;
}

[Serializable]
public class Objective
{
    public string content;
}

[Serializable]
public class Character
{
    public string name;
    public int type;
    public List<Stat> stat;
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Stat
{
    public string name;
    public int value;
}

[Serializable]
public class Background
{
    public int id;
    public string name;
    public List<CustomObject> customObject;
    public List<Background> link;
}

[Serializable]
public class CustomObject
{
    public int objectType;
    public float x;
    public float y;
    public float z;
}
