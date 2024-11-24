using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveToScene : EditorWindow
{
    [MenuItem("Tools/Go to Main Scene %#x")]  // Ctrl+Shift+X or Cmd+Shift+X
    static void GoToMainScene()
    {
        if (Application.isPlaying)
        {
            // 게임 플레이 중일 경우, 게임을 종료한 후 0.5초 뒤에 씬을 전환
            EditorApplication.isPlaying = false; // 게임 종료
            EditorApplication.delayCall += () =>
            {
                // 게임 종료 후 씬 전환 전에 상태를 다시 확인
                if (!Application.isPlaying) // 게임이 종료되었는지 확인
                {
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/HJM/MainScene.unity");
                }
            };
        }
        else
        {
            // 게임이 실행 중이지 않을 때만 EditorSceneManager 사용
            if (SceneManager.GetActiveScene().name != "MainScene")
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/HJM/MainScene.unity");
            }
        }
    }
}
