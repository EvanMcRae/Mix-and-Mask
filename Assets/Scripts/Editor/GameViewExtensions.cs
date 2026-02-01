using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

// From https://discussions.unity.com/t/unity-2021-mouse-cursor-no-longer-locks-hides-when-entering-play-mode-regression/886114/8
// Force focuses the game window and also toggles the pixelation shader

[InitializeOnLoad]
public static class GameViewExtensions
{
    static GameViewExtensions()
    {
        EditorApplication.playModeStateChanged += PlayModeChanged;
        EditorApplication.pauseStateChanged += PauseStateChanged;
        AssemblyReloadEvents.beforeAssemblyReload += GameViewExtensionsDestructor;

        if (!EditorApplication.isPlaying)
        {
            var update = new string[1] { "Assets/ShaderPass.asset" };
            AssetDatabase.ForceReserializeAssets(update);
            AssemblyReloadEvents.afterAssemblyReload += SetShaderOnLoad;
        }
    }

    private static void SetShaderOnLoad()
    {
        AssemblyReloadEvents.afterAssemblyReload -= SetShaderOnLoad;
        ShaderPassHolder.DisablePass();
    }

    private static void GameViewExtensionsDestructor()
    {
        EditorApplication.playModeStateChanged -= PlayModeChanged;
        EditorApplication.pauseStateChanged -= PauseStateChanged;
        AssemblyReloadEvents.beforeAssemblyReload -= GameViewExtensionsDestructor;
    }

    private static void PlayModeChanged(PlayModeStateChange playMode)
    {
        // We only execute anything when we just entered play mode.
        if (playMode != PlayModeStateChange.EnteredPlayMode)
        {
            if (playMode == PlayModeStateChange.EnteredEditMode || playMode == PlayModeStateChange.ExitingPlayMode)
            {
                if (ShaderPassHolder.Pass != null)
                    ShaderPassHolder.DisablePass();
            }
            return;
        }

        if (ShaderPassHolder.Pass != null)
            ShaderPassHolder.EnablePass();

        var gameWindow = EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        // We only do the force focus if Focused or Maximized play mode selected.
        // We do not force focus when Unfocused selected.
        if (!gameWindow.maximized && !PlayModeWindow.GetPlayModeFocused()) return;

        gameWindow.Focus();
        gameWindow.SendEvent(new Event
        {
            button = 0,
            clickCount = 1,
            type = EventType.MouseDown,
            mousePosition = gameWindow.rootVisualElement.contentRect.center
        });
    }

    private static void PauseStateChanged(PauseState state)
    {
        if (state == PauseState.Paused)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}