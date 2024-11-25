using System.Collections.Generic;
using Data.Models.Universe.Characters;

namespace UniversePlay
{
    public sealed partial class UniversePlayViewModel
    {
        private List<UniverseNpc> currentMapNpcList = new();


        private UniverseNpc currentInteractNpc;
        
        public UniverseNpc CurrentInteractNpc
        {
            get => currentInteractNpc;
            set => SetField(ref currentInteractNpc, value);
        }
        public List<UniverseNpc> CurrentMapNpcList
        {
            get => currentMapNpcList;
        }
    }
}