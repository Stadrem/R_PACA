using UnityEngine;
using UnityEngine.UI;

public class HpBarSystem : MonoBehaviour
{
    public Image hpBar;
    public GameObject root;
    private int maxHp;
    private int currentHp;
    
    public void Init(int hp)
    {
        maxHp = hp;
        currentHp = hp;
        HideHpBar();
    }
    
    public void ShowHpBar()
    {
        root.gameObject.SetActive(true);
    }
    public void HideHpBar()
    {
        root.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 데미지를 받은 만큼 UI에 적용하는 함수
    /// </summary>
    /// <param name="damage"> 받은 데미지 </param>
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Max(0, currentHp);
        hpBar.fillAmount = (float)currentHp / maxHp;
    }
}