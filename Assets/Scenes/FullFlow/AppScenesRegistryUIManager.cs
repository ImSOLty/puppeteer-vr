using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AppScenesRegistryUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform scenesUIList;
    [SerializeField] GameObject sceneUIElementPrefab;
    [SerializeField] private GameObject preview;
    [SerializeField] private Text previewNameText;
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
    }

    public void SelectScene(SceneProperties sceneProperties)
    {
        selectedScene = sceneProperties;
        Settings.Animation.ScenePropertiesData = sceneProperties;
        preview.SetActive(true);
        previewNameText.text = selectedScene.name;
    }

    public void DeleteSelectedScene()
    {
        AppScenesManager.DeleteScene(selectedScene);
        preview.SetActive(false);
        UpdateElementList();
    }
    public void AnimateSelectedScene()
    {
        scenesWindow.SetActive(false);
        animationWindow.SetActive(true);
    }

    private void UpdateElementList()
    {
        ClearElementList();

        int num = 0;
        float elementSize = 0;
        float gapBetweenElements = 10;

        foreach (SceneProperties sceneProperties in AppScenesManager.GetScenesProperties())
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
