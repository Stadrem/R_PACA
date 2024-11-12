﻿using Data.Models.Universe.Characters.Player;

namespace Data.Remote.Dtos.Request
{
    [System.Serializable]
    public class ScenarioUserUploadReqDto
    {
        public int userCode;
        public int scenarioCode;
        public int health;
        public int strength;
        public int dex;
    }
    
    public static class ScenarioUserUploadReqDtoExtensions
    {
        public static ScenarioUserUploadReqDto ToSecenarioUserUploadDto(this UniverseUserSettings settings)
        {
            return new ScenarioUserUploadReqDto
            {
                userCode = settings.UserId,
                scenarioCode = settings.UniverseId,
                health = settings.CharacterStats.HitPoints,
                strength = settings.CharacterStats.Strength,
                dex = settings.CharacterStats.Dexterity
            };
        }
    }
}