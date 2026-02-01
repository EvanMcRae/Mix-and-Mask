using System;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class Utils
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }
    public static void SetNavigation(Selectable source, Selectable dest, Direction dir)
    {
        if (source == null) return;
        Navigation nav = source.navigation;
        switch (dir)
        {
            case Direction.UP:
                nav.selectOnUp = dest;
                break;
            case Direction.DOWN:
                nav.selectOnDown = dest;
                break;
            case Direction.LEFT:
                nav.selectOnLeft = dest;
                break;
            case Direction.RIGHT:
                nav.selectOnRight = dest;
                break;
        }
        source.navigation = nav;
    }

    public static void SetExclusiveAction(ref Action source, Action target)
    {
        if (source != null)
        {
            foreach (Action action in source.GetInvocationList())
            {
                source -= action;
            }
        }
        if (target != null)
            source += target;
    }

    public static void KillTween(ref Tween currTween)
    {
        if (currTween != null && currTween.IsActive() && !currTween.IsComplete())
        {
            currTween.Kill();
            currTween = null;
        }
    }

    public static bool IsWebPlayer()
    {
        return Application.platform == RuntimePlatform.WebGLPlayer
#if UNITY_EDITOR
        || EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL
#endif
        ;
    }
}