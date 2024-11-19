using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    //싱글톤
    public static SoundManager instance;

    Scene currentScene;

    int currentBGM = -1;

    int previousBGM = -1;

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

        // 현재 씬의 이름을 가져옴
        currentScene = SceneManager.GetActiveScene();
    }

    // 씬이 로드될 때마다 호출되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 현재 씬의 이름을 가져옴
        currentScene = SceneManager.GetActiveScene();

        ChangedBGM();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 오브젝트가 파괴될 때 이벤트 등록 해제 (중복 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void PlayBGM(int i)
    {
        if (currentBGM != i)
        {
            previousBGM = currentBGM;

            bgmAudioSource.Stop();

            bgmAudioSource.clip = bgmClips[i];

            bgmAudioSource.volume = bgmVolume;

            bgmAudioSource.loop = true;

            bgmAudioSource.Play();

            currentBGM = i;
        }
    }

    public void PlayBGM(int i, float volume)
    {
        if (currentBGM != i)
        {
            previousBGM = currentBGM;

            bgmAudioSource.Stop();

            bgmAudioSource.clip = bgmClips[i];

            bgmAudioSource.volume = bgmVolume * volume;

            bgmAudioSource.loop = true;

            bgmAudioSource.Play();

            currentBGM = i;
        }
    }

    // 볼륨 변경 메서드 추가
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;

        // 현재 재생 중인 BGM의 볼륨 업데이트
        if (bgmAudioSource.isPlaying)
        {
            bgmAudioSource.volume = bgmVolume;
        }
    }

    void ChangedBGM()
    {
        if(currentScene.name == "MainScene")
        {
            PlayBGM(0, 1);
        }
        else if(currentScene.name == "LobbyScene")
        {
            PlayBGM(1, 0.5f);
        }
        else if(currentScene.name == "Town")
        {
            PlayBGM(2, 0.5f);
        }
    }

    public void BattleBGMCall()
    {
        PlayBGM(3, 0.5f);
    }

    public void BattleBGMEnd()
    {
        PlayBGM(previousBGM, 0.5f);
    }
}