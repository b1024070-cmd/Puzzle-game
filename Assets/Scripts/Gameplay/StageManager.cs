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
    private float elapsedTime; // クリアタイム計測用

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

        if (board.IsCleared())
        {
            cleared = true;
            ShowResult(true);
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
        // 前回生成したタイル・プレイヤー・ブロックを全部消してから作り直す
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