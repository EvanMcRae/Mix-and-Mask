using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int EnemiesPossessed = 0, EnemiesKilled = 0;

    void Start()
    {
        EnemiesPossessed = 0;
        EnemiesKilled = 0;
    }

    void OnDestroy()
    {
        EnemiesPossessed = 0;
        EnemiesKilled = 0;
    }
}