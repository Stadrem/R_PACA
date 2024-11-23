using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToScene : EditorWindow
{
    private static float lastPressTime = 0f; // 마지막으로 버튼을 눌렀을 때의 시간
    private const float doublePressThreshold = 0.5f; // 두 번 눌렀을 때의 시간 간격 (0.5초)

    [MenuItem("Tools/Go to Main Scene %#x")]  // Ctrl+Shift+X or Cmd+Shift+X
    static void GoToMainScene()
    {
        // 현재 시간을 확인
        float currentPressTime = Time.realtimeSinceStartup;

        if (currentPressTime - lastPressTime < doublePressThreshold)
        {
            // 두 번 눌린 경우, 게임을 종료한 후 씬을 로드하고 바로 플레이 모드로 전환
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false; // 게임 종료
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/HJM/MainScene.unity");

            // 씬을 로드한 후 플레이 모드 시작
            EditorApplication.isPlaying = true;
        }
        else
        {
            // 첫 번째 눌림: 게임이 실행 중이면 게임 종료 후 씬을 전환, 아니면 바로 씬 전환
            if (Application.isPlaying)
            {
                // 게임이 실행 중일 때는 잠시 기다린 후 씬을 전환
                EditorApplication.isPlaying = false; // 게임 종료
                EditorApplication.delayCall += () =>
                {
                    if (!Application.isPlaying)
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/HJM/MainScene.unity");
                    }
                };
            }
            else
            {
                // 게임이 실행 중이 아니면 바로 씬 전환
                if (SceneManager.GetActiveScene().name != "MainScene")
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/HJM/MainScene.unity");
                }
            }
        }

        // 마지막으로 눌린 시간 갱신
        lastPressTime = currentPressTime;
    }
}
