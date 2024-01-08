using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;
using DG.Tweening;

public class GroundingSceneManager : MonoBehaviour
{
    [Header("[Resources]")]
    [SerializeField] private List<Sprite> stageSprites;
    [SerializeField] private List<Sprite> logoSprites;
    [SerializeField] private List<Sprite> emptySliderSprites;
    [SerializeField] private List<Sprite> fillSliderSprites;
    [SerializeField] private List<Sprites> easySprites;
    [SerializeField] private List<Sprites> normalSprites;
    [SerializeField] private List<Sprites> hardSprites;
    [SerializeField] private List<GameItem> gameItems;

    [Space(10), Header("[UGUI]")]
    [ReadOnly(false), SerializeField] private CanvasGroup canvasGroup;
    [ReadOnly(false), SerializeField] private Transform stage;
    [ReadOnly(false), SerializeField] private RectTransform cursorCircle;
    [ReadOnly(false), SerializeField] private Image questImage;
    [ReadOnly(false), SerializeField] private Image hintImage;
    [ReadOnly(false), SerializeField] private Image emptySliderImage;
    [ReadOnly(false), SerializeField] private Image fillSliderImage;
    [ReadOnly(false), SerializeField] private Image logoImage;
    [ReadOnly(false), SerializeField] private TMP_Text sliderText;
    [ReadOnly(false), SerializeField] private List<Button> targetButtons;

    [Space(10), Header("[Current Game Info]")]
    [ReadOnly(false), SerializeField] private Image curStage;
    [ReadOnly(false), SerializeField] private List<Sprite> curQuestSprites;
    [ReadOnly(false), SerializeField] private List<Sprite> curHintSprites;
    [ReadOnly(false), SerializeField] private List<Sprite> curTargetSprites;
    [ReadOnly(false), SerializeField] private List<GameItem> curTargets;

    [Space(10), Header("[SFX]")]
    [ReadOnly(false), SerializeField] private AudioSource sfx;

    [Space(10), Header("[ETC]")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float minX = -730.0f/* -1550.0f */;
    [SerializeField] private float maxX = 1230.0f/* 2600.0f */;
    [SerializeField] private float timeRemaining;
    [SerializeField] private float maxTime;
    [ReadOnly(false), SerializeField] private float rotationState = 0f;
    [ReadOnly(false), SerializeField] private bool isTouching = false;
    private CompositeDisposable clearDisposables = new();

    private void Start()
    {
        Observable
            .EveryUpdate()
            .Where(_ => canvasGroup.alpha == 1)
            .Subscribe(delegate
                {
                    TouchMovementRestriction();
                    OnUpdateRotateCursorCircle();
                    ToggleSound();
                    TimerUpdate();
                })
            .AddTo(gameObject);

        Observable
            .EveryUpdate()
            .Select(_ => canvasGroup.alpha == 0 && sfx.isPlaying)
            .DistinctUntilChanged()
            .Subscribe(delegate { sfx.Stop(); })
            .AddTo(gameObject);

        GroundModel.OnGameStart += OnGameStart;
    }

    private void TouchMovementRestriction()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        float directionX = mousePosition.x - screenCenter.x;
        Vector3 newPosition = stage.position - new Vector3(directionX, 0, 0) * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(
            value: newPosition.x,
            min: minX,
            max: maxX
        );

        stage.position = newPosition;
    }

    private void OnUpdateRotateCursorCircle()
    {
        if (!isTouching)
        {
            rotationState -= 360f / 4f * Time.deltaTime;

            Vector2 centerPoint = new(Screen.width / 2, Screen.height / 2);
            float radius = Mathf.Min(Screen.width, Screen.height) / 4;
            float radian = rotationState * Mathf.Deg2Rad;
            Vector2 newPosition = centerPoint + new Vector2(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius);

            cursorCircle.position = newPosition;
        }

        if (Input.GetMouseButtonDown(0))
        {
            isTouching = true;
            cursorCircle.DOKill();
        }

        if (Input.GetMouseButton(0))
        {
            cursorCircle
                .DOMove(Input.mousePosition, 0.2f)
                .SetEase(Ease.OutQuint);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Vector2 centerPoint = new(Screen.width / 2, Screen.height / 2);
            float radius = Mathf.Min(Screen.width, Screen.height) / 4;
            float radian = rotationState * Mathf.Deg2Rad;
            Vector2 newPosition = centerPoint + new Vector2(Mathf.Cos(radian) * radius, Mathf.Sin(radian) * radius);

            cursorCircle
                .DOMove(newPosition, 0.4f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                    {
                        isTouching = false;
                    }
            );
        }
    }

    private void ToggleSound()
    {
        if (sfx.isPlaying)
        {
            return;
        }
        else
        {
            sfx.Play();
        }
    }

    private void OnGameStart(GroundModel.Difficulty curDifficulty, int stageNum)
    {
        Debug.Log($"GroundingSceneManager.OnGameStart(): curDifficulty is [{curDifficulty}], Stage Num is [{stageNum}]");

        /// Initialization
        clearDisposables?.Dispose();
        clearDisposables = new();

        curStage.sprite = null;
        curQuestSprites.Clear();
        curHintSprites.Clear();
        curTargetSprites.Clear();
        curTargets.Clear();

        foreach (var target in targetButtons)
        {
            var image = target.GetComponent<Image>();
            image.raycastTarget = false;
            target.image = null;
            target.gameObject.SetActive(false);
        }

        /// Load Resources
        switch (curDifficulty)
        {
            case GroundModel.Difficulty.Easy:
                switch (stageNum)
                {
                    case 0:
                        AddSpriteResources(0, easySprites, 0, 5, 0);
                        AddTargetResources(0, 5);
                        break;
                    case 1:
                        AddSpriteResources(1, easySprites, 5, 10, 1);
                        AddTargetResources(5, 5);
                        break;
                }
                break;

            case GroundModel.Difficulty.Normal:
                switch (stageNum)
                {
                    case 0:
                        AddSpriteResources(2, normalSprites, 0, 6, 0);
                        AddTargetResources(10, 6);
                        break;
                    case 1:
                        AddSpriteResources(2, normalSprites, 6, 12, 0);
                        AddTargetResources(16, 6);
                        break;
                    case 2:
                        AddSpriteResources(3, normalSprites, 12, 18, 0);
                        AddTargetResources(22, 6);
                        break;
                    case 3:
                        AddSpriteResources(3, normalSprites, 18, 24, 1);
                        AddTargetResources(28, 6);
                        break;
                }
                break;

            case GroundModel.Difficulty.Hard:
                switch (stageNum)
                {
                    case 0:
                        AddSpriteResources(4, hardSprites, 0, 8, 0);
                        AddTargetResources(34, 8);
                        break;
                    case 1:
                        AddSpriteResources(4, hardSprites, 8, 16, 0);
                        AddTargetResources(42, 8);
                        break;
                    case 2:
                        AddSpriteResources(4, hardSprites, 16, 24, 0);
                        AddTargetResources(50, 8);
                        break;
                    case 3:
                        AddSpriteResources(5, hardSprites, 24, 32, 1);
                        AddTargetResources(58, 8);
                        break;
                    case 4:
                        AddSpriteResources(6, hardSprites, 32, 40, 1);
                        AddTargetResources(66, 8);
                        break;
                }
                break;
        }

        cursorCircle.position = Vector2.zero;

        SetInitialTime();

        GroundModel.InitializationStageLevel();

        GroundModel.StageLevel
            .AsObservable()
            .Subscribe(OnCurrentLevelHandler)
            .AddTo(clearDisposables);

        void AddSpriteResources(int stageNum, List<Sprites> curDifficultySprites, int startIndex, int endIndex, int mapInfo)
        {
            curStage.sprite = stageSprites[stageNum];
            logoImage.sprite = logoSprites[mapInfo];
            emptySliderImage.sprite = emptySliderSprites[mapInfo];
            fillSliderImage.sprite = fillSliderSprites[mapInfo];

            for (int i = startIndex; i < endIndex; i++)
            {
                curQuestSprites.Add(curDifficultySprites[i].quest);
                curHintSprites.Add(curDifficultySprites[i].hint);
                curTargetSprites.Add(curDifficultySprites[i].target);
            }
        }

        void AddTargetResources(int index, int count)
        {
            curTargets.AddRange(gameItems.GetRange(index, count));

            for (int i = 0; i < curTargets.Count; i++)
            {
                targetButtons[i].GetComponent<RectTransform>().anchoredPosition = curTargets[i].anchoredPos;
                targetButtons[i].gameObject.SetActive(true);
                var image = targetButtons[i].GetComponent<Image>();
                image.sprite = curTargetSprites[i];
                image.SetNativeSize();
            }
        }
    }

    private void OnCurrentLevelHandler(int level)
    {
        if (level >= curQuestSprites.Count && level >= curHintSprites.Count)
        {
            OnGameClear();
            return;
        }

        var image = targetButtons[level].GetComponent<Image>();
        image.raycastTarget = true;

        targetButtons[level]
            .OnClickAsObservable()
            .Subscribe(delegate
                {
                    FoundTarget();
                    targetButtons[level].gameObject.SetActive(false);
                    GroundModel.RaiseNextLevel();
                })
            .AddTo(clearDisposables);

        if (curQuestSprites[level] != null)
        {
            questImage.sprite = curQuestSprites[level];
        }

        if (curHintSprites[level] != null)
        {
            hintImage.sprite = curHintSprites[level];
        }

        Debug.Log($"GroundingSceneManager.OnCurrentLevelHandler(): GroundModel.CurrentLevel Value is [{level}]");
    }

    private void OnGameClear()
    {
        SceneLevelModel.CurrentSceneLevel = 13;
        SceneLevelModel.SceneLevel.OnNext(13);
        GroundModel.RaiseGameClear();
        Debug.Log($"GroundingSceneManager.OnGameClear()");
    }

    private void OnGameFail()
    {
        SceneLevelModel.CurrentSceneLevel = 14;
        SceneLevelModel.SceneLevel.OnNext(14);
        GroundModel.RaiseGameFail();
        Debug.Log($"GroundingSceneManager.OnGameFail()");
    }

    private void TimerUpdate()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            StartCoroutine(UpdateTimeUI(Time.deltaTime));
        }
        else if (timeRemaining <= 0)
        {
            OnGameFail();
        }
    }

    private void SetInitialTime()
    {
        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                maxTime = timeRemaining = 10f;
                break;
            case GroundModel.Difficulty.Normal:
                maxTime = timeRemaining = 8f;
                break;
            case GroundModel.Difficulty.Hard:
                maxTime = timeRemaining = 5f;
                break;
        }
        fillSliderImage.fillAmount = 1;
    }

    private void FoundTarget()
    {
        float addedTime = 0;

        switch (GroundModel.CurrentDifficulty.Value)
        {
            case GroundModel.Difficulty.Easy:
                addedTime = 2f;
                break;
            case GroundModel.Difficulty.Normal:
                addedTime = 1.5f;
                break;
            case GroundModel.Difficulty.Hard:
                addedTime = 1f;
                break;
        }

        timeRemaining = Mathf.Min(timeRemaining + addedTime, maxTime);

        StartCoroutine(UpdateTimeUI(0.5f));
    }

    private IEnumerator UpdateTimeUI(float animationDuration)
    {
        float targetFillAmount = timeRemaining / maxTime;
        float startFillAmount = fillSliderImage.fillAmount;
        float elapsedTime = 0;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newFillAmount = Mathf.Lerp(startFillAmount, targetFillAmount, elapsedTime / animationDuration);
            fillSliderImage.fillAmount = newFillAmount;
            sliderText.text = "제한 시간 : " + timeRemaining.ToString("F0") + "초";
            yield return null;
        }

        fillSliderImage.fillAmount = targetFillAmount;
    }
}