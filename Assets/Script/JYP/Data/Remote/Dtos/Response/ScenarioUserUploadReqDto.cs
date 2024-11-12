using Data.Models.Universe.Characters;
using Data.Models.Universe.Characters.Player;

namespace Data.Remote.Dtos.Response
{
    [System.Serializable]
    public class ScenarioUserResponseDto
    {
        public int id;
        public int userCode;
        public int scenarioCode;
        public int health;
        public int strength;
        public int dex;
    }

    public static class ScenarioUserResponseDtoExtensions
    {
        public static UniverseUserSettings ToUniverseUserSettings(this ScenarioUserResponseDto dto)
        {
            return new UniverseUserSettings(
                dto.scenarioCode,
                dto.userCode,
                new CharacterStats(dto.health, dto.strength, dto.dex)
            );
        }
    }
}