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
                Id = dto.scenarioAvatarId,
                Name = dto.avatarName,
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
            if(dtos.Count == 0)
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


            //todo: 배치된 맵 확인, 현재는 테스트를 위해 일단 첫번째에 넣는다

            backgroundParts.First().NpcList = avatars
                .Select((t) => t.ToNpcInfo())
                .ToList();


            return backgroundParts;
        }
    }
}