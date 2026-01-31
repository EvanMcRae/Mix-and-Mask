using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private const string GAME_SCENE = "MASTER_GameScene";
    private GameObject previousSelection;

    [SerializeField] private GameObject SettingsPanel, CreditsPanel, InstructionsPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
    }

    public void PressInstructions()
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
    }

    public void PressSettings()
    {
        previousSelection = EventSystem.current.currentSelectedGameObject;
    }

    // TODO
    public void ClosePopup()
    {
        EventSystem.current.SetSelectedGameObject(previousSelection);
    }
}
