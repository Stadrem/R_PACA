using System;
using System.Collections.Generic;
using UnityEngine;

namespace Script.JYP.UniversePlay
{
    public class UniversePlayTest : MonoBehaviour
    {
        private void Start()
        {
            var instance = PlayUniverseManager.Instance;
        }


        private void Update()
        {
            TestingByKey();
        }

        #region Test

        private void TestingByKey()
        {
        }


        public void OnClickTest()
        {
            PlayUniverseManager.Instance.InGamePlayerManager.Init();
            PlayUniverseManager.Instance.BackgroundManager.Init();
        }

        #endregion
    }
}