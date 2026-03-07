using UnityEngine;

namespace Character {
    [RequireComponent(typeof(MovementController))]
    public class AnimatedController : MonoBehaviour
    {
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        [SerializeField] private float moveSpeed; // useful to observe for debugging
        [SerializeField] private Animator animator;
        [SerializeField] private MovementController moveController;
        private bool hasSpeedParameter;
        protected Animator Animator { get { return animator; } }

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (moveController == null)
            {
                moveController = GetComponent<MovementController>();
            }

            Debug.Assert(animator, "AnimatedController requires an Animator in children.");
            Debug.Assert(moveController, "AnimatedController requires a MovementController.");

            hasSpeedParameter = false;
            if (animator != null)
            {
                foreach (var parameter in animator.parameters)
                {
                    if (parameter.type == AnimatorControllerParameterType.Float && parameter.nameHash == SpeedHash)
                    {
                        hasSpeedParameter = true;
                        break;
                    }
                }

                if (!hasSpeedParameter)
                {
                    Debug.LogError("Animator is missing required float parameter 'Speed'.");
                }
            }
        }

        public void SetTrigger(string name)
        {
            if (animator != null && !string.IsNullOrWhiteSpace(name))
            {
                animator.SetTrigger(name);
            }
        }

        private void Update()
        {
            if (animator == null || moveController == null || !hasSpeedParameter) return;

            moveSpeed = moveController.GetHorizontalSpeedPercent();
            animator.SetFloat(SpeedHash, moveSpeed);
        }
    }
}
