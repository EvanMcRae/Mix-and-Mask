using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string GAME_SCENE = "MASTER_GameScene";
    [SerializeField] private GameObject firstSelection;
    private GameObject previousSelection;

    [SerializeField] private PopupPanel SettingsPanel, ControlsPanel, CreditsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelection);
    }

    public void PressPlay()
    {
        ScreenTransition.Out(() =>
        {
            SceneManager.LoadScene(GAME_SCENE);
        });
    }

    public void PressQuit()
    {
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
        previousSelection = EventSystem.current.currentSelectedGameObject;
        CreditsPanel.Up();
    }

    public void PressControls()
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
        ControlsPanel.Up();
    }

    public void PressSettings()
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
        SettingsPanel.Up();
    }

    // TODO
    public void ClosePopup()
    {
        EventSystem.current.SetSelectedGameObject(previousSelection);
    }
}
