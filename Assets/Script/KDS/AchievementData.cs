using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Game Data/Achievement")]
public class AchievementData : ScriptableObject
{
    public AchievementSet set;
}
