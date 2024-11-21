using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSet : MonoBehaviour
{
    public TMP_Text nickName;
    public Slider hpBar;
    public List<Image> selects; // [0]: 공격, [1]: 방어
    public TMP_Text hp;
    public GameObject turnBlur;

    int maxHealth;

    public void NicknameSet(string name)
    {
        nickName.text = name;
    }

    // 체력 초기화
    public void HpBarInit(int health)
    {
        maxHealth = health;  // 최대 체력
        hpBar.maxValue = health;
        hpBar.value = hpBar.maxValue;
        UpdateHpText();// 초기 체력
    }

    // 데미지
    public void DamagedPlayer(int damage)
    {
        float damagePercentage = hpBar.value - damage;
        hpBar.value = Mathf.Max(damagePercentage, 0);
        UpdateHpText();
    }

    // HP 텍스트 갱신
    private void UpdateHpText()
    {
        int currentHp = Mathf.RoundToInt(hpBar.value);
        hp.text = $"{currentHp} / {maxHealth}";
    }

    public void SetSelectImage(int select) // select[0]: 선택안함, [1]: 공격, [2]: 방어
    {
        foreach (var img in selects)
        {
            img.gameObject.SetActive(false);
        }

        if (select == 1) // [1]: 공격
        {
            selects[0].gameObject.SetActive(true);
        }
        else if (select == 2) // [2]: 방어
        {
            selects[1].gameObject.SetActive(true);
        }
    }

    
    public void OnLightProfile()
    {
        turnBlur.SetActive(true);
    }
    
    public void OffLightProfile()
    {
        turnBlur.SetActive(false);
    }
}
