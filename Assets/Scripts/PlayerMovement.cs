using UnityEngine;

/// <summary>
/// Represents the movement behavior of the player.
/// </summary>
public class MyPlayerMovement : MonoBehaviour
{
    public Animator animator;
    public bool walk = false;
    public bool run = false;
    public bool shift = false;
    public bool canMove = true;
    private int isWalkingHash;
    private int isRunningHash;
    private int isShiftingHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
        isRunningHash = Animator.StringToHash("IsRunning");
        isShiftingHash = Animator.StringToHash("IsShifting");
    }

    // Update is called once per frame
    void Update()
    {
        // If we can't move, make sure we stop animating
        if (!canMove)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
            animator.SetBool(isShiftingHash, false);
            walk = run = shift = false;
            return;
        }

        animator.SetBool(isShiftingHash, shift);

        if (shift)
        {
            animator.SetBool(isWalkingHash, false);
            animator.SetBool(isRunningHash, false);
        }
        else
        {
            animator.SetBool(isWalkingHash, walk);
            animator.SetBool(isRunningHash, run);
        }
    }

}
