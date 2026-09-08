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
    [SerializeField] private Button nextStageButton;

    [Header("ポーズ")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button pauseRetryButton;
    [SerializeField] private Button pauseBackToSelectButton;

    [Header("アイテム効果メッセージ")]
    [SerializeField] private TextMeshProUGUI effectMessageText;
    [SerializeField] private float effectMessageDuration = 2f;

    private PuzzleBoard board;
    private StageData currentStageData;
    private int currentStageNumber;
    private bool isLastStage;
    private bool cleared;
    private bool failed;
    private bool isPaused;
    private bool isRunning;
    private float remainingTime;
    private float elapsedTime;
    private float effectMessageTimer;

    public event Action<int> OnStageCleared;
    public event Action OnBackToSelect;
    public event Action OnRequestNextStage;

    void Awake()
    {
        retryButton.onClick.AddListener(RetryStage);
        backToSelectButton.onClick.AddListener(RequestBackToSelect);
        nextStageButton.onClick.AddListener(() => OnRequestNextStage?.Invoke());

        pauseButton.onClick.AddListener(PauseStage);
        resumeButton.onClick.AddListener(ResumeStage);
        pauseRetryButton.onClick.AddListener(RetryStage);
        pauseBackToSelectButton.onClick.AddListener(RequestBackToSelect);
    }

    public void BeginStage(StageData stageData, int stageNumber, bool isLastStage)
    {
        currentStageData = stageData;
        currentStageNumber = stageNumber;
        this.isLastStage = isLastStage;
        isRunning = true;

        StartStage();
    }

    public void StopStage()
    {
        isRunning = false;
        isPaused = false;
        pausePanel.SetActive(false);
        view.ClearBoard();
    }

    private void StartStage()
    {
        cleared = false;
        failed = false;
        isPaused = false;
        elapsedTime = 0f;
        remainingTime = currentStageData.timeLimit;
        effectMessageTimer = 0f;

        resultPanel.SetActive(false);
        pausePanel.SetActive(false);
        effectMessageText.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(true); // ステージ開始時にPauseボタンを再表示

        view.ClearBoard();
        view.Initialize(currentStageData);

        board = new PuzzleBoard(currentStageData);
        view.PlayerController.Initialize(board);

        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!isRunning) return;

        UpdateEffectMessageTimer();

        if (isPaused || cleared || failed) return;

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
        if (!isRunning || isPaused || cleared || failed) return;

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

    // ---- ポーズ処理 ----

    private void PauseStage()
    {
        if (!isRunning || cleared || failed) return;

        isPaused = true;
        view.PlayerController.enabled = false;
        pausePanel.SetActive(true);
    }

    private void ResumeStage()
    {
        isPaused = false;
        view.PlayerController.enabled = true;
        pausePanel.SetActive(false);
    }

    private void RequestBackToSelect()
    {
        pausePanel.SetActive(false);
        OnBackToSelect?.Invoke();
    }

    // ---- アイテム効果 ----

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
                ShowEffectMessage("Wall Break! Head into a wall");
                break;

            case ItemEffectType.TimeExtend:
                remainingTime += 15f;
                ShowEffectMessage("Time +15s!");
                break;

            case ItemEffectType.SpeedUp:
                view.PlayerController.ApplySpeedEffect(2f, 5f);
                ShowEffectMessage("Speed Up!");
                break;

            case ItemEffectType.SpeedDown:
                view.PlayerController.ApplySpeedEffect(0.5f, 5f);
                ShowEffectMessage("Speed Down...");
                break;

            case ItemEffectType.AddWall:
                board.AddRandomWall();
                Vector2Int? added = board.ConsumeLastWallAdded();
                if (added.HasValue)
                {
                    view.AddWallVisual(added.Value);
                }
                ShowEffectMessage("A wall appeared!");
                break;
        }
    }

    private void ShowEffectMessage(string message)
    {
        effectMessageText.text = message;
        effectMessageText.gameObject.SetActive(true);
        effectMessageTimer = effectMessageDuration;
    }

    private void UpdateEffectMessageTimer()
    {
        if (effectMessageTimer <= 0f) return;

        effectMessageTimer -= Time.deltaTime;
        if (effectMessageTimer <= 0f)
        {
            effectMessageText.gameObject.SetActive(false);
        }
    }

    // ---- 結果画面 ----

    private void ShowResult(bool isCleared)
    {
        resultPanel.SetActive(true);
        pauseButton.gameObject.SetActive(false); // 結果画面ではPauseボタンを隠す

        if (isCleared)
        {
            resultText.text = isLastStage
                ? $"ALL CLEAR!\nTime: {elapsedTime:F2}s"
                : $"CLEAR!\nTime: {elapsedTime:F2}s";

            nextStageButton.gameObject.SetActive(!isLastStage);
        }
        else
        {
            resultText.text = "Time's up...";
            nextStageButton.gameObject.SetActive(false);
        }
    }

    private void RetryStage()
    {
        pausePanel.SetActive(false);
        StartStage();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int displaySeconds = Mathf.CeilToInt(remainingTime);
        timerText.text = displaySeconds.ToString();
    }
}