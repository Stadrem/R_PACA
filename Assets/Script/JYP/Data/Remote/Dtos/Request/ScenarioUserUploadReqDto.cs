using System;
using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;

namespace Data.Remote.Dtos.Request
{
    [Serializable]
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
        public static ScenarioUserUploadReqDto ToSecenarioUserUploadDto(this UniversePlayerSettings settings)
        {
            return new ScenarioUserUploadReqDto
            {
                userCode = settings.UserId,
                scenarioCode = settings.UniverseId,
                health = settings.CharacterStats.GetStat(EStatType.Hp),
                strength = settings.CharacterStats.GetStat(EStatType.Str),
                dex = settings.CharacterStats.GetStat(EStatType.Dex)
            };
        }
    }
}