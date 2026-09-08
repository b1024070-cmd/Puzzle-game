using UnityEngine;

// ステージの進捗(どこまでクリアしたか)を管理する。
// PlayerPrefsを使うことで、アプリを閉じても記録が残る。
public static class StageProgress
{
    private const string HighestClearedKey = "HighestClearedStage";

    // まだ1つもクリアしていない状態を 0 として扱う
    public static int HighestClearedStage
    {
        get => PlayerPrefs.GetInt(HighestClearedKey, 0);
        private set => PlayerPrefs.SetInt(HighestClearedKey, value);
    }

    // 指定したステージ番号(1始まり)が選択可能かどうか
    // ステージ1は常に解放。それ以降は、直前のステージをクリアしていれば解放される
    public static bool IsUnlocked(int stageNumber)
    {
        if (stageNumber <= 1) return true;
        return stageNumber - 1 <= HighestClearedStage;
    }

    // 指定したステージ番号をクリア済みとして記録する
    public static void MarkCleared(int stageNumber)
    {
        if (stageNumber > HighestClearedStage)
        {
            HighestClearedStage = stageNumber;
            PlayerPrefs.Save();
        }
    }

    // 開発中の動作確認用:進捗を初期化する
    public static void ResetProgress()
    {
        HighestClearedStage = 0;
        PlayerPrefs.Save();
    }
}