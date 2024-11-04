using Cinemachine;
using UnityEngine;

public class BackgroundDetailCameraMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float maxMoveDistance = 1.0f;
    [SerializeField] private float moveLerpSpeed = 25.0f;
    [SerializeField] private float zoomSpeed = 1.0f;
    [SerializeField] private float zoomLerpSpeed = 25.0f;
    [SerializeField] private float minZoomOrthographicSize = 4.0f;
    [SerializeField] private float maxZoomOrthographicSize = 10.0f;

    private CinemachineVirtualCamera vCamera;
    private Vector3 originalPos;


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(originalPos, maxMoveDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (IsBlocked()) return;

        var moveVel = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) *
                      (moveSpeed * Time.deltaTime);
        Move(moveVel);

        var zoomVelocity = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        Zoom(zoomVelocity);
    }

    private bool IsBlocked()
    {
        if (vCamera == null) return true;
        var distance = Vector3.Distance(vCamera.transform.position, originalPos);
        return distance > maxMoveDistance;
    }

    private void Move(Vector3 moveVel)
    {
        var movePos = vCamera.transform.position + moveVel * (moveSpeed * Time.deltaTime);
        vCamera.transform.position = Vector3.Lerp(vCamera.transform.position, movePos, Time.deltaTime * moveLerpSpeed);
    }

    /// <summary>
    /// zoom in/out
    /// </summary>
    /// <param name="zoomVelocity">minus인 경우, zoom in\n plus인 경우, zoom out</param>
    private void Zoom(float zoomVelocity)
    {
        var newOrthographicSize = vCamera.m_Lens.OrthographicSize + zoomVelocity * (zoomSpeed * Time.deltaTime);
        if (newOrthographicSize < minZoomOrthographicSize)
            newOrthographicSize = minZoomOrthographicSize;
        if (newOrthographicSize > maxZoomOrthographicSize)
            newOrthographicSize = maxZoomOrthographicSize;

        vCamera.m_Lens.OrthographicSize = Mathf.Lerp(
            vCamera.m_Lens.OrthographicSize,
            newOrthographicSize,
            Time.deltaTime * zoomLerpSpeed
        );
    }

    public void StartMove(CinemachineVirtualCamera virtualCamera)
    {
        vCamera = virtualCamera;
        originalPos = vCamera.transform.position;
    }

    public void FinishMove()
    {
        vCamera.transform.position = originalPos;
        vCamera = null;
    }
}