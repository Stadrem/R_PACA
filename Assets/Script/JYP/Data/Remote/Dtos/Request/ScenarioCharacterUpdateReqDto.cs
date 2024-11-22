[System.Serializable]
public class ScenarioCharacterUpdateReqDto
{
    public int scenarioAvatarId;
    /// <summary>
    ///     등장인물 이름
    /// </summary>
    public string avatarName;

    /// <summary>
    /// outfit id
    /// </summary>
    public int outfit;

    public string avatarDetail;
    /// <summary>
    /// 플레이가능 여부 (true: 플레이 가능, false: 플레이 불가능[NPC])
    /// </summary>
    public bool isPlayable;

    public int health;

    public int strength;

    public int dex;

    public float axisX;

    public float axisY;
    
    public float axisZ;
    public float rotation;
    
    public int worldId;
}