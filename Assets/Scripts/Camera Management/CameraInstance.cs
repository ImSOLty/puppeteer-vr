using UnityEngine;

public class CameraInstance : MonoBehaviour
{
    private int _id;
    private RenderTexture _renderTexture;
    private Camera _camera;
    private Renderer _screenRenderer;
    [SerializeField] private Transform screen, sphere;
    private CameraData _cameraData;

    private void Start()
    {
        _id = gameObject.GetInstanceID();
        GetComponents();
        UpdateResolution(CameraConstants.DefaultWidth, CameraConstants.DefaultHeight);
    }

    private void GetComponents()
    {
        _screenRenderer = screen.GetComponent<MeshRenderer>();
        _camera = gameObject.GetComponentInChildren<Camera>();
    }

    void UpdateCameraView()
    {
        // Release and destroy previous target texture
        if (_camera.targetTexture)
        {
            _camera.targetTexture.Release();
            Destroy(_camera.targetTexture);
        }

        // Set new target texture
        _camera.targetTexture = new RenderTexture(_cameraData.Width, _cameraData.Height,
            CameraConstants.TextureDepth, CameraConstants.TextureFormat);
        _screenRenderer.material.mainTexture = _camera.targetTexture;

        // Resize Screen-panel
        Vector3 newScale;
        if (_cameraData.Width > _cameraData.Height)
            newScale = new Vector3(1f, 1f, (float)_cameraData.Height / _cameraData.Width);
        else
            newScale = new Vector3((float)_cameraData.Width / _cameraData.Height, 1f, 1f);

        screen.localScale = newScale / CameraConstants.ScreenResize;
    }

    public void UpdateResolution(int width, int height)
    {
        _cameraData ??= new CameraData(_camera.name);

        _cameraData.Width = width;
        _cameraData.Height = height;
        UpdateCameraView();
    }

    void SetAsClosed(bool closed)
    {
        screen.gameObject.SetActive(!closed);
        sphere.gameObject.SetActive(closed);
    }

    public void UpdateClosure()
    {
        _cameraData.Closed = !_cameraData.Closed;
        SetAsClosed(_cameraData.Closed);
    }

    void UpdateNear(float near)
    {
        _cameraData.Near = near;
        UpdateCameraProperties();
    }

    void UpdateFar(float far)
    {
        _cameraData.Far = far;
        UpdateCameraProperties();
    }

    void UpdateFOV(float fov)
    {
        _cameraData.FOV = fov;
        UpdateCameraProperties();
    }

    void UpdateName(string newName)
    {
        _cameraData.Name = newName;
    }

    void UpdateCameraProperties()
    {
        _camera.fieldOfView = _cameraData.FOV;
        _camera.nearClipPlane = _cameraData.Near;
        _camera.farClipPlane = _cameraData.Far;
    }

    public override string ToString()
    {
        return _cameraData.Name;
    }
}