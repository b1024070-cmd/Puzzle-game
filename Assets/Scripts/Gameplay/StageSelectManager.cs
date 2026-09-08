using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectManager : MonoBehaviour
{
    [SerializeField] private GameFlowManager gameFlowManager;

    // Inspectorで、Stage1Button ~ Stage5Button の順に登録する
    [SerializeField] private Button[] stageButtons;

    void Awake()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNumber = i + 1; // ループ変数をそのまま使うとバグの元になるため、ローカル変数にコピーする
            stageButtons[i].onClick.AddListener(() => gameFlowManager.StartStage(stageNumber));
        }
    }

    // ボタンの見た目(押せるか、番号か鍵マークか)を最新の進捗にあわせて更新する
    public void RefreshButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            int stageNumber = i + 1;
            bool unlocked = StageProgress.IsUnlocked(stageNumber);

            stageButtons[i].interactable = unlocked;

            TextMeshProUGUI label = stageButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = unlocked ? stageNumber.ToString() : "Lock";
            }
        }
    }
}