using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class LevelButtons : MonoBehaviour
{
    [ReadOnly(true), SerializeField] private List<Button> buttons;
    [ReadOnly(true), SerializeField] private List<int> levels;

    private void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var level = levels[i];
            var button = buttons[i];

            button
                .OnClickAsObservable()
                .Subscribe(delegate { OnClickHandler(level); })
                .AddTo(gameObject);
        }
    }

    private void OnClickHandler(int level)
    {
        SceneLevelModel.SetCurrentSceneLevel(level);
        SceneLevelModel.GetSceneLevel().OnNext(level);
    }
}