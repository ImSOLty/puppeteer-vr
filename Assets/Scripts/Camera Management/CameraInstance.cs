using UnityEngine;

public class CameraConstants
{
    public const int Depth = 16;
    public const RenderTextureFormat Format = RenderTextureFormat.ARGB32;
}

public class CameraInstance : MonoBehaviour
{
    private RenderTexture _renderTexture;
    private Camera _camera;
    private Renderer _screenRenderer;
    [SerializeField] private GameObject screen, sphere;
    private CameraData _cameraData;

    private void Start()
    {
        GetComponents();
        _cameraData = new CameraData(1920, 1080);
        UpdateCameraView();
    }

    private void GetComponents()
    {
        _screenRenderer = screen.GetComponent<MeshRenderer>();
        _camera = gameObject.GetComponentInChildren<Camera>();
    }

    void UpdateCameraView()
    {
        if (_camera.targetTexture)
        {
            _camera.targetTexture.Release();
            Destroy(_camera.targetTexture);
        }

        _camera.targetTexture = new RenderTexture(_cameraData.Width, _cameraData.Height, CameraConstants.Depth,
            CameraConstants.Format);
        _screenRenderer.material.mainTexture = _camera.targetTexture;
    }

    public void UpdateResolution(int width, int height)
    {
        _cameraData.Width = width;
        _cameraData.Height = height;
        UpdateCameraView();
    }

    void SetAsClosed(bool closed)
    {
        screen.SetActive(!closed);
        sphere.SetActive(closed);
    }

    public void UpdateClosure()
    {
        _cameraData.Closed = !_cameraData.Closed;
        SetAsClosed(_cameraData.Closed);
    }
}