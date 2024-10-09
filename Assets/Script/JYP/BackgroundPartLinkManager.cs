﻿using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BackgroundPartLinkManager : MonoBehaviour
{
    public List<LinkedBackgroundPart> backgroundParts = new List<LinkedBackgroundPart>();
    public float yAlignment = 0.5f;
    public CinemachineVirtualCamera linkViewCamera;
    private bool isLinking = false;

    private LinkedBackgroundPart currentPart;
    private ILinkable currentLinkable;

    private bool isDetailView = false;

    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        linkViewCamera.Priority = 20;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isLinking) return;
            if (!isDetailView)
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    var part = hit.collider.GetComponent<LinkedBackgroundPart>();
                    if (part != null)
                    {
                        isDetailView = true;
                        currentPart = part;
                        part.ChangeViewType(LinkedBackgroundPart.EViewType.DetailView);
                        part.detailViewCamera.Priority = 10;
                        linkViewCamera.Priority = 0;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!isLinking)
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                if (!Physics.Raycast(ray, out var hit)) return;

                currentLinkable = hit.collider.GetComponent<ILinkable>();
                if (currentLinkable == null) return;

                isLinking = true;
            }

            else
            {
                var ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    var linkable = hit.collider.GetComponent<ILinkable>();
                    if (linkable != null)
                    {
                        linkable.Link(currentLinkable);
                        currentLinkable.Link(linkable);

                        var currentObject = (currentLinkable as LinkedBackgroundPart)?.gameObject;
                        if (currentObject != null)
                        {
                            CreateLine(currentObject, hit.collider.gameObject);
                        }
                    }
                }


                currentLinkable = null;
                isLinking = false;
            }
        }

        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Create("Town", EBackgroundPartType.Town);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Create("Dungeon", EBackgroundPartType.Dungeon);
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isDetailView) return;
            isDetailView = false;
            currentPart.ChangeViewType(LinkedBackgroundPart.EViewType.LinkableView);
            currentPart.detailViewCamera.Priority = 0;
            linkViewCamera.Priority = 20;
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

    public void Create(string backgroundName, EBackgroundPartType type)
    {
        if (backgroundParts.Exists(x => x.Name == backgroundName)) return;

        var go = Resources.Load<GameObject>("BackgroundPart/LinkableBackgroundPart");
        go = Instantiate(go, new Vector3(0, yAlignment, 0), Quaternion.identity);
        var part = go.GetComponent<LinkedBackgroundPart>();
        part.Init(backgroundName, type);

        backgroundParts.Add(part);
    }

    public void UpdatePart(string originalName, string backgroundName, EBackgroundPartType type)
    {
        var backgroundPart = backgroundParts.Find(x => x.Name == originalName);
        if (backgroundPart == null) return;

        backgroundPart.Name = backgroundName;
        backgroundPart.Type = type;
    }

    public void Delete(string backgroundName)
    {
        var backgroundPart = backgroundParts.Find(x => x.Name == backgroundName);

        foreach (var part in backgroundPart.LinkedParts)
        {
            part.LinkedParts.Remove(backgroundPart);
        }

        backgroundParts.Remove(backgroundParts.Find(x => x.Name == backgroundName));
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