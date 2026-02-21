using UnityEngine;
using Farming;

namespace Character
{
    public class RayCast : TileSelector 
    {
        [SerializeField] private float rayDistance = 5f;
        // Update is called once per frame
        void Update()
        {
            SelectTile();
        }
        void SelectTile()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if(Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance))
            {
                if(hitInfo.collider.TryGetComponent<FarmTile>(out FarmTile tile))
                {
                    SetActiveTile(tile);
                }
            }
            else
            {
                SetActiveTile(null);
            }
        }
    }

}
