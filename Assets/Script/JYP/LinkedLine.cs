using UnityEngine;

public class LinkedLine : MonoBehaviour
{
    public LinkedBackgroundPart start;
    public LinkedBackgroundPart end;

    private GameObject startObj;
    private GameObject endObj;
    private Vector3 startPortalPos;
    private Vector3 endPortalPos;
    private LineRenderer line;

    public void Init(LinkedBackgroundPart startPart, LinkedBackgroundPart endPart)
    {
        this.start = startPart;
        this.end = endPart;
        startObj = start.gameObject;
        endObj = end.gameObject;
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 2f;
        line.endWidth = 2f;
        
    }

    private void Update()
    {
        line.SetPosition(0, startObj.transform.position);
        line.SetPosition(1, endObj.transform.position);
    }

    public void DeleteLine()
    {
        start.UnLink(end);
        end.UnLink(start);
        Destroy(gameObject);
    }
}