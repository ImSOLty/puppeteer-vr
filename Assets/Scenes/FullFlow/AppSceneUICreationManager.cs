using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AppSceneUICreationManager : MonoBehaviour
{
    [SerializeField] GameObject assetSelectionWindow, scenesRegistryWindow, creationWindow;
    [SerializeField] RectTransform assetSelectionUIList;
    [SerializeField] GameObject selectableElementPrefab;
    [SerializeField] Button doneButton, selectLocation, selectCharacters, manageButton;
    [SerializeField] InputField sceneNameInputField;

    private AppSceneCreationManager sceneCreationManager;
    private HashSet<AssetProperties> selectedCharactersAssetProperties;
    private AssetProperties selectedLocationAssetProperties;
    private Button previousSelectedButton;

    private bool charactersSelected, locationSelected;

    public void Awake()
    {
        sceneCreationManager = FindObjectOfType<AppSceneCreationManager>();
        ResetUI();
        UpdateManageButtonsWindow();
    }
    private void ResetUI()
    {
        selectedCharactersAssetProperties = new();
        selectedLocationAssetProperties = null;
        previousSelectedButton = null;
        charactersSelected = false;
        locationSelected = false;
    }

    public void SelectLocationButton()
    {
        assetSelectionWindow.SetActive(true);
        creationWindow.SetActive(false);
        UpdateAssetSelectionWindow(AssetType.LOCATION);
    }

    public void SelectCharactersButton()
    {
        assetSelectionWindow.SetActive(true);
        creationWindow.SetActive(false);
        UpdateAssetSelectionWindow(AssetType.CHARACTER);
    }

    public void DoneOrCloseSelectionButton()
    {
        assetSelectionWindow.SetActive(false);
        creationWindow.SetActive(true);
        UpdateManageButtonsWindow();
    }
    public void CloseCreationButton()
    {
        scenesRegistryWindow.SetActive(true);
        creationWindow.SetActive(false);
    }
    private void UpdateManageButtonsWindow()
    {
        selectLocation.targetGraphic.color = locationSelected ? Settings.Colors.selectionColorful : Settings.Colors.defaultColor;
        selectCharacters.targetGraphic.color = charactersSelected ? Settings.Colors.selectionColorful : Settings.Colors.defaultColor;
        manageButton.interactable = locationSelected && charactersSelected;
    }

    private void UpdateAssetSelectionWindow(AssetType assetType)
    {
        ClearElementList();

        int num = 0;
        float elementSize = 0;
        float gapBetweenElements = 10;

        foreach (AssetProperties assetProperties in AssetsManager.GetAssetsPropertiesByAssetType(assetType))
        {
            if (!assetProperties.Exists())
            {
                continue;
            }
            GameObject listElement = AddUIListElementByAssetProperties(assetProperties, assetType);

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
        assetSelectionUIList.sizeDelta = new Vector2(assetSelectionUIList.sizeDelta.x, gapBetweenElements + num * (elementSize + gapBetweenElements));
    }

    private GameObject AddUIListElementByAssetProperties(AssetProperties assetProperties, AssetType assetType)
    {
        GameObject listElement = Instantiate(selectableElementPrefab, assetSelectionUIList.transform);

        Button button = listElement.GetComponent<Button>();
        Text buttonName = listElement.GetComponentInChildren<Text>();

        buttonName.text = assetProperties.name;

        button.onClick.AddListener(
            () =>
            {
                if (assetType == AssetType.LOCATION)
                {
                    SwitchSelectionLocation(assetProperties);
                    if (previousSelectedButton)
                        previousSelectedButton.targetGraphic.color = Settings.Colors.defaultColor;
                    if (selectedLocationAssetProperties == assetProperties)
                        button.targetGraphic.color = Settings.Colors.selectionColorful;
                    else
                        button.targetGraphic.color = Settings.Colors.defaultColor;
                    previousSelectedButton = button;
                }
                else
                {
                    SwitchSelectionCharacter(assetProperties);
                    if (selectedCharactersAssetProperties.Contains(assetProperties))
                        button.targetGraphic.color = Settings.Colors.selectionColorful;
                    else
                        button.targetGraphic.color = Settings.Colors.defaultColor;
                }
            }
        );

        if (assetType == AssetType.LOCATION && selectedLocationAssetProperties == assetProperties)
            button.targetGraphic.color = Settings.Colors.selectionColorful;
        if (assetType == AssetType.CHARACTER && selectedCharactersAssetProperties.Contains(assetProperties))
            button.targetGraphic.color = Settings.Colors.selectionColorful;

        return listElement;
    }

    private void SwitchSelectionCharacter(AssetProperties asset)
    {
        if (selectedCharactersAssetProperties.Contains(asset))
        {
            selectedCharactersAssetProperties.Remove(asset); // If was selected - deselected
        }
        else
        {
            selectedCharactersAssetProperties.Add(asset); // was not selected -> selected
        }

        charactersSelected = selectedCharactersAssetProperties.Count > 0;
        doneButton.interactable = charactersSelected;
    }

    private void SwitchSelectionLocation(AssetProperties asset)
    {
        if (selectedLocationAssetProperties == asset)
        {
            selectedLocationAssetProperties = null; // This location was selected -> deselected
        }
        else
        {
            selectedLocationAssetProperties = asset; // No location or another one was selected -> selected this one
        }

        locationSelected = selectedLocationAssetProperties != null;
        doneButton.interactable = locationSelected;
    }

    private void ClearElementList()
    {
        foreach (Transform child in assetSelectionUIList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void Manage()
    {
        if (sceneNameInputField.text == "")
        {
            return;
        }

        Settings.Animation.AnimationMode = Mode.PROPS_MANAGEMENT;
        Settings.Animation.ScenePropertiesData = null;
        sceneCreationManager.InitialSetupWithNameLocationAndCharacters(
            name: sceneNameInputField.text,
            location: selectedLocationAssetProperties,
            characters: selectedCharactersAssetProperties.ToList()
        );
        sceneCreationManager.StartSceneCreation();
    }
}

