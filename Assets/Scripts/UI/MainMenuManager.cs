#if UNITY_EDITOR
using DG.Tweening;
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
    [SerializeField] private CanvasGroup buttonGroup;
    private Tween fadeTween;

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
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, Play);
            }
            return;
        }
        AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
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
                AkUnitySoundEngine.PostEvent("Back", Utils.WwiseGlobal);
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, () => ScreenTransition.Out(() => Quit()));
            }
            return;
        }
        AkUnitySoundEngine.PostEvent("Back", Utils.WwiseGlobal);
        Quit();
    }

    public void ActuallyQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void Quit()
    {
        AkUnitySoundEngine.PostEvent("StopMusic", Utils.WwiseGlobal);
        ScreenTransition.Out(() => Quit());
    }

    public void PressCredits()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, Credits);
            }
            return;
        }
        AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
        Credits();
    }

    public void Credits()
    {
        FadeOutMenu();
        CreditsPanel.Up();
        CreditsPanel.OnClose = FadeInMenu;
    }

    public void PressControls()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, Controls);
            }
            return;
        }
        AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
        Controls();
    }

    public void Controls()
    {
        FadeOutMenu();
        ControlsPanel.Up();
        ControlsPanel.OnClose = FadeInMenu;
    }

    public void PressSettings()
    {
        if (ScreenTransition.inProgress)
        {
            if (ScreenTransition.goingIn)
            {
                AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
                Utils.SetExclusiveAction(ref ScreenTransition.instance.InAction, Settings);
            }
            return;
        }
        AkUnitySoundEngine.PostEvent("Select", Utils.WwiseGlobal);
        Settings();
    }

    public void Settings()
    {
        FadeOutMenu();
        SettingsPanel.Up();
        SettingsPanel.OnClose = FadeInMenu;
    }

    void FadeOutMenu()
    {
        if (fadeTween != null)
            Utils.KillTween(ref fadeTween);
        fadeTween = buttonGroup.DOFade(0, 0.5f).SetUpdate(true);
    }

    void FadeInMenu()
    {
        if (fadeTween != null)
            Utils.KillTween(ref fadeTween);
        fadeTween = buttonGroup.DOFade(1, 0.5f).SetUpdate(true);
    }
}
