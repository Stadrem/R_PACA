﻿using System.Collections;
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

    // 오타 조심
    const string IDLE = "Idle";
    const string WALK = "Walk";

    CustomActions input;

    NavMeshAgent agent;

    public Animator animator;

    [Header("클릭이동 가능한 레이어")]
    [SerializeField] LayerMask clickableLayers; // 나중에 레이어 제대로 분리해서 땅만 선택 레이어에 넣기

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
        input = new CustomActions();
        AssignInputs();

    }
    private void Start()
    {
        // 내 플레이어라면 카메라를 활성화하자
        cam.SetActive(photonView.IsMine);

        PhotonNetwork.SendRate = 30; // 초당 전송 속도
        PhotonNetwork.SerializationRate = 10; // 초당 동기화 호출 횟수
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
            if (Quaternion.Dot(receiveRot, Quaternion.identity) > 0.0001f) // 유효한 쿼터니언인지 확인
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * lerpSpeed);
            }
        }
    }

    void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, clickableLayers))
        {
            // 목적지를 hit 포인트로 설정
            agent.destination = hit.point;

            // 클릭 이펙트 생성
            GameObject clickEffect = Instantiate(Resources.Load("ClickEffect") as GameObject, hit.point + new Vector3(0, 0.1f, 0), Quaternion.identity);

            // 일정 시간이 지나면 이펙트를 삭제
            StartCoroutine(DestroyAfterTime(clickEffect, 2.0f)); // 2초 후 삭제됨
            // 추후에 클릭이펙트가 플레이어 도착 시 사라지도록 수정
        }

    }
    IEnumerator DestroyAfterTime(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);  // delay 초 대기
        Destroy(go); // go를 삭제함
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

    // 애니메이션 재생 (테스트용으로 idle, walk 만), 다른 방식으로 애니메이션 넣어도 됨
    // 나중에 포톤 동기화해야함 !!! 아바타설정 후에 하면 될듯
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
}
