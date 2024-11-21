using Data.Models.Universe.Characters;
using TMPro;
using UnityEngine;
using UniversePlay;
using ViewModels;

public class UserStatsManager : MonoBehaviour
{
    public UserStats UserStats;

    public TMP_InputField healthInput;
    public TMP_InputField strengthInput;
    public TMP_InputField dexterityInput;

    private UniversePlayViewModel ViewModel => ViewModelManager.Instance.UniversePlayViewModel;

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
            StartCoroutine(
                ViewModel.UpdateStatByUserCode(
                    UserCodeMgr.Instance.UserCode,
                    new CharacterStats(health, UserStats.userStrength, UserStats.userDexterity)
                )
            );
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
            StartCoroutine(
                ViewModel.UpdateStatByUserCode(
                    UserCodeMgr.Instance.UserCode,
                    new CharacterStats(UserStats.userHealth, strength, UserStats.userDexterity)
                )
            );
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
            StartCoroutine(
                ViewModel.UpdateStatByUserCode(
                    UserCodeMgr.Instance.UserCode,
                    new CharacterStats(UserStats.userHealth, UserStats.userStrength, dexterity)
                )
            );
        }
        else
        {
            Debug.LogError("유효한 숫자를 입력하세요.");
        }
    }
}