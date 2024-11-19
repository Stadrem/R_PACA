using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationConnect : MonoBehaviour
{
    //이동 가능한 상태일 때, 걷기 시 이펙트 재생
    Animator anim;

    public ParticleSystem walkEffect;

    private void Start()
    {
        if(transform.parent != null)
        {
            anim = transform.GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if(anim != null)
        {
            AnimatorStateInfo currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);

            if (currentStateInfo.IsName("Walk") && walkEffect.isPlaying)
            {
                return;
            }
            else if (!currentStateInfo.IsName("Walk") && walkEffect.isPlaying)
            {
                walkEffect.Stop();
            }
            else if (currentStateInfo.IsName("Walk"))
            {
                Debug.Log("재생중");
                walkEffect.Play();
            }
        }
    }
}
