using UnityEngine;

public class DestroyObj : MonoBehaviour
{
    
    public float lifetime = 5.0f;

    void Start()
    {
        
        Destroy(gameObject, lifetime);
    }
}
