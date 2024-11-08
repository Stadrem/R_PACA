using System.Collections.Generic;
using System.Linq;
using Data.Remote.Dtos.Response;

namespace Data.Remote.Dtos
{
    public static class MappingExtensions
    {
        public static UniverseData ToUniverse(this ScenarioResDto dto)
        {
            return new UniverseData()
            {
                id = dto.scenarioCode,
                name = dto.scenarioTitle,
                backgroundPartDataList = ToBackgroundPartInfo(dto.scenarioCode, dto.worldParts, dto.scenarioAvatarList)
            };
        }

        public static NpcInfo ToNpcInfo(this ScenarioAvatarResDto dto)
        {
            return new NpcInfo()
            {
                id = dto.scenarioAvatarId,
                name = dto.avatarName,
                npcShapeType = (NpcInfo.ENpcType)dto.outfit,
                hitPoints = dto.health,
                strength = dto.strength,
                dexterity = dto.dex,
                backgroundPartId = dto.worldId,
                position = new UnityEngine.Vector3(dto.axisX, dto.axisY, dto.axisZ),
                yRotation = dto.rotation,
            };
        }

        public static BackgroundPartInfo ToBackgroundPartInfo(this ScenarioWorldPartResDto dto, int universeId,
            BackgroundPartInfo towards)
        {
            return new BackgroundPartInfo()
            {
                ID = dto.partId,
                Name = dto.partName,
                UniverseId = universeId,
                TowardBackground = towards
            };
        }

        private static List<BackgroundPartInfo> ToBackgroundPartInfo(int universeId, List<ScenarioWorldPartResDto> dtos,
            List<ScenarioAvatarResDto> avatars)
        {
            if (dtos.Count == 0)
                return new List<BackgroundPartInfo>();

            var result = new BackgroundPartInfo[dtos.Count];
            dtos.Reverse();
            BackgroundPartInfo previous = null;

            var backgroundParts = dtos.Select(
                    (t) =>
                    {
                        var part = t.ToBackgroundPartInfo(universeId, previous);
                        part.PortalList = new List<PortalData>();
                        part.NpcList = new List<NpcInfo>();
                        previous = part;
                        return part;
                    }
                ).Reverse()
                .ToList();

            backgroundParts.First().IsRoot = true;


            avatars.GroupBy(
                (avatar) => avatar.worldId
            ).Where((t) => t.Key != -1)
                .ToList()
                .ForEach((t) =>
                {
                    var part = backgroundParts.First((part) => part.ID == t.Key);
                    part.NpcList = t.Select((avatar) => avatar.ToNpcInfo()).ToList();
                });

            return backgroundParts;
        }

        public static CharacterInfo ToCharacterInfo(this ScenarioCharacterUpdateResDto dto)
        {
            return new CharacterInfo()
            {
                id = dto.scenarioAvatarId,
                name = dto.avatarName,
                shapeType = (NpcInfo.ENpcType)dto.outfit,
                isPlayable = dto.isPlayable,
                hitPoints = dto.health,
                strength = dto.strength,
                dexterity = dto.dex,
                position = new UnityEngine.Vector3(dto.axisX, dto.axisY, dto.axisZ),
                yRotation = dto.rotation,
            };
        }
    }
}