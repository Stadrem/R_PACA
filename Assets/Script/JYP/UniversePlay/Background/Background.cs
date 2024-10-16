using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private int id;
    public int Id => id;
    private string name;


    public void Init(int id, string name, GameObject preset = null)
    {
        this.id = id;
        this.name = name;
        if (preset != null)
        {
            Instantiate(preset);
        }
    }
}