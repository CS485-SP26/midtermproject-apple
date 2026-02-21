using UnityEngine;

namespace Character {
    [RequireComponent(typeof(MovementController))]
    public class AnimatedController : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        [SerializeField] float moveSpeed; // useful to observe for debugging
        private float previousSpeed = -1f;
        private MovementController moveController;
        private Animator animator;
        protected Animator Animator { get { return animator; } }

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            moveController = GetComponent<MovementController>();
            Debug.Assert(animator, "AnimatedController requires an Animator in children.");
            Debug.Assert(moveController, "AnimatedController requires a MovementController.");
        }

        public void SetTrigger(string name)
        {
            if (animator != null)
            {
                animator.SetTrigger(name);
            }
        }

        void Update()
        {
            if (animator == null || moveController == null) return;

            moveSpeed = moveController.GetHorizontalSpeedPercent();
            if (!Mathf.Approximately(moveSpeed, previousSpeed))
            {
                previousSpeed = moveSpeed;
                animator.SetFloat(SpeedHash, moveSpeed);
            }
        }
    }
}
