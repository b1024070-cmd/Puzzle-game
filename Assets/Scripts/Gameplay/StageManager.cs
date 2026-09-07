using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private PuzzleView view;

    private PuzzleBoard board;
    private bool cleared;

    void Start()
    {
        board = new PuzzleBoard(view.StageData);
        view.Initialize();
        view.PlayerController.Initialize(board);
    }

    void LateUpdate()
    {
        if (cleared) return;

        view.UpdateVisuals(board);

        if (board.IsCleared())
        {
            cleared = true;
            Debug.Log("ステージクリア！");
        }
    }
}