using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UpdateAbilitiesIcons : MonoBehaviour
{
    [SerializeField] Image rightImageA;
    [SerializeField] Image rightImageB;


    //test Sprites
    [SerializeField] Sprite[] iconSprites;

    //test variable
    int testIndex = 0;

    public void UpdateAbilityIcons(int index)
    {
        if (index < 0 || iconSprites.Length <= index)
        {
            return;
        }

        rightImageA.sprite = iconSprites[testIndex];
        
        rightImageB.sprite = iconSprites[testIndex+1 == iconSprites.Length?0: testIndex + 1];
    }

    public void IncremenetIndexes()
    {
        testIndex++;
        if (testIndex < 0 || testIndex >= iconSprites.Length)
        {
            testIndex = 0;
        }
        UpdateAbilityIcons(testIndex);
    }

    //Need more sophisticated "set these two icons logic"
    //public void UpdateAbilityIcons(some enum)
    //{
    //    //switch statement of some kind
    //
    //    rightImageA.sprite = iconSprites[givenIndex];
    //    rightImageB.sprite = iconSprites[givenIndex + 1];
    //}

}
