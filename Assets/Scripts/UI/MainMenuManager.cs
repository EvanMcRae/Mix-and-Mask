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

    [SerializeField] private PopupPanel SettingsPanel, ControlsPanel, CreditsPanel;
    [SerializeField] private Button QuitButton;
    [SerializeField] private GameObject wwiseGlobal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelection);
        instance = this;

        // Disable quitting in web player
        if (Utils.IsWebPlayer())
        {
            QuitButton.gameObject.SetActive(false);

            // Remap navigation
            Utils.SetNavigation(QuitButton.navigation.selectOnDown, QuitButton.navigation.selectOnUp, Utils.Direction.UP);
            Utils.SetNavigation(QuitButton.navigation.selectOnUp, QuitButton.navigation.selectOnDown, Utils.Direction.DOWN);
        }

        if (Utils.WwiseGlobal == null)
        {
            Utils.WwiseGlobal = wwiseGlobal;
        }
        AkUnitySoundEngine.PostEvent("PlayTitle", Utils.WwiseGlobal);
    }

    void Update()
    {
        if (Utils.WwiseGlobal == null)
        {
            Utils.WwiseGlobal = wwiseGlobal;
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

        AkUnitySoundEngine.PostEvent("StopMusic", Utils.WwiseGlobal);

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
        CreditsPanel.Up();
    }

    public void PressControls()
    {
        if (ScreenTransition.inProgress) return;
        ControlsPanel.Up();
    }

    public void PressSettings()
    {
        if (ScreenTransition.inProgress) return;
        SettingsPanel.Up();
    }
}
