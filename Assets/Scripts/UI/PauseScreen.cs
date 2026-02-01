using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public static bool Paused = false, PausedThisFrame, GoingToMainMenu = false;
    [SerializeField] private PopupPanel pausePanel, settingsPanel;
    [SerializeField] private GameObject wwiseGlobal;

    void Start()
    {
        if (Utils.WwiseGlobal == null)
        {
            Utils.WwiseGlobal = wwiseGlobal;
        }
        AkUnitySoundEngine.PostEvent("StartGame", Utils.WwiseGlobal);
    }

    void Update()
    {
        if (Utils.WwiseGlobal == null)
        {
            Utils.WwiseGlobal = wwiseGlobal;
        }
    }

    void LateUpdate()
    {
        PausedThisFrame = false;
        if (!ScreenTransition.inProgress && !WaveManager.GameOver && Keyboard.current[Utils.IsWebPlayer() ? Key.Tab : Key.Escape].wasPressedThisFrame)
        {
            Paused = !Paused;
            if (Paused)
            {
                Pause();
            }
            else if (!PopupPanel.overlayUp)
            {
                Resume();
            }
        }
    }

    void Pause()
    {
        AkUnitySoundEngine.PostEvent("PauseAll", Utils.WwiseGlobal);
        PausedThisFrame = true;
        pausePanel.Up();
        Time.timeScale = 0;
    }

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pausePanel.Down(() => {
            Time.timeScale = 1;
            AkUnitySoundEngine.PostEvent("ResumeAll", Utils.WwiseGlobal);
        });
    }

    public void TitleScreen()
    {
        GoingToMainMenu = true;
        ScreenTransition.Out(() =>
        {
            Time.timeScale = 1;
            GoingToMainMenu = false;
            SceneManager.LoadScene("MASTER_MainMenu");
        });
    }

    public void Settings()
    {
        if (settingsPanel == null) return; // TODO remove this is just to prevent errors rn
        settingsPanel.Up();
    }
}
