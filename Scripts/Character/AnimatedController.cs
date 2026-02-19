using Character;
using UnityEngine;

namespace Character {
    public class AnimatedController : MonoBehaviour
    {
        [SerializeField] float moveSpeed; // useful to observe for debugging
        MovementController moveController;
        Animator animator;
        protected Animator Animator { get { return animator; } }
        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            Debug.Log("Animator found: " + animator);
            moveController = GetComponent<MovementController>();
        }

        public void SetTrigger(string name)
        {
            Debug.Log("SetTrigger called: " + name);
            animator.SetTrigger(name);
        }

        void Update()
        {
            moveSpeed = moveController.GetHorizontalSpeedPercent();
            animator.SetFloat("Speed", moveSpeed);
        }
    }
}
