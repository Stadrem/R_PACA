using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private int id;
    public int Id => id;
    private string name;
    private bool isLoaded;


    public void Init(int id, string name, GameObject preset = null)
    {
        this.id = id;
        this.name = name;
        if (preset != null)
        {
            Instantiate(preset);
        }
    }

    public void LoadMap()
    {
        //get placed background parts
        //
    }

    public void Show()
    {
    }

    public void Hide()
    {
    }
}