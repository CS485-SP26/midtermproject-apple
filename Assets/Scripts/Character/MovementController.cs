using UnityEngine;

namespace Character {
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] protected float acceleration = 20f;
        [SerializeField] protected float maxVelocity = 5f;
        protected Rigidbody rb;
        protected Vector2 moveInput;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Move(Vector2 lateralInput)
        {
            moveInput = Vector2.ClampMagnitude(lateralInput, 1f);
        }

        public void Stop()
        {
            if (rb == null)
            {
                return;
            }

            rb.linearVelocity = Vector3.zero;
            moveInput = Vector2.zero;
        }

        public virtual void Jump() { /* NO JUMP SUPPORT */ }

        public virtual float GetHorizontalSpeedPercent()
        {
            return moveInput.magnitude;
        }

        protected virtual void FixedUpdate()
        {
            SimpleMovement();
        }

        void SimpleMovement()
        {
            if (rb == null)
            {
                return;
            }

            Vector3 movement = Vector3.zero;
            movement += transform.right * moveInput.x;
            movement += transform.forward * moveInput.y;
            movement.Normalize();
            movement *= Time.fixedDeltaTime * acceleration;
            rb.MovePosition(rb.position + movement);
        }
    }
}