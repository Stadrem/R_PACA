using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    [SerializeField]
    public List<Universe> uni;

    public Temp temp1;

    // Start is called before the first frame update
    void Start()
    {
        temp1 = GetComponent<Temp>();
        string temp = JsonUtility.ToJson(temp1, true);

        print(temp);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
