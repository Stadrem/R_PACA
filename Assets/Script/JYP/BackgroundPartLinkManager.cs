using System.Collections.Generic;
using UnityEngine;

public class BackgroundPartLinkManager : MonoBehaviour
{
    public List<LinkedBackgroundPart> BackgroundParts = new List<LinkedBackgroundPart>();
    public float yAlignment = 0.5f;
    private bool _isLinking = false;
    private ILinkable _currentLinkable;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Move
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!_isLinking)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit)) return;

                _currentLinkable = hit.collider.GetComponent<ILinkable>();
                if (_currentLinkable == null) return;

                _isLinking = true;
            }

            else
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    var linkable = hit.collider.GetComponent<ILinkable>();
                    if (linkable != null)
                    {
                        linkable.Link(_currentLinkable);
                        _currentLinkable.Link(linkable);

                        var currentObject = (_currentLinkable as LinkedBackgroundPart)?.gameObject;
                        if (currentObject != null)
                        {
                            CreateLine(currentObject, hit.collider.gameObject);
                        }
                    }
                }


                _currentLinkable = null;
                _isLinking = false;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Create("Town", EBackgroundParkType.Town);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Create("Dungeon", EBackgroundParkType.Dungeon);
        }
    }

    private void CreateLine(GameObject obj1, GameObject obj2)
    {
        var line = new GameObject("Line");
        var lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, obj1.transform.position);
        lineRenderer.SetPosition(1, obj2.transform.position);
    }

    #region public methods

    public void Create(string backgroundName, EBackgroundParkType type)
    {
        if (BackgroundParts.Exists(x => x.Name == backgroundName)) return;

        // create object
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (type == EBackgroundParkType.Dungeon)
            go.GetComponent<Renderer>()
                .material.color = Color.red;
        else if (type == EBackgroundParkType.Town)
            go.GetComponent<Renderer>()
                .material.color = Color.green;
        go.transform.position = new Vector3(0, yAlignment, 0);
        go.name = backgroundName;
        var part = go.AddComponent<LinkedBackgroundPart>();
        part.Init(
            name: backgroundName,
            type: type
        );

        part.LinkedParts = new List<LinkedBackgroundPart>();

        BackgroundParts.Add(part);
    }

    public void UpdatePart(string originalName, string backgroundName, EBackgroundParkType type)
    {
        var backgroundPart = BackgroundParts.Find(x => x.Name == originalName);
        if (backgroundPart == null) return;

        backgroundPart.Name = backgroundName;
        backgroundPart.Type = type;
    }

    public void Delete(string backgroundName)
    {
        var backgroundPart = BackgroundParts.Find(x => x.Name == backgroundName);

        foreach (var part in backgroundPart.LinkedParts)
        {
            part.LinkedParts.Remove(backgroundPart);
        }

        BackgroundParts.Remove(BackgroundParts.Find(x => x.Name == backgroundName));
    }

    public void Link(LinkedBackgroundPart current, LinkedBackgroundPart next)
    {
        if (current.LinkedParts.Contains(next)) return;
        current.LinkedParts.Add(next);
        next.LinkedParts.Add(current);
    }

    public void Unlink(LinkedBackgroundPart current, LinkedBackgroundPart next)
    {
        if (!current.LinkedParts.Contains(next)) return;
        current.LinkedParts.Remove(next);
        next.LinkedParts.Remove(current);
    }

    #endregion

    #region private methods

    #endregion
}