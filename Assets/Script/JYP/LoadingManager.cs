using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class LoadingManager : MonoBehaviourPun
{
    private static LoadingManager instance;

    public static LoadingManager Instance => instance;


    [SerializeField]
    private Canvas loadingCanvas;

    [SerializeField]
    private TMP_Text loadingText;

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void FinishLoading()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_FinishLoading), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_FinishLoading()
    {
        if (!isLoading)
        {
            return;
        }

        isLoading = false;
        StopAllCoroutines();
        loadingCanvas.gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        if (photonView.IsMine)
        {
            photonView.RPC(nameof(RPC_StartLoading), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_StartLoading()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;
        loadingCanvas.gameObject.SetActive(true);
        StartCoroutine(CoLoading());
    }

    private IEnumerator CoLoading()
    {
        while (isLoading)
        {
            loadingText.text = "로딩 중";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "로딩 중.";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "로딩 중..";
            yield return new WaitForSeconds(0.3f);
            loadingText.text = "로딩 중...";
            yield return new WaitForSeconds(0.3f);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Town" || scene.name == "Dungeon")
        {
            Invoke(nameof(FinishLoading), 0.1f);
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void Create()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }

        PhotonNetwork.Instantiate(
            "UniversePlay/LoadingManager",
            Vector3.zero,
            Quaternion.identity
        );
    }
}