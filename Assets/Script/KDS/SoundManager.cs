using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    //싱글톤
    public static SoundManager instance;

    public static SoundManager Get()
    {
        if (instance == null)
        {
            // 프리팹 로드
            GameObject prefabToInstantiate = Resources.Load<GameObject>("scripts/SoundManager");
            if (prefabToInstantiate != null)
            {
                // 프리팹 생성
                GameObject newInstance = Instantiate(prefabToInstantiate);
                instance = newInstance.GetComponent<SoundManager>();

                if (instance == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return instance;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 마우스 클릭 감지
        if (Input.GetMouseButtonDown(0))  // 왼쪽 클릭
        {
            // 클릭한 UI 오브젝트가 버튼인지 확인
            if (EventSystem.current.currentSelectedGameObject != null &&
                EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.Button>() != null)
            {
                PlaySFX(1);
            }
        }
    }

    public AudioClip[] sfxClips;
    public AudioClip[] bgmClips;

    public float sfxVolume = 1;
    public float bgmVolume = 1;

    public AudioSource sfxAudioSource;
    public AudioSource bgmAudioSource;

    public void PlaySFX(int num)
    {
        sfxAudioSource.PlayOneShot(sfxClips[num], sfxVolume);
    }

    public void PlayBGM(int num)
    {
        bgmAudioSource.PlayOneShot(bgmClips[num], bgmVolume);
    }
}
