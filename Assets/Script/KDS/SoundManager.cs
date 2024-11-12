using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
