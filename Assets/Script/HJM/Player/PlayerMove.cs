using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;
using Photon.Pun;

public class PlayerMove : MonoBehaviourPun
{
    // 카메라 
    public GameObject cam;

    CustomActions input;

    NavMeshAgent agent;

    public Animator animator;

    [Header("클릭이동 가능한 레이어")]
    [SerializeField] LayerMask clickableLayers;

    float lookRotationSpeed = 8.0f;

    public Transform lookPos;

    // 클릭 이동 가능 여부
    public bool clickMovementEnabled = true;
    GameObject currentClickEffect;
    // 서버에서 넘어오는 위치값
    Vector3 receivePos;
    // 서버에서 넘어오는 회전값
    Quaternion receiveRot;
    // 보정 속력
    public float lerpSpeed = 50.0f;



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        input = new CustomActions();
        AssignInputs();
    }
    private void Start()
    {
        // 내 플레이어라면 카메라를 활성화하자
        cam.SetActive(photonView.IsMine);

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
    }

    void AssignInputs()
    {
        if (photonView.IsMine)
        {
            input.Main.ClickMove.performed += ctx => ClickToMove();
        }
        // 나의 Player가 아니라면
        else
        {
            // 위치 보정
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * lerpSpeed);
            // 회전 보정
            if (Quaternion.Dot(receiveRot, Quaternion.identity) > 0.0001f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * lerpSpeed);
            }
        }
    }
    void ClickToMove()
    {
        // 클릭 이동이 활성화되어 있을 때만 이동 아니면 나감
        if (!clickMovementEnabled) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            agent.destination = hit.point;

            if (currentClickEffect != null)
            {
                Destroy(currentClickEffect);
            }

            // 클릭 이펙트 생성
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 adjustedRotation = rotation.eulerAngles;
            adjustedRotation.x += 90;
            currentClickEffect = Instantiate(Resources.Load("ClickEffect01") as GameObject, hit.point + new Vector3(0, 0.1f, 0), Quaternion.Euler(adjustedRotation));

            StartCoroutine(DestroyEffectOnArrival(currentClickEffect));
        }
    }

    IEnumerator DestroyEffectOnArrival(GameObject effect)
    {
        while (agent.pathPending || agent.remainingDistance > 0.1f)
        {
            yield return null;
        }
        if (effect != null)
        {
            Destroy(effect);

            if (currentClickEffect == effect)
            {
                currentClickEffect = null;
            }
        }
    }
    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        FaceTarget();
        SetAnimaions();

    }

    // 플레이어가 바라볼 방향 구하기
    void FaceTarget()
    {
        // 캐릭터가 이동 중일 때만 회전하도록 설정
        if (agent.velocity.magnitude > 0.1f)
        {
            Vector3 direction = (agent.destination - transform.position).normalized;

            // 목적지와의 거리 체크
            if (Vector3.Distance(transform.position, agent.destination) > agent.stoppingDistance)
            {
                lookPos.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookPos.rotation, Time.deltaTime * lookRotationSpeed);
            }
        }
    }

    void SetAnimaions()
    {
        if (photonView.IsMine)
        {
            if (agent.velocity == Vector3.zero)
            {
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalk", false);
            }
            else
            {
                animator.SetBool("isWalk", true);
                animator.SetBool("isIdle", false);
            }
        }

    }

    // 이동 가능 여부 설정하는 함수
    public void CallRPCSetMovement(bool value)
    {
        photonView.RPC("MovementEnabled", RpcTarget.All, value);
    }

    [PunRPC]
    public void MovementEnabled(bool enabled)
    {
        clickMovementEnabled = enabled;
    }

}
