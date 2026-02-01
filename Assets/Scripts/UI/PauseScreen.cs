using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public static bool Paused = false, PausedThisFrame;
    [SerializeField] private PopupPanel pausePanel, settingsPanel;

    // Update is called once per frame
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
        PausedThisFrame = true;
        pausePanel.Up();
        Time.timeScale = 0;
    }

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        pausePanel.Down(() => Time.timeScale = 1);
    }

    public void TitleScreen()
    {
        ScreenTransition.Out(() => SceneManager.LoadScene("MASTER_MainMenu"));
    }

    public void Settings()
    {
        if (settingsPanel == null) return; // TODO remove this is just to prevent errors rn
        settingsPanel.Up();
    }
}
