using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("5ステージ分のデータ(Stage1~Stage5の順)")]
    [SerializeField] private StageData[] stageDataList;

    [Header("参照")]
    [SerializeField] private StageSelectManager stageSelectManager;
    [SerializeField] private StageManager stageManager;

    [Header("画面切り替え用パネル")]
    [SerializeField] private GameObject stageSelectPanel;
    [SerializeField] private GameObject gameplayPanel;

    void Start()
    {
        stageManager.OnBackToSelect += HandleBackToSelect;
        ShowStageSelect();
    }

    // ステージ選択画面のボタンから呼ばれる
    public void StartStage(int stageNumber)
    {
        stageSelectPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        StageData data = stageDataList[stageNumber - 1];
        stageManager.BeginStage(data, stageNumber);
    }

    private void HandleBackToSelect()
    {
        stageManager.StopStage();
        gameplayPanel.SetActive(false);
        ShowStageSelect();
    }

    private void ShowStageSelect()
    {
        stageSelectPanel.SetActive(true);
        stageSelectManager.RefreshButtons();
    }
}