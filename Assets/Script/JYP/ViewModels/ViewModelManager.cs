using UnityEngine;

namespace ViewModels
{
    public class ViewModelManager
    {
        private static ViewModelManager instance;

        public static ViewModelManager Instance => instance ??= new ViewModelManager();

        private UniverseEditViewModel universeEditViewModel = null;

        public UniverseEditViewModel UniverseEditViewModel => universeEditViewModel ??= new UniverseEditViewModel();

        public void Reset()
        {
            universeEditViewModel = null;
        }
    }
}