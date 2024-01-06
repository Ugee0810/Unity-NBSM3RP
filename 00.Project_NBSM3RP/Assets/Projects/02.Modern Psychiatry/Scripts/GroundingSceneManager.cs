/* 
게임 클리어 or 실패 로직
    물건 찾고 이미지 변경 로직
    시간 슬라이더 + 텍스트
    초기 제한 시간 10/8/5초, 물체 찾을 때 마다 2/1.5/1초씩 추가
글꼴 변경
*/

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using TMPro;

public class GroundingSceneManager : MonoBehaviour
{
    [Header("[전체 이미지 및 타겟 포지션]")]
    [SerializeField] private List<Sprites> easySprites;
    [SerializeField] private List<Sprites> normalSprites;
    [SerializeField] private List<Sprites> hardSprites;
    [SerializeField] private List<GameItem> gameItems;

    [Space(10), Header("[UGUI]")]
    [ReadOnly(false), SerializeField] private CanvasGroup canvasGroup;
    [ReadOnly(false), SerializeField] private RectTransform cursorCircle;
    [ReadOnly(false), SerializeField] private Image sliderImage;
    [ReadOnly(false), SerializeField] private TMP_Text sliderText;
    [ReadOnly(false), SerializeField] private List<Button> targetButtons;

    [Space(10), Header("[Current Game Info]")]
    [ReadOnly(false), SerializeField] private Transform currentBackGround;
    [ReadOnly(false), SerializeField] private List<Sprite> currentQuestSprites;
    [ReadOnly(false), SerializeField] private List<Sprite> currentHintSprites;
    [ReadOnly(false), SerializeField] private List<Sprite> currentItemSprites;
    [ReadOnly(false), SerializeField] private List<GameItem> currentTargets;

    [Space(10), Header("[SFX]")]
    [ReadOnly(false), SerializeField] private AudioSource sfx;
    [ReadOnly(false), SerializeField] private bool isSFXPlay = false;

    [Space(10), Header("[ETC]")]
    [SerializeField] private float moveSpeed = 3.0f;
    [SerializeField] private float minX = -1550.0f;
    [SerializeField] private float maxX = 2600.0f;
    private CompositeDisposable clearButtonDisposables = new();

    private void Start()
    {
        Observable
            .EveryUpdate()
            .Select(_ => canvasGroup.alpha == 1)
            .Subscribe(delegate
                {
                    MouseMovementRestriction();
                    DetectionRelatedHandler();
                    PlaySFX();
                })
            .AddTo(gameObject);

        GroundModel.OnGameStart += OnGameStart;
    }

    private void MouseMovementRestriction()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        float directionX = mousePosition.x - screenCenter.x;
        Vector3 newPosition = currentBackGround.position - new Vector3(directionX, 0, 0) * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(
            value: newPosition.x,
            min: minX,
            max: maxX
        );
        currentBackGround.position = newPosition;
    }

    private void DetectionRelatedHandler()
    {
        Vector3 mousePosition = Input.mousePosition;

        cursorCircle.position = mousePosition;
    }

    private void PlaySFX()
    {
        if (canvasGroup.alpha == 0)
        {
            if (sfx.isPlaying)
            {
                sfx.Stop();
            }

            isSFXPlay = false;
        }
        else
        {
            if (isSFXPlay)
            {
                return;
            }
            else
            {
                sfx.Play();

                isSFXPlay = true;
            }
        }
    }

    private void OnGameStart(GroundModel.Difficulty curDifficulty, int number)
    {
        Debug.Log($"GroundingSceneManager.OnGameStart(): curDifficulty is [{curDifficulty}], Stage Num is [{number}]");

        clearButtonDisposables?.Dispose();
        clearButtonDisposables = new();

        currentQuestSprites.Clear();
        currentHintSprites.Clear();
        currentItemSprites.Clear();
        currentTargets.Clear();

        foreach (var target in targetButtons)
        {
            target.image = null;
            target.gameObject.SetActive(false);
        }

        targetButtons.First().gameObject.SetActive(true);

        switch (curDifficulty)
        {
            case GroundModel.Difficulty.Easy:
                switch (number)
                {
                    case 0:
                        AddResourcesByDifficulty(easySprites, 0, 4);
                        break;
                    case 1:
                        AddResourcesByDifficulty(easySprites, 5, 10);
                        break;
                }

                ActivateButtons(5);
                break;
            case GroundModel.Difficulty.Normal:
                switch (number)
                {
                    case 0:
                        AddResourcesByDifficulty(normalSprites, 0, 6);
                        break;
                    case 1:
                        AddResourcesByDifficulty(normalSprites, 7, 12);
                        break;
                    case 2:
                        AddResourcesByDifficulty(normalSprites, 13, 18);
                        break;
                    case 3:
                        AddResourcesByDifficulty(normalSprites, 18, 24);
                        break;
                }

                ActivateButtons(6);
                break;
            case GroundModel.Difficulty.Hard:
                switch (number)
                {
                    case 0:
                        AddResourcesByDifficulty(hardSprites, 0, 8);
                        break;
                    case 1:
                        AddResourcesByDifficulty(hardSprites, 9, 16);
                        break;
                    case 2:
                        AddResourcesByDifficulty(hardSprites, 16, 24);
                        break;
                    case 3:
                        AddResourcesByDifficulty(hardSprites, 25, 32);
                        break;
                    case 4:
                        AddResourcesByDifficulty(hardSprites, 33, 40);
                        break;
                }

                ActivateButtons(8);
                break;
        }

        FindLastActiveImageButton()
            .OnClickAsObservable()
            .Subscribe(delegate { OnGameClear(); })
            .AddTo(clearButtonDisposables);

        void AddResourcesByDifficulty(List<Sprites> currentSprites, int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (currentSprites[i] != null)
                {
                    if (currentSprites[i].quest != null)
                    {
                        currentQuestSprites.Add(currentSprites[i].quest);
                    }

                    if (currentSprites[i].hint != null)
                    {
                        currentHintSprites.Add(currentSprites[i].hint);
                    }

                    if (currentSprites[i].item != null)
                    {
                        currentItemSprites.Add(currentSprites[i].item);
                    }
                }

                if (gameItems[i] != null)
                {
                    currentTargets.Add(gameItems[i]);
                }
            }
        }

        void ActivateButtons(int count)
        {
            for (int i = 0; i < count; i++)
            {
                targetButtons[i].gameObject.SetActive(true);
            }
        }

        Button FindLastActiveImageButton()
        {
            for (int i = targetButtons.Count - 1; i >= 0; i--)
            {
                Image imageComponent = targetButtons[i].GetComponent<Image>();

                if (imageComponent != null && imageComponent.enabled)
                {
                    return targetButtons[i];
                }
            }

            return null;
        }
    }

    private void OnGameClear()
        => GroundModel.RaiseOnGameClear();

    private void OnGameFail()
        => GroundModel.RaiseOnGameFail();
    
    private void SetCurrentTargets()
    {
        for (int i = 0; i < currentTargets.Count; i++)
        {
            targetButtons[i].transform.position = currentTargets[i].anchoredPos;
            targetButtons[i].image.sprite = currentItemSprites[i];
        }
    }
}