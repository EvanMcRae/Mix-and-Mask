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
    }

    public void PressPlay()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, Play);
            }
            return;
        }
        Play();
    }

    public void Play()
    {
        ScreenTransition.Out(() =>
        {
            SceneManager.LoadScene(GAME_SCENE);
        });
    }

    public void PressQuit()
    {
        // Disable quitting in web player
        if (Utils.IsWebPlayer())
        {
            return;
        }

        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, () => ScreenTransition.Out(() => Quit()));
            }
            return;
        }

        ScreenTransition.Out(() => Quit());
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void PressCredits()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, () => CreditsPanel.Up());
            }
            return;
        }
        CreditsPanel.Up();
    }

    public void PressControls()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, () => ControlsPanel.Up());
            }
            return;
        }
        ControlsPanel.Up();
    }

    public void PressSettings()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, () => SettingsPanel.Up());
            }
        return;
        }
        SettingsPanel.Up();
    }
}
