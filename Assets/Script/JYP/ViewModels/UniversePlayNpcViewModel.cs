using System.Collections.Generic;
using Data.Models.Universe.Characters;

namespace UniversePlay
{
    public sealed partial class UniversePlayViewModel
    {
        private List<UniverseNpc> currentMapNpcList = new();


        public List<UniverseNpc> CurrentMapNpcList
        {
            get => currentMapNpcList;
        }
    }
}