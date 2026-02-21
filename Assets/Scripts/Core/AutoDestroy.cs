using UnityEngine;

namespace Core {
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float lifespan = 5f;
        void Start()
        {
            if (lifespan <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            Destroy(gameObject, lifespan);
        }
    }
}
