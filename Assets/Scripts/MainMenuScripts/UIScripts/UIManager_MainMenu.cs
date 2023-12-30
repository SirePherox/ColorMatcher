using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_MainMenu : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private Button loadGame_btn;

    [Header("Help Panel References")]
    [SerializeField] private Button showHelpPanel_btn;
    [SerializeField] private Button closeHelpPanel_btn;

    [Header("Settings Panel References")]
    [SerializeField] private Button showSettingsPanel_btn;
    [SerializeField] private Button closeSettingsPanel_btn;
    [SerializeField] private Button resetProgress_btn;

    [Header("References")]
    [SerializeField] private HelpPanelController helpPanel;
    [SerializeField] private GameObject LoadingGame_object;

    // Start is called before the first frame update
    void Start()
    {
        AddButtonListeners();
        LoadingGame_object.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadGameScene()
    {
        //show loading screen, scene wont load immediately
        LoadingGame_object.SetActive(true);
        //call load scene async
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(SceneIndex.GAME_SCENE)); 
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }

    private void AddButtonListeners()
    {
        showHelpPanel_btn.onClick.AddListener(helpPanel.OpenPanel);
        closeHelpPanel_btn.onClick.AddListener(helpPanel.ClosePanel);
        loadGame_btn.onClick.AddListener(LoadGameScene);
    }


}
