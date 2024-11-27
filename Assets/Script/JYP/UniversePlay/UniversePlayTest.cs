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
            if (Max30.Get().notInput)
            {
                Alert.Get().Set("스탯을 변경하세요.");
                return;
            }

            if (Max30.Get().maxOver)
            {
                Alert.Get().Set("스탯 최대값이 초과되었습니다.");
                return;
            }
            PlayUniverseManager.Instance.StartPlay();
        }

        #endregion
    }
}