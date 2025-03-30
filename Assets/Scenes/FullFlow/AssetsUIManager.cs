using System;
using System.IO;
using SimpleFileBrowser;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AssetsUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform assetsUIList;
    [SerializeField] private GameObject assetUIElementPrefab;
    [SerializeField] private GameObject preview, createNewAssetWindow;
    [SerializeField] private Text previewNameText, previewReferenceText, previewCreatedAtText;
    [SerializeField] private RawImage previewImage;
    [SerializeField] private AssetsManager assetsManager;
    [SerializeField] private FileSelectionManager fileSelectionManager;
    [SerializeField] private InputField assetNameInputField;
    [SerializeField] private Texture2D defaultTexture;
    private AssetProperties selectedAsset;
    private AssetType currentAssetType = AssetType.LOCATION;
    private string selectedAssetPath = "";
    private string selectedPreviewPath = "";

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
        previewNameText.text = selectedAsset.name;
        previewReferenceText.text = selectedAsset.fileReference;
        previewCreatedAtText.text = selectedAsset.createdAt;

        previewReferenceText.color = !selectedAsset.Exists() ? Settings.Colors.missingColor : Settings.Colors.defaultBlack;
        previewImage.texture = selectedAsset.PreviewExists() ? selectedAsset.GetPreviewTexture() : defaultTexture;
    }

    public void DeleteSelectedAsset()
    {
        preview.SetActive(false);
        assetsManager.DeleteAnAsset(currentAssetType, selectedAsset);
        UpdateElementList();
    }
    public void AddNewAssetButton()
    {
        Settings.Hints.currentHintAbout = HintAbout.ADD_ASSET_SETTINGS;
        createNewAssetWindow.SetActive(true);
    }

    public void SelectAssetPath()
    {
        if (currentAssetType == AssetType.CHARACTER)
        {
            fileSelectionManager.SetFilters(new FileBrowser.Filter("Assets", ".vrm"));
        }
        else
        {
            fileSelectionManager.SetFilters(new FileBrowser.Filter("Assets", ".glb", ".gltf"));
        }

        FileBrowser.ShowLoadDialog((paths) =>
        {
            selectedAssetPath = paths[0];
        }, () => { }, FileBrowser.PickMode.Files, false, null, null, "Select File", "Select");

        fileSelectionManager.SetupCanvasAfterInit();
    }

    public void SelectPreviewPath()
    {
        fileSelectionManager.SetFilters(new FileBrowser.Filter("Previews", ".jpg", ".jpeg", ".png"));

        FileBrowser.ShowLoadDialog((paths) =>
        {
            selectedPreviewPath = paths[0];
        }, () => { }, FileBrowser.PickMode.Files, false, null, null, "Select File", "Select");

        fileSelectionManager.SetupCanvasAfterInit();
    }

    public void SaveNewAsset()
    {
        string name = assetNameInputField.text;
        if (name == "" || selectedAssetPath == "")
        {
            return;
        }
        assetsManager.CreateNewAsset(
            assetType: currentAssetType,
            assetProperties: new AssetProperties(name: name, fileReference: selectedAssetPath, previewPath: selectedPreviewPath)
        );
        UpdateElementList();
        CloseCreateNewAssetWindow();
    }
    public void CloseCreateNewAssetWindow()
    {
        assetNameInputField.text = "";
        createNewAssetWindow.SetActive(false);
    }

    private void UpdateElementList()
    {
        ClearElementList();

        int num = 0;
        float elementSize = 0;
        float gapBetweenElements = 10;

        foreach (AssetProperties assetProperties in AssetsManager.GetAssetsPropertiesByAssetType(currentAssetType))
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
            buttonName.color = Settings.Colors.missingColor;
        }
        else
        {
            buttonName.text = assetProperties.name;
            buttonName.color = Settings.Colors.defaultBlack;
        }

        listElement.GetComponent<Button>().onClick.AddListener(() => SelectAsset(assetProperties));
        return listElement;
    }
}
