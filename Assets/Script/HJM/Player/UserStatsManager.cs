using UnityEngine;
using TMPro;

public class UserStatsManager : MonoBehaviour
{
    public UserStats UserStats;

    public TMP_InputField healthInput;
    public TMP_InputField strengthInput;
    public TMP_InputField dexterityInput;

    void Start()
    {

        UserStats = FindObjectOfType<UserStats>();

        if (UserStats == null) return;
        
        // 각 OnEndEdit 이벤트 추가
        healthInput.onEndEdit.AddListener(OnHealthInputEnd);
        strengthInput.onEndEdit.AddListener(OnStrengthInputEnd);
        dexterityInput.onEndEdit.AddListener(OnDexterityInputEnd);
        
        
    }

    // Health 입력이 끝났을 때 호출되는 함수
    private void OnHealthInputEnd(string input)
    {
        if (int.TryParse(input, out int health))
        {
            UserStats.userHealth = health;
        }
        else
        {
            Debug.LogError("유효한 숫자를 입력하세요.");
        }
    }

    // Strength 입력이 끝났을 때 호출되는 함수
    private void OnStrengthInputEnd(string input)
    {
        if (int.TryParse(input, out int strength))
        {
            UserStats.userStrength = strength;
        }
        else
        {
            Debug.LogError("유효한 숫자를 입력하세요.");
        }
    }

    // Dexterity 입력이 끝났을 때 호출되는 함수
    private void OnDexterityInputEnd(string input)
    {
        if (int.TryParse(input, out int dexterity))
        {
            UserStats.userDexterity = dexterity;
        }
        else
        {
            Debug.LogError("유효한 숫자를 입력하세요.");
        }
    }
}
