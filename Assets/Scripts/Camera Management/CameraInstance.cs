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
    private Transform _screenTransform;

    private void Start()
    {
        GetComponents();
        SetupTexture();
    }

    private void GetComponents()
    {
        _screenRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        _screenTransform = _screenRenderer.transform;
        _camera = gameObject.GetComponentInChildren<Camera>();
    }

    void SetupTexture()
    {
        SetResolution(1920, 1080);
    }

    private void SetResolution(int width, int height)
    {
        if (_camera.targetTexture)
        {
            _camera.targetTexture.Release();
        }

        _camera.targetTexture = new RenderTexture(width, height, CameraConstants.Depth, CameraConstants.Format);
        _screenRenderer.material.mainTexture = _camera.targetTexture;
    }
}