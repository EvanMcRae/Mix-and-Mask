#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    private const string GAME_SCENE = "MASTER_GameScene";
    [SerializeField] private GameObject firstSelection;
    private GameObject previousSelection;

    [SerializeField] private PopupPanel SettingsPanel, ControlsPanel, CreditsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelection);
        instance = this;

        // Disable fullscreen in demo / web player
        if (Application.platform == RuntimePlatform.WebGLPlayer
#if UNITY_EDITOR
            || EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL
#endif
        )
        {
            
        }
    }

    public void PressPlay()
    {
        if (ScreenTransition.inProgress) return;
        ScreenTransition.Out(() =>
        {
            SceneManager.LoadScene(GAME_SCENE);
        });
    }

    public void PressQuit()
    {
        if (ScreenTransition.inProgress) return;
        ScreenTransition.Out(() =>
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
        });
    }

    public void PressCredits()
    {
        if (ScreenTransition.inProgress) return;
        previousSelection = EventSystem.current.currentSelectedGameObject;
        CreditsPanel.Up();
    }

    public void PressControls()
    {
        if (ScreenTransition.inProgress) return;
        previousSelection = EventSystem.current.currentSelectedGameObject;
        ControlsPanel.Up();
    }

    public void PressSettings()
    {
        if (ScreenTransition.inProgress) return;
        previousSelection = EventSystem.current.currentSelectedGameObject;
        SettingsPanel.Up();
    }

    public void ClosePopup()
    {
        EventSystem.current.SetSelectedGameObject(previousSelection);
    }
}
