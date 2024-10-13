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


    const string IDLE = "Idle";
    const string WALK = "Walk";

    CustomActions input;

    NavMeshAgent agent;

    Animator animator;

    [Header("Movement")]
    //[SerializeField] ParticleSystem clickEffect;
    [SerializeField] LayerMask clickableLayers;

    float lookRotationSpeed = 8.0f;

    public Transform lookPos;


    // 서버에서 넘어오는 위치값
    Vector3 receivePos;
    // 서버에서 넘어오는 회전값
    Quaternion receiveRot;
    // 보정 속력
    public float lerpSpeed = 50.0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        input = new CustomActions();
        AssignInputs();

    }
    private void Start()
    {
        // 내 플레이어라면 카메라를 활성화하자
        cam.SetActive(photonView.IsMine);
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
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * lerpSpeed);
        }
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            // 목적지를 hit포인트로 설정
            agent.destination = hit.point;

            // 이동클릭 이펙트 생성
            // 시간지나면 사라지게
            GameObject clickEffect = PhotonNetwork.Instantiate("ClickEffect", hit.point += new Vector3(0, 0.1f, 0), Quaternion.identity);
            // 일정 시간이 지나면 이펙트를 삭제
            StartCoroutine(DestroyAfterTime(clickEffect, 2.0f)); // 2초 후 삭제됨
        }

    }
    IEnumerator DestroyAfterTime(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);  // delay 초 대기
        PhotonNetwork.Destroy(go); // go를 삭제함
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

    // 애니메이션 재생 (테스트용으로 걷기만) 
    void SetAnimaions()
    {
        if (agent.velocity == Vector3.zero)
        {
            animator.Play(IDLE);
        }
        else
        {
            animator.Play(WALK);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 만약에 내가 데이터를 보낼 수 있는 상태라면 (내 것이라면)
        if (stream.IsWriting)
        {
            // 나의 위치값을 보낸다.
            stream.SendNext(transform.position);
            // 나의 회전값을 보낸다.
            stream.SendNext(transform.rotation);
            // LookPos 의 위치값을 보낸다.
            stream.SendNext(lookPos);
        }
        // 데이터를 받을 수 있는 상태라면 (내 것이 아나라면)
        else if (stream.IsReading)
        {
            // 위치값을 받자.
            receivePos = (Vector3)stream.ReceiveNext();
            // 회전값을 받자.
            receiveRot = (Quaternion)stream.ReceiveNext();
            // LookPos 의 위치값을 받자.
            lookPos.position = (Vector3)stream.ReceiveNext();
        }
    }
}
