using UnityEngine;

namespace Data.Local
{
    public static class PlayerPreferencesManager
    {
        private const string CreateUniverseTutorialKey = "CreateUniverseTutorialKey";

        public static bool IsCreateUniverseTutorialNeed
        {
            get => PlayerPrefs.GetInt(CreateUniverseTutorialKey, 0) == 1;
            set => PlayerPrefs.SetInt(CreateUniverseTutorialKey, value ? 1 : 0);
        }
    }
}