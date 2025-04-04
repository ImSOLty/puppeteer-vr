using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AppScenesRegistryUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform scenesUIList;
    [SerializeField] GameObject sceneUIElementPrefab;
    [SerializeField] private GameObject preview;
    [SerializeField] private Text previewNameText, updatedAtText;
    [SerializeField] private AppScenesManager scenesManager;
    [SerializeField] private GameObject scenesCreationWindow, sceneRegistryWindow, scenesWindow, animationWindow;

    SceneProperties selectedScene = null;

    private void Awake()
    {
        UpdateElementList();
    }
    public void AddNewScene()
    {
        scenesCreationWindow.SetActive(true);
        sceneRegistryWindow.SetActive(false);
        Settings.Hints.currentHintAbout = HintAbout.ADD_SCENE_SETTINGS;
    }

    public void SelectScene(SceneProperties sceneProperties)
    {
        selectedScene = sceneProperties;
        Settings.Animation.ScenePropertiesData = sceneProperties;
        preview.SetActive(true);
        previewNameText.text = selectedScene.name;
        updatedAtText.text = selectedScene.updatedAt;
    }

    public void DeleteSelectedScene()
    {
        AppScenesManager.DeleteScene(selectedScene);
        preview.SetActive(false);
        UpdateElementList();
    }
    public void EditSelectedScene()
    {
        Settings.Animation.AnimationMode = Mode.PROPS_MANAGEMENT;

        AppSceneCreationManager appSceneCreationManager = FindObjectOfType<AppSceneCreationManager>();
        appSceneCreationManager.InitialSetupWithExistingSceneProperties(selectedScene);
        appSceneCreationManager.StartSceneCreation();
    }
    public void AnimateSelectedScene()
    {
        // Check if there no cameras
        if (selectedScene.cameraPropDatas.Count == 0)
        {
            return;//For now
        }
        scenesWindow.SetActive(false);
        animationWindow.SetActive(true);
        Settings.Hints.currentHintAbout = HintAbout.ANIMATION_OPTIONS;
    }

    private void UpdateElementList()
    {
        ClearElementList();

        int num = 0;
        float elementSize = 0;
        float gapBetweenElements = 10;

        foreach (SceneProperties sceneProperties in AppScenesManager.GetScenesPropertiesList())
        {
            GameObject listElement = AddUIListElementBySceneProperties(sceneProperties);

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
        scenesUIList.sizeDelta = new Vector2(scenesUIList.sizeDelta.x, gapBetweenElements + num * (elementSize + gapBetweenElements));
    }
    private GameObject AddUIListElementBySceneProperties(SceneProperties sceneProperties)
    {
        GameObject listElement = Instantiate(sceneUIElementPrefab, scenesUIList.transform);
        Text buttonName = listElement.GetComponentInChildren<Text>();

        buttonName.text = sceneProperties.name;

        listElement.GetComponent<Button>().onClick.AddListener(() => SelectScene(sceneProperties));
        return listElement;
    }

    private void ClearElementList()
    {
        foreach (Transform child in scenesUIList.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
