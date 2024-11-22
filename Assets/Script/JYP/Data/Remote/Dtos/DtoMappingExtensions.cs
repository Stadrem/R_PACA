using System.Collections.Generic;
using System.Linq;
using Data.Models.Universe.Characters;
using Data.Remote.Dtos.Response;
using UnityEngine;

namespace Data.Remote.Dtos
{
    public static class DtoMappingExtensions
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

        private static UniverseNpc ToUniverseNpc(this ScenarioAvatarResDto dto)
        {

            return new UniverseNpc(
                dto.scenarioAvatarId,
                dto.avatarName,
                dto.avatarDetail,
                new CharacterStats(dto.health, dto.strength, dto.dex),
                dto.rotation,
                new Vector3(dto.axisX, dto.axisY, dto.axisZ),
                (UniverseNpc.ENpcType)dto.outfit,
                dto.worldId
            );
        }

        private static BackgroundPartInfo ToBackgroundPartInfo(this ScenarioWorldPartResDto dto, int universeId,
            BackgroundPartInfo towards)
        {
            return new BackgroundPartInfo()
            {
                ID = dto.partId,
                Name = dto.partName,
                Type = (EBackgroundPartType)dto.backgroundType,
                UniverseId = universeId,
                TowardBackground = towards
            };
        }

        private static List<BackgroundPartInfo> ToBackgroundPartInfo(int universeId, List<ScenarioWorldPartResDto> dtos,
            List<ScenarioAvatarResDto> avatars)
        {
            if (dtos.Count == 0)
                return new List<BackgroundPartInfo>();

            dtos.Reverse();
            BackgroundPartInfo previous = null;

            var backgroundParts = dtos.Select(
                    (t) =>
                    {
                        var part = t.ToBackgroundPartInfo(universeId, previous);
                        part.PortalList = new List<PortalData>();
                        part.NpcList = new List<UniverseNpc>();
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
                .ForEach(
                    (t) =>
                    {
                        var part = backgroundParts.First((part) => part.ID == t.Key);
                        part.NpcList = t.Select((avatar) => avatar.ToUniverseNpc()).ToList();
                    }
                );

            return backgroundParts;
        }

        public static CharacterInfo ToCharacterInfo(this ScenarioCharacterUpdateResDto dto)
        {
            return new CharacterInfo()
            {
                id = dto.scenarioAvatarId,
                name = dto.avatarName,
                shapeType = (UniverseNpc.ENpcType)dto.outfit,
                isPlayable = dto.isPlayable,
                hitPoints = dto.health,
                description = dto.avatarDetail,
                strength = dto.strength,
                dexterity = dto.dex,
                position = new Vector3(dto.axisX, dto.axisY, dto.axisZ),
                yRotation = dto.rotation,
            };
        }
    }
}