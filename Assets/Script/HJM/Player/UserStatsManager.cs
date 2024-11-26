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

    int MaxStatTotal = 30;

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
            Max30.Get().notInput = false;
            int newTotal = health + UserStats.userStrength + UserStats.userDexterity;
            if (newTotal > MaxStatTotal)
            {
                Alert.Get().Set($"총합이 {MaxStatTotal}을 초과할 수 없습니다.");
                healthInput.text = UserStats.userHealth.ToString(); // 이전 값 복원
                Max30.Get().maxOver = true;
                return;
            }
            Max30.Get().maxOver = false;
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
            Max30.Get().notInput = false;
            int newTotal = UserStats.userHealth + strength + UserStats.userDexterity;
            if (newTotal > MaxStatTotal)
            {
                Alert.Get().Set($"총합이 {MaxStatTotal}을 초과할 수 없습니다.");
                strengthInput.text = UserStats.userStrength.ToString(); // 이전 값 복원
                Max30.Get().maxOver = true;
                return;
            }
            Max30.Get().maxOver = false;
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
            Max30.Get().notInput = false;
            int newTotal = UserStats.userHealth + UserStats.userStrength + dexterity;
            if (newTotal > MaxStatTotal)
            {
                Alert.Get().Set($"총합이 {MaxStatTotal}을 초과할 수 없습니다.");
                dexterityInput.text = UserStats.userDexterity.ToString(); // 이전 값 복원
                Max30.Get().maxOver = true;
                return;
            }
            Max30.Get().maxOver = false;
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