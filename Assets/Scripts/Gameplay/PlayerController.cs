using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const float BaseMoveInterval = 0.15f;

    private PuzzleBoard board;
    private float moveTimer;
    private float speedMultiplier = 1f;
    private float speedEffectRemaining;

    public void Initialize(PuzzleBoard board)
    {
        this.board = board;
    }

    void Update()
    {
        if (Keyboard.current == null || board == null) return;

        UpdateSpeedEffect();

        Vector2Int direction = GetHeldDirection();

        if (direction == Vector2Int.zero)
        {
            moveTimer = 0f;
            return;
        }

        bool justPressed = WasAnyDirectionPressedThisFrame();
        if (justPressed)
        {
            board.TryMove(direction);
            moveTimer = 0f;
            return;
        }

        moveTimer += Time.deltaTime;
        float currentInterval = BaseMoveInterval / speedMultiplier;

        if (moveTimer >= currentInterval)
        {
            board.TryMove(direction);
            moveTimer = 0f;
        }
    }

    // 一定時間だけ移動速度を変更する(1より大きいと速く、小さいと遅くなる)
    public void ApplySpeedEffect(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        speedEffectRemaining = duration;
    }

    private void UpdateSpeedEffect()
    {
        if (speedEffectRemaining <= 0f) return;

        speedEffectRemaining -= Time.deltaTime;
        if (speedEffectRemaining <= 0f)
        {
            speedEffectRemaining = 0f;
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