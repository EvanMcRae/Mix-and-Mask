using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour
{
    public static bool Paused = false, PausedThisFrame, GoingToMainMenu = false;
    [SerializeField] private PopupPanel pausePanel, settingsPanel;

    void Start()
    {
        settingsPanel.OnClose += () =>
        {
            GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetUpdate(true);
        };
    }

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
        settingsPanel.Up();
        Debug.Log(GetComponent<CanvasGroup>());
        GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetUpdate(true);
    }
}
