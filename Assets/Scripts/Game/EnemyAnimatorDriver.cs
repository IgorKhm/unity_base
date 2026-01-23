using UnityEngine;

public class EnemyAnimatorDriver : MonoBehaviour
{
    private Animator anim;
    private EnemyVision2D vision;
    private EnemyChaser2D chaser;

    void Awake()
    {
        anim = GetComponent<Animator>();
        vision = GetComponentInParent<EnemyVision2D>();
        chaser = GetComponentInParent<EnemyChaser2D>();
    }

    void Update()
    {
        if (anim == null) return;

        bool canSee = vision != null && vision.CanSeePlayer;
        // bool isAttacking = canSee && (chaser == null || !chaser.IsStunned);
        
        anim.SetBool("IsAttacking", canSee);

    }
}