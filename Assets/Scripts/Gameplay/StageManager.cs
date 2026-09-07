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

    private PuzzleBoard board;
    private bool cleared;
    private bool failed;
    private float remainingTime;
    private float elapsedTime;

    void Start()
    {
        retryButton.onClick.AddListener(RetryStage);
        StartStage();
    }

    private void StartStage()
    {
        cleared = false;
        failed = false;
        elapsedTime = 0f;
        remainingTime = view.StageData.timeLimit;

        resultPanel.SetActive(false);

        board = new PuzzleBoard(view.StageData);
        view.Initialize();
        view.PlayerController.Initialize(board);

        UpdateTimerDisplay();
    }

    void Update()
    {
        if (cleared || failed) return;

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
        if (cleared || failed) return;

        view.UpdateVisuals(board);
        CheckChestOpen();
        CheckWallBroken();

        if (board.IsCleared())
        {
            cleared = true;
            ShowResult(true);
        }
    }

    private void CheckChestOpen()
    {
        if (board.TryOpenChest(board.PlayerPosition))
        {
            view.RemoveChestVisual(board.PlayerPosition);

            ItemEffectType effect = ItemEffectPicker.PickGoodEffect();
            ApplyEffect(effect);
        }
    }

    private void CheckWallBroken()
    {
        Vector2Int? broken = board.ConsumeLastWallBroken();
        if (broken.HasValue)
        {
            view.BreakWallVisual(broken.Value);
            Debug.Log("壁を破壊しました: " + broken.Value);
        }
    }

    private void ApplyEffect(ItemEffectType effect)
    {
        switch (effect)
        {
            case ItemEffectType.WallBreak:
                board.ActivateWallBreak();
                Debug.Log("効果: 壁破壊モード発動 - 次に壁へ向かうと壊せます");
                break;
            case ItemEffectType.TimeExtend:
                remainingTime += 15f;
                Debug.Log("効果: 時間延長 +15秒");
                break;
            case ItemEffectType.SpeedUp:
                view.PlayerController.ApplySpeedBoost(2f, 5f);
                Debug.Log("効果: スピードアップ(2倍速・5秒間)");
                break;
        }
    }

    private void ShowResult(bool isCleared)
    {
        resultPanel.SetActive(true);

        if (isCleared)
        {
            resultText.text = $"CLEAR!\nTime: {elapsedTime:F2}s";
        }
        else
        {
            resultText.text = "Time's up...";
        }
    }

    private void RetryStage()
    {
        foreach (Transform child in view.transform)
        {
            Destroy(child.gameObject);
        }

        StartStage();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int displaySeconds = Mathf.CeilToInt(remainingTime);
        timerText.text = displaySeconds.ToString();
    }
}