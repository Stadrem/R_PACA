using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class CoroutineExtensions
    {
        public static void DoAfterSeconds(this MonoBehaviour monoBehaviour, float seconds, System.Action callback)
        {
            monoBehaviour.StartCoroutine(DoAfterSeconds(seconds, callback));
        }

        private static IEnumerator DoAfterSeconds(float seconds, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }
    }
}