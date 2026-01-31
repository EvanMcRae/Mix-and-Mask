#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;
    private const string GAME_SCENE = "MASTER_GameScene";
    [SerializeField] private GameObject firstSelection;
    private GameObject previousSelection;

    [SerializeField] private PopupPanel SettingsPanel, ControlsPanel, CreditsPanel;
    [SerializeField] private Button PlayButton, QuitButton, CreditsButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelection);
        instance = this;

        // Disable quitting in web player
        if (Utils.IsWebPlayer())
        {
            QuitButton.gameObject.SetActive(false);
            Utils.SetNavigation(PlayButton, CreditsButton, Utils.Direction.UP);
            Utils.SetNavigation(CreditsButton, PlayButton, Utils.Direction.DOWN);
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

        // Disable quitting in web player
        if (Utils.IsWebPlayer())
        {
            return;
        }

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
