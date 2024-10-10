using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvatarSetting : MonoBehaviour
{
    GameObject[] avatarParts;

    // Start is called before the first frame update
    void Start()
    {
        avatarParts = new GameObject[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            avatarParts[i] = transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
