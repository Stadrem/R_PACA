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
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PlayUniverseManager.Instance.FinishConversation();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
            }
            else TestingByKey();
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