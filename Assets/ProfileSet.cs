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

    private float currentHealthPercentage;  // 체력 비율을 저장하는 변수

    public void NicknameSet(string name)
    {
        nickName.text = name;
    }

    // 체력 초기화
    public void HpBarInit(int health)
    {
        hpBar.maxValue = health;
        hpBar.value = hpBar.maxValue;
    }

    // 데미지
    public void DamagedPlayer(int damage)
    {
        float damagePercentage = hpBar.value - damage;
        hpBar.value = damagePercentage;
      
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
}


