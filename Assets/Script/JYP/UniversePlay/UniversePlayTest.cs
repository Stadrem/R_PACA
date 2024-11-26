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
                Alert.Get().Set("능력치를 입력하세요.");
                return;
            }

            if (Max30.Get().maxOver)
            {
                Alert.Get().Set("능력치가 초과되었습니다.");
                return;
            }

            Alert.Get().Set("시작중입니다...", 6.0f);
            PlayUniverseManager.Instance.StartPlay();
        }

        #endregion
    }
}