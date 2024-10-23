using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObjectBlockingObject : MonoBehaviour
{
    [Header("장애물로 인식할 레이어")]
    [SerializeField]
    private LayerMask LayerMask;
    [SerializeField]
    private Transform Target;
    [SerializeField]
    private Camera Camera;

    [Header("투과한 장애물 투명도(1은 반투명)")]
    [SerializeField]
    [Range(0, 1.0f)]
    private float FadedAlpha = 0.33f;

    [SerializeField] // 그림자 유무인데 있는게 나은듯
    private bool RetainShadows = true;

    [Header("페이드 속도")]
    [SerializeField] // 페이드 속도인데 1이 무난
    private float FadeSpeed = 1;

    private Vector3 TargetPositionOffset = Vector3.up;

    [Header("레이캐스트에 걸린 장애물 목록")]
    [SerializeField]
    private List<FadingObject> objectsBlockingView = new List<FadingObject>();
    private Dictionary<FadingObject, Coroutine> RunningCorutines = new Dictionary<FadingObject, Coroutine>();

    private RaycastHit[] Hits = new RaycastHit[10];

    private void Start()
    {
        StartCoroutine(CheckForObjects());
    }

    private IEnumerator CheckForObjects()
    {
        while (true)
        {
            // 카메라와 타겟 사이의 거리 계산
            float distance = Vector3.Distance(Camera.transform.position, Target.transform.position + TargetPositionOffset);
            Vector3 direction = (Target.transform.position + TargetPositionOffset - Camera.transform.position).normalized;

            // 박스캐스트 크기 (가로, 세로, 깊이)
            Vector3 boxSize = new Vector3(1.0f, 1.0f, distance); // 직육면체의 크기

            // 박스캐스트로 충돌 감지
            int hits = Physics.BoxCastNonAlloc // NonAllo은 결과를 저장할 배열을 미리 마련해둠
            (
                Camera.transform.position,   // 시작 위치
                boxSize / 2,                 // 직육면체의 절반 크기
                direction,                   // 쏘는 방향
                Hits,                        // 충돌 결과를 저장할 RaycastHit 배열
                Camera.transform.rotation,   // 카메라의 회전 방향으로 쏘기
                distance,                    // 최대 거리
                LayerMask                    // 감지할 레이어
            );

            // 레이캐스트 가시화
            Debug.DrawRay(Camera.transform.position, direction * distance, Color.red); // 레이 방향
            Debug.DrawLine(Camera.transform.position, Target.transform.position, Color.blue); // 타겟과 카메라를 연결하는 선


            if (hits > 0)
            {
                // 충돌체 수 만큼 반복한다
                for (int i = 0; i < hits; i++)
                {
                    // 부딪힌 충돌체들에게서 FadingObject 컴포넌트 가져옴
                    FadingObject fadingObject = GetFadingObjectFromHit(Hits[i]);
                    // null이 아니고 objectsBlockingView에 이미 포함된 fadingObject가 아니라면,
                    if (fadingObject != null && !objectsBlockingView.Contains(fadingObject))
                    {
                        // 현재 fadingObject에 대한 코루틴이 이미 실행 중이라면,
                        if (RunningCorutines.ContainsKey(fadingObject))
                        {
                            // 실행 중인 코루틴이 null이 아니면 
                            if (RunningCorutines[fadingObject] != null)
                            {   // 중지
                                StopCoroutine(RunningCorutines[fadingObject]);
                            }

                            // 실행 중인 코루틴 딕셔너리에서 제거
                            RunningCorutines.Remove(fadingObject);
                        }

                        // 새로운 fadingObject에 대해 페이드 아웃 코루틴 시작
                        RunningCorutines.Add(fadingObject, StartCoroutine(FadeObjectOut(fadingObject)));
                        // 장애물 오브젝트 목록에 fadingObject 추가
                        objectsBlockingView.Add(fadingObject);
                    }
                }
            }

            FadingObjectsNoLongerBeingHit();
            ClearHits();

            yield return null;
        }
    }

    // 충돌 중이지 않은 장애물을 찾아서 페이드 아웃 효과를 적용하고,
    // 해당 장애물을 관리 리스트에서 제거하는 함수
    private void FadingObjectsNoLongerBeingHit()
    {
        // objectsToRemove 리스트 자리를 objectsBlockingView 수 만큼 준비
        List<FadingObject> objectsToRemove = new List<FadingObject>(objectsBlockingView.Count);

        // objectsBlockingView에 포함된 fadingObject에 대해 순회
        foreach (FadingObject fadingObject in objectsBlockingView)
        {
            // 부딪히는 중인지, 초기값은 false;
            bool objectIsBeingHit = false;
            // 충돌체의 수 만큼 반복한다. (부딪히고 있는 중의 오브젝트인지 판별하기 위한 반복문)
            for (int i = 0; i < Hits.Length; i++)
            {
                // 충돌체가 GetFadingObjectFromHit함수를 통해 반환한 FadingObject값을 hitFadingObject에 저장한다.
                FadingObject hitFadingObject = GetFadingObjectFromHit(Hits[i]);
                // hitFadingObject가 null이 아니고 fadingObject와 같다면
                if (hitFadingObject != null && fadingObject == hitFadingObject)
                {
                    // 부딪히고 있는 중의 오브젝트가 맞다.
                    objectIsBeingHit = true;
                    // 반복을 멈춘다.
                    break;
                }
            }

            // 부딪히고 있는 중의 충돌체라면(장애물이라면)
            if (!objectIsBeingHit)
            {
                // RunningCorutines 딕셔너리에 해당 장애물이 있는지 확인한다. 
                if (RunningCorutines.ContainsKey(fadingObject))
                {
                    // RunningCorutines에 해당 장애물이 있다면
                    if (RunningCorutines[fadingObject] != null)
                    {
                        // 코루틴을 중지한다.
                        StopCoroutine(RunningCorutines[fadingObject]);
                    }
                    // 해당 장애물을 딕셔너리에서 지운다.
                    RunningCorutines.Remove(fadingObject);
                }
                // 해당 장애물을 RunningCorutines 딕셔너리에 추가하고 FadeObjectIn 함수를 실행
                RunningCorutines.Add(fadingObject, StartCoroutine(FadeObjectIn(fadingObject)));
                // objectsToRemove 딕셔너리에 해당 장애물을 추가한다.
                objectsToRemove.Add(fadingObject);
            }
        }
        // objectsToRemove 딕셔너리의 removeObject에 대해 순회한다.
        foreach (FadingObject removeObject in objectsToRemove)
        {
            // objectsBlockingView 딕셔너리에서 해당 장애물을 제거한다.
            objectsBlockingView.Remove(removeObject);
        }
    }

    // 머티리얼 리스트의 머티리얼 옵션에 접근해서
    // Transparent로 바꾸고 투명도를 낮추는 페이드아웃 함수
    private IEnumerator FadeObjectOut(FadingObject FadingObject)
    {
        // FadingObject의 모든 머티리얼에 대해 투명도 관련 설정을 적용
        foreach (Material material in FadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha); // 소스 블렌드 설정
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor); // 대상 블렌드 설정
            material.SetInt("_ZWrite", 0); // Z 쓰기 비활성화
            material.SetInt("_Surface", 1); // 서페이스 타입 설정

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent; // 렌더 큐를 투명으로 설정

            material.SetShaderPassEnabled("DepthOnly", false); // 깊이 전용 패스 비활성화
            material.SetShaderPassEnabled("SHADOWCASTER", RetainShadows); // 그림자 캐스터 설정

            material.SetOverrideTag("RenderType", "Transparent"); // 렌더 타입을 투명으로 설정

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); // 투명 서페이스 키워드 활성화
            material.EnableKeyword("_ALPHAPREMULTIPLY_ON"); // 알파 프리멀티플라이 키워드 활성화
        }

        // 투명도 감소를 위한 시간 변수 초기화
        float time = 0;

        // FadingObject의 첫 번째 머티리얼의 알파 값이 FadedAlpha보다 클 때까지 반복
        while (FadingObject.Materials[0].color.a > FadedAlpha)
        {
            // 모든 머티리얼의 색상을 업데이트하여 알파 값을 점진적으로 감소
            foreach (Material material in FadingObject.Materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(FadingObject.InitialAlpha, FadedAlpha, time * FadeSpeed) // 현재 알파 값을 선형 보간하여 설정
                    );
                }
            }

            time += Time.deltaTime; // 시간 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        // 해당 FadingObject의 코루틴이 실행 중이라면 중지하고 딕셔너리에서 제거
        if (RunningCorutines.ContainsKey(FadingObject))
        {
            StopCoroutine(RunningCorutines[FadingObject]);
            RunningCorutines.Remove(FadingObject);
        }
    }
    // 머티리얼을 원래 설정값으로 되돌리고, 투명도를 증가시키는 함수
    private IEnumerator FadeObjectIn(FadingObject FadingObject)
    {
        // 시간 변수를 초기화
        float time = 0;

        // FadingObject의 첫 번째 머티리얼의 알파 값이 초기값보다 작을 때까지 반복
        while (FadingObject.Materials[0].color.a < FadingObject.InitialAlpha)
        {
            // 모든 머티리얼의 색상을 업데이트하여 알파 값을 점진적으로 증가
            foreach (Material material in FadingObject.Materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color(
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        Mathf.Lerp(FadedAlpha, FadingObject.InitialAlpha, time * FadeSpeed) // 현재 알파 값을 선형 보간하여 설정
                    );
                }
            }

            time += Time.deltaTime; // 시간 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        // 알파가 초기값에 도달한 후 머티리얼의 투명도 관련 설정을 원래대로 되돌림
        foreach (Material material in FadingObject.Materials)
        {
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One); // 소스 블렌드 설정
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero); // 대상 블렌드 설정
            material.SetInt("_ZWrite", 1); // Z 쓰기 활성화
            material.SetInt("_Surface", 0); // 서페이스 타입 설정

            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry; // 렌더 큐를 일반으로 설정

            material.SetShaderPassEnabled("DepthOnly", true); // 깊이 전용 패스 활성화
            material.SetShaderPassEnabled("SHADOWCASTER", true); // 그림자 캐스터 활성화

            material.SetOverrideTag("RenderType", "Opaque"); // 렌더 타입을 불투명으로 설정

            material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT"); // 투명 서페이스 키워드 비활성화
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // 알파 프리멀티플라이 키워드 비활성화
        }

        // 해당 FadingObject의 코루틴이 실행 중이라면 중지하고 딕셔너리에서 제거
        if (RunningCorutines.ContainsKey(FadingObject))
        {
            StopCoroutine(RunningCorutines[FadingObject]);
            RunningCorutines.Remove(FadingObject);
        }
    }
    // 레이캐스트 결과로 나온 충돌체 배열 초기화
    private void ClearHits()
    {
        System.Array.Clear(Hits, 0, Hits.Length);
    }

    // 레이캐스트로 찾은 충돌체가 콜라이더오브젝트에 붙어있는 FadingObject를 반환하고,
    // 그렇지 않으면 null을 반환한다.
    private FadingObject GetFadingObjectFromHit(RaycastHit Hit)
    {
        return Hit.collider != null ? Hit.collider.GetComponent<FadingObject>() : null;
    }
}
