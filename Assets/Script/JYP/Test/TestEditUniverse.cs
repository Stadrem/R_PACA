using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModels;

public class TestEditUniverse : MonoBehaviour
{
    
    private UniverseEditViewModel viewModel;
    
    private void Start()
    {
        viewModel = ViewModelManager.Instance.UniverseEditViewModel;
        DoAfterSec(
            2f,
            () =>
            {
                viewModel.Characters = new List<CharacterInfo>()
                {
                    new CharacterInfo()
                    {
                        id = 1,
                        name = "Test1",
                        isPlayable = true,
                    },

                    new CharacterInfo()
                    {
                        id = 2,
                        name = "Test2",
                        shapeType = ECharacterShapeType.Goblin,
                        isPlayable = false,
                    },

                    new CharacterInfo()
                    {
                        id = 3,
                        name = "Test3",
                        shapeType = ECharacterShapeType.Human,
                        isPlayable = false,
                    },
                };
            }
        );
    }


    private void DoAfterSec(float sec, Action action)
    {
        StartCoroutine(DoAfterSecCoroutine(sec, action));
    }

    private IEnumerator DoAfterSecCoroutine(float sec, Action action)
    {
        yield return new WaitForSeconds(sec);
        action();
    }

}

