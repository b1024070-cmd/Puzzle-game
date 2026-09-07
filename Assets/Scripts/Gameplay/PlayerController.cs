using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PuzzleBoard board;

    public void Initialize(PuzzleBoard board)
    {
        this.board = board;
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        Vector2Int direction = Vector2Int.zero;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
            direction = new Vector2Int(0, -1);
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
            direction = new Vector2Int(0, 1);
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            direction = new Vector2Int(-1, 0);
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame)
            direction = new Vector2Int(1, 0);

        if (direction != Vector2Int.zero && board != null)
        {
            board.TryMove(direction);
        }
    }
}