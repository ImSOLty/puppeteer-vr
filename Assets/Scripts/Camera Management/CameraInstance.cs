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
    private CameraData _cameraData;

    private void Start()
    {
        GetComponents();
        _cameraData = new CameraData(1920, 1080);
        UpdateCameraView();
    }

    private void GetComponents()
    {
        _screenRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        _screenTransform = _screenRenderer.transform;
        _camera = gameObject.GetComponentInChildren<Camera>();
    }

    void UpdateCameraView()
    {
        var material = new Material(_screenRenderer.material);

        if (_camera.targetTexture)
        {
            _camera.targetTexture.Release();
            Destroy(_camera.targetTexture);
        }

        _camera.targetTexture = new RenderTexture(_cameraData.Width, _cameraData.Height, CameraConstants.Depth,
            CameraConstants.Format);
        material.mainTexture = _camera.targetTexture;

        Color prevColor = material.color;

        Color newColor = new Color(prevColor.r, prevColor.g, prevColor.b, _cameraData.Selected ? 1.0f : 0.5f);
        material.color = newColor;
        _screenRenderer.material = material;
    }

    public void UpdateResolution(int width, int height)
    {
        _cameraData.Width = width;
        _cameraData.Height = height;
        UpdateCameraView();
    }

    public void UpdateSelection(bool selected)
    {
        _cameraData.Selected = selected;
        UpdateCameraView();
    }
}