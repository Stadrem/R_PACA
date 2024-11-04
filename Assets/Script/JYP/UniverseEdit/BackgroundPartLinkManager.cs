using System.Collections.Generic;
using System.ComponentModel;
using Cinemachine;
using UnityEngine;
using ViewModels;

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
    private List<LinkedBackgroundPart> linkedParts = new List<LinkedBackgroundPart>();

    [SerializeField] private NPCSpawner npcSpawner;
    [SerializeField] private BackgroundEditUIController backgroundEditUIController;
    [SerializeField] private BackgroundDetailCameraMove backgroundDetailCameraMove;
    private UniverseEditViewModel EditViewModel => ViewModelManager.Instance.UniverseEditViewModel;

    private void Start()
    {
        camera = Camera.main;
        linkViewCamera.Priority = 20;
        npcSpawner.Init();
        EditViewModel.PropertyChanged += OnViewModelPropertyChanged;
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
                        ShowBackgroundPartDetailView(part);
                    }
                }
            }
        }

        else if (Input.GetMouseButtonDown(1))
        {
            Link();
        }

        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isDetailView) return;
            ExitDetailMode();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var part = hit.collider.GetComponent<LinkedBackgroundPart>();
                if (part != null)
                {
                    Delete(part.ID);
                    Destroy(part.gameObject);
                }
            }
        }
    }

    public void ExitDetailMode()
    {
        backgroundDetailCameraMove.FinishMove();
        isDetailView = false;
        backgroundEditUIController.SetLinkMode();
        npcSpawner.FinishSpawner();
        currentPart.ChangeViewType(LinkedBackgroundPart.EViewType.LinkableView);
        currentPart.detailViewCamera.Priority = 0;
        linkViewCamera.Priority = 20;
    }

    private void Link()
    {
        if (!isLinking)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;

            currentLinkable = hit.collider.GetComponent<ILinkable>();
            if (currentLinkable == null) return;

            isLinking = true;
            print($"start linking {hit.collider.name}");
        }

        else
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                var linkable = hit.collider.GetComponent<ILinkable>();
                if (linkable != null)
                {
                    print($"end linking {hit.collider.name}");
                    var from = ((LinkedBackgroundPart)currentLinkable).ID;
                    var to = ((LinkedBackgroundPart)linkable).ID;
                    StartCoroutine(
                        EditViewModel.LinkBackgroundPart(
                            from,
                            to,
                            result =>
                            {
                                if (result.IsSuccess)
                                {
                                    var currentObject = ((LinkedBackgroundPart)currentLinkable)?.gameObject;
                                    if (currentObject != null)
                                    {
                                        CreateLine(currentObject, hit.collider.gameObject);
                                    }

                                    // Link((LinkedBackgroundPart)currentLinkable, (LinkedBackgroundPart)linkable);
                                }
                                else
                                {
                                    Debug.LogError(result.error);
                                }

                                currentLinkable = null;
                                isLinking = false;
                            }
                        )
                    );
                }
            }
        }
    }

    private void ShowBackgroundPartDetailView(LinkedBackgroundPart part)
    {
        backgroundEditUIController.SetDetailNpcMode();
        isDetailView = true;
        currentPart = part;
        part.ChangeViewType(LinkedBackgroundPart.EViewType.DetailView);
        npcSpawner.StartSpawner(part.spawnOffset);
        part.detailViewCamera.Priority = 10;
        backgroundDetailCameraMove.StartMove(part.detailViewCamera);
        linkViewCamera.Priority = 0;
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

    public void Create(BackgroundPartInfo backgroundPartInfo)
    {
        var go = Resources.Load<GameObject>("BackgroundPart/LinkableBackgroundPart");
        go = Instantiate(go, new Vector3(0, yAlignment, 0), Quaternion.identity);
        var part = go.GetComponent<LinkedBackgroundPart>();
        part.Init(backgroundPartInfo);

        backgroundParts.Add(part);
    }

    public void Delete(int id)
    {
        var backgroundPart = backgroundParts.Find(x => x.ID == id);

        // foreach (var part in backgroundPart.linkedParts)
        // {
        //     part.linkedParts.Remove(backgroundPart);
        // }

        foreach (var line in lines.FindAll(x => x.start == backgroundPart || x.end == backgroundPart))
        {
            line.DeleteLine();
        }

        backgroundParts.Remove(backgroundPart);
    }

    public void Link(LinkedBackgroundPart current, LinkedBackgroundPart next)
    {
        return;
        // if (current.linkedParts.Contains(next)) return;
        // current.linkedParts.Add(next);
        // next.linkedParts.Add(current);
    }

    public void Unlink(LinkedBackgroundPart current, LinkedBackgroundPart next)
    {
        // if (!current.linkedParts.Contains(next)) return;
        // current.linkedParts.Remove(next);
        // next.linkedParts.Remove(current);
    }

    #endregion

    #region private methods

    private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EditViewModel.BackgroundParts))
        {
            foreach (var part in EditViewModel.BackgroundParts)
            {
                if (!backgroundParts.Exists(x => x.ID == part.ID)) Create(part);
            }
        }
    }

    private void DeleteLine(LinkedLine line)
    {
        lines.Remove(line);
        line.DeleteLine();
    }

    #endregion
}