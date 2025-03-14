using System;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssetsUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform assetsUIList;
    [SerializeField] private GameObject assetUIElementPrefab;
    [SerializeField] private GameObject preview;
    [SerializeField] private Text previewNameText, previewReferenceText;
    [SerializeField] private AssetsManager assetsManager;

    private AssetProperties selectedAsset;
    private AssetType currentAssetType = AssetType.LOCATION;

    void Start()
    {
        UpdateElementList();
    }

    public void SelectAssetType(string assetTypeAsString)// Due to use in buttons
    {
        currentAssetType = Enum.Parse<AssetType>(assetTypeAsString);
        UpdateElementList();
    }
    public void SelectAsset(AssetProperties assetProperties)
    {
        selectedAsset = assetProperties;
        preview.SetActive(true);
        previewNameText.text = assetProperties.name;
        previewReferenceText.text = assetProperties.fileReference;
        if (!assetProperties.Exists())
        {
            previewReferenceText.color = Color.red;
        }
        else
        {
            previewReferenceText.color = Color.black;
        }
    }

    public void DeleteSelectedAsset()
    {
        preview.SetActive(false);
        assetsManager.DeleteAnAsset(currentAssetType, selectedAsset);
        UpdateElementList();
    }

    private void UpdateElementList()
    {
        ClearElementList();

        int num = 0;
        float elementSize = 0;
        float gapBetweenElements = 10;

        foreach (AssetProperties assetProperties in assetsManager.GetAssetsPropertiesByAssetType(currentAssetType))
        {
            GameObject listElement = AddUIListElementByAssetProperties(assetProperties);

            // Positioning on scroll view
            float yPositionOffset = 50;
            RectTransform elementRect = listElement.GetComponent<RectTransform>();

            elementSize = elementRect.sizeDelta.y;

            elementRect.localPosition = new Vector2(
                elementRect.localPosition.x,
                -(yPositionOffset + gapBetweenElements + (gapBetweenElements + elementRect.sizeDelta.y) * num)
            );
            num += 1;
        }
        // Positioning scroll view
        assetsUIList.sizeDelta = new Vector2(assetsUIList.sizeDelta.x, gapBetweenElements + num * (elementSize + gapBetweenElements));
    }

    private void ClearElementList()
    {
        foreach (Transform child in assetsUIList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject AddUIListElementByAssetProperties(AssetProperties assetProperties)
    {
        GameObject listElement = Instantiate(assetUIElementPrefab, assetsUIList.transform);
        Text buttonName = listElement.GetComponentInChildren<Text>();
        if (!assetProperties.Exists())
        {
            buttonName.text = assetProperties.name + " (missing reference)";
            buttonName.color = Color.red;
        }
        else
        {
            buttonName.text = assetProperties.name;
            buttonName.color = Color.black;
        }

        listElement.GetComponent<Button>().onClick.AddListener(() => SelectAsset(assetProperties));
        return listElement;
    }
}
