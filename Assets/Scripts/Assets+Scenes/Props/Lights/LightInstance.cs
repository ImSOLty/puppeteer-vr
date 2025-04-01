using UnityEngine;

public class LightInstance : MonoBehaviour
{
    private LightPropData propData;
    [SerializeField] private Light _light;
    [SerializeField] private Renderer _sphere;

    private void UpdateObjectColors()
    {
        _sphere.material.color = _light.color;
    }

    public void UpdateLight(Color color, float range, float intensity)
    {
        _light.color = color;
        _light.range = range;
        _light.intensity = intensity;
        UpdateObjectColors();
    }

    public void SetPropData(LightPropData propData)
    {
        this.propData = propData;
        transform.position = propData.position;
        transform.eulerAngles = propData.rotation;
        UpdateLight(propData.color, propData.range, propData.intensity);
    }

    public LightPropData AssemblePropData()
    {
        LightPropData result = new LightPropData(
            position: transform.position,
            rotation: transform.eulerAngles,
            color: _light.color,
            range: _light.range,
            intensity: _light.intensity
        );
        if (propData != null) { result.propUuid = propData.propUuid; }
        return result;
    }
}