using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageManager : MonoBehaviour
{
    [Header("盤面")]
    [SerializeField] private PuzzleView view;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("結果画面")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button backToSelectButton;

    private PuzzleBoard board;
    private StageData currentStageData;
    private int currentStageNumber;
    private bool cleared;
    private bool failed;
    private float remainingTime;
    private float elapsedTime;
    private bool isRunning;

    // クリアした時に呼ばれる(GameFlowManagerが進捗を更新するために使う)
    public event Action<int> OnStageCleared;

    // 選択画面に戻る操作をされた時に呼ばれる
    public event Action OnBackToSelect;

    void Awake()
    {
        retryButton.onClick.AddListener(RetryStage);
        backToSelectButton.onClick.AddListener(() => OnBackToSelect?.Invoke());
    }

    // GameFlowManagerから呼び出す、ステージ開始のエントリーポイント
    public void BeginStage(StageData stageData, int stageNumber)
    {
        currentStageData = stageData;
        currentStageNumber = stageNumber;
        isRunning = true;

        StartStage();
    }

    public void StopStage()
    {
        isRunning = false;
        view.ClearBoard();
    }

    private void StartStage()
    {
        cleared = false;
        failed = false;
        elapsedTime = 0f;
        remainingTime = currentStageData.timeLimit;

        resultPanel.SetActive(false);

        view.ClearBoard();
        view.Initialize(currentStageData);

        board = new PuzzleBoard(currentStageData);
        view.PlayerController.Initialize(board);

        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!isRunning || cleared || failed) return;

        elapsedTime += Time.deltaTime;
        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            failed = true;
            ShowResult(false);
        }

        UpdateTimerDisplay();
    }

    void LateUpdate()
    {
        if (!isRunning || cleared || failed) return;

        view.UpdateVisuals(board);
        CheckChestOpen();
        CheckWallBroken();

        if (board.IsCleared())
        {
            cleared = true;
            StageProgress.MarkCleared(currentStageNumber);
            OnStageCleared?.Invoke(currentStageNumber);
            ShowResult(true);
        }
    }

    private void CheckChestOpen()
    {
        if (board.TryOpenChest(board.PlayerPosition))
        {
            view.RemoveChestVisual(board.PlayerPosition);

            ItemEffectType effect = ItemEffectPicker.PickEffect();
            ApplyEffect(effect);
        }
    }

    private void CheckWallBroken()
    {
        Vector2Int? broken = board.ConsumeLastWallBroken();
        if (broken.HasValue)
        {
            view.BreakWallVisual(broken.Value);
        }
    }

    private void ApplyEffect(ItemEffectType effect)
    {
        switch (effect)
        {
            case ItemEffectType.WallBreak:
                board.ActivateWallBreak();
                break;

            case ItemEffectType.TimeExtend:
                remainingTime += 15f;
                break;

            case ItemEffectType.SpeedUp:
                view.PlayerController.ApplySpeedEffect(2f, 5f);
                break;

            case ItemEffectType.SpeedDown:
                view.PlayerController.ApplySpeedEffect(0.5f, 5f);
                break;

            case ItemEffectType.AddWall:
                board.AddRandomWall();
                Vector2Int? added = board.ConsumeLastWallAdded();
                if (added.HasValue)
                {
                    view.AddWallVisual(added.Value);
                }
                break;
        }
    }

    private void ShowResult(bool isCleared)
    {
        resultPanel.SetActive(true);

        resultText.text = isCleared
            ? $"CLEAR!\nTime: {elapsedTime:F2}s"
            : "Time's up...";
    }

    private void RetryStage()
    {
        StartStage();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int displaySeconds = Mathf.CeilToInt(remainingTime);
        timerText.text = displaySeconds.ToString();
    }
}