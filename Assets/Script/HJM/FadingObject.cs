using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingObject : MonoBehaviour, IEquatable<FadingObject>
{
    // 렌더러 리스트
    public List<Renderer> Renderers = new List<Renderer>();
    public Vector3 Position;
    // 머티리얼 리스트
    public List<Material> Materials = new List<Material>();
    // 초기 알파값
    [HideInInspector]
    public float InitialAlpha;

    private void Awake()
    {
        // 현재 위치를 저장
        Position = transform.position;

        // 
        // 렌더러의 수가 0이라면
        if(Renderers.Count == 0)
        {   // 이 컴포넌트(FadingObject)가 붙어있는 오브젝트에서 렌더러를 가져와서 리스트에 추가
            Renderers.AddRange(GetComponentsInChildren<Renderer>());
        }
        // Renderers 리스트에 포함된 모든 렌더러에 대해 순회한다.
        foreach (Renderer renderer in Renderers)
        {
            // 렌더러의 머티리얼을 가져와서 리스트에 추가(머티리얼 인스턴스 생성)
            Materials.AddRange(renderer.materials);
        }

        // 맨처음 머티리얼 값의 알파값을 초기화
        InitialAlpha = Materials[0].color.a;
    }

    // 현재 오브젝트와 다른 객체가 동일한지 비교하는 기능
    public bool Equals(FadingObject other)
    {
        return Position.Equals(other.Position);
    }
    // 동일한 해시코드를 반환하는지 확인하기위해 Equal이랑 세트
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
