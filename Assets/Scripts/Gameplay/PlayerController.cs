using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const float BaseMoveInterval = 0.15f; // 通常時、1マス動くのにかかる時間

    private PuzzleBoard board;
    private float moveTimer;
    private float speedMultiplier = 1f;
    private float speedBoostRemaining;

    public void Initialize(PuzzleBoard board)
    {
        this.board = board;
    }

    void Update()
    {
        if (Keyboard.current == null || board == null) return;

        UpdateSpeedBoost();

        Vector2Int direction = GetHeldDirection();

        if (direction == Vector2Int.zero)
        {
            moveTimer = 0f; // キーが離されたらタイマーをリセット
            return;
        }

        // 押した瞬間は即座に1回動く
        bool justPressed = WasAnyDirectionPressedThisFrame();
        if (justPressed)
        {
            board.TryMove(direction);
            moveTimer = 0f;
            return;
        }

        // 押しっぱなしの間は、間隔ごとに連続移動する
        moveTimer += Time.deltaTime;
        float currentInterval = BaseMoveInterval / speedMultiplier;

        if (moveTimer >= currentInterval)
        {
            board.TryMove(direction);
            moveTimer = 0f;
        }
    }

    // 一定時間だけ移動速度を上げる(スピードアップ効果用)
    public void ApplySpeedBoost(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        speedBoostRemaining = duration;
    }

    private void UpdateSpeedBoost()
    {
        if (speedBoostRemaining <= 0f) return;

        speedBoostRemaining -= Time.deltaTime;
        if (speedBoostRemaining <= 0f)
        {
            speedBoostRemaining = 0f;
            speedMultiplier = 1f;
        }
    }

    private Vector2Int GetHeldDirection()
    {
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
            return new Vector2Int(0, -1);
        if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            return new Vector2Int(0, 1);
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
            return new Vector2Int(-1, 0);
        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
            return new Vector2Int(1, 0);

        return Vector2Int.zero;
    }

    private bool WasAnyDirectionPressedThisFrame()
    {
        return Keyboard.current.upArrowKey.wasPressedThisFrame
            || Keyboard.current.wKey.wasPressedThisFrame
            || Keyboard.current.downArrowKey.wasPressedThisFrame
            || Keyboard.current.sKey.wasPressedThisFrame
            || Keyboard.current.leftArrowKey.wasPressedThisFrame
            || Keyboard.current.aKey.wasPressedThisFrame
            || Keyboard.current.rightArrowKey.wasPressedThisFrame
            || Keyboard.current.dKey.wasPressedThisFrame;
    }
}