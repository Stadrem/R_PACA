using System.Collections.Generic;
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
    private List<LinkedLine> lines = new List<LinkedLine>();
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
        else if (Input.GetKeyDown(KeyCode.D))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var part = hit.collider.GetComponent<LinkedBackgroundPart>();
                if (part != null)
                {
                    Delete(part.backgroundPartName);
                    Destroy(part.gameObject);
                }
            }
        }
    }

    private void CreateLine(GameObject obj1, GameObject obj2)
    {
        var line = new GameObject("Line");
        line.layer = LayerMask.NameToLayer("Line");
        var linkedLine = line.AddComponent<LinkedLine>();
        var part1 = obj1.GetComponent<LinkedBackgroundPart>();
        var part2 = obj2.GetComponent<LinkedBackgroundPart>();
        linkedLine.Init(part1, part2);
        lines.Add(linkedLine);
    }

    #region public methods

    public void Create(string backgroundName, EBackgroundPartType type)
    {
        if (backgroundParts.Exists(x => x.backgroundPartName == backgroundName)) return;

        var go = Resources.Load<GameObject>("BackgroundPart/LinkableBackgroundPart");
        go = Instantiate(go, new Vector3(0, yAlignment, 0), Quaternion.identity);
        var part = go.GetComponent<LinkedBackgroundPart>();
        part.Init(backgroundName, type);

        backgroundParts.Add(part);
    }

    public void UpdatePart(string originalName, string backgroundName, EBackgroundPartType type)
    {
        var backgroundPart = backgroundParts.Find(x => x.backgroundPartName == originalName);
        if (backgroundPart == null) return;

        backgroundPart.backgroundPartName = backgroundName;
        backgroundPart.backgroundPartType = type;
    }

    public void Delete(string backgroundName)
    {
        var backgroundPart = backgroundParts.Find(x => x.backgroundPartName == backgroundName);

        foreach (var part in backgroundPart.linkedParts)
        {
            part.linkedParts.Remove(backgroundPart);
        }

        foreach (var line in lines.FindAll(x => x.start == backgroundPart || x.end == backgroundPart))
        {
            line.DeleteLine();
        }

        backgroundParts.Remove(backgroundPart);
    }

    public void Link(LinkedBackgroundPart current, LinkedBackgroundPart next)
    {
        if (current.linkedParts.Contains(next)) return;
        current.linkedParts.Add(next);
        next.linkedParts.Add(current);
    }

    public void Unlink(LinkedBackgroundPart current, LinkedBackgroundPart next)
    {
        if (!current.linkedParts.Contains(next)) return;
        current.linkedParts.Remove(next);
        next.linkedParts.Remove(current);
    }

    #endregion

    #region private methods

    private void DeleteLine(LinkedLine line)
    {
        lines.Remove(line);
        line.DeleteLine();
    }

    #endregion
}