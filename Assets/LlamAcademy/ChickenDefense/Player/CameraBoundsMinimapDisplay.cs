using UnityEngine;

namespace LlamAcademy.ChickenDefense.Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class CameraBoundsMinimapDisplay : MonoBehaviour
    {
        [SerializeField] private Camera Camera;
        private LineRenderer LineRenderer;
        private Ray[] Rays = new Ray[4];
        [SerializeField]
        private LayerMask FloorLayers;

        private void Awake()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.positionCount = 4;
        }

        private void Update()
        {
            Rays[0] = Camera.ViewportPointToRay(new Vector3(0, 0, 0)); // bottom left
            Rays[1] = Camera.ViewportPointToRay(new Vector3(0, 1, 0)); // top left
            Rays[2] = Camera.ViewportPointToRay(new Vector3(1, 1, 0)); // top right
            Rays[3] = Camera.ViewportPointToRay(new Vector3(1, 0, 0)); // bottom right
            float cameraHeight = -Camera.transform.position.y;
            for (int i = 0; i < 4; i++)
            {
                if (Physics.Raycast(Rays[i], out RaycastHit hit, float.MaxValue, FloorLayers))
                {
                    LineRenderer.SetPosition(i, hit.point + Vector3.up * 0.01f); //-hit.distance));
                }
            }
        }

        private static Vector3 GetPointAtHeight(Ray ray, float height)
        {
            return ray.origin + (((ray.origin.y - height) / -ray.direction.y) * ray.direction) + Vector3.up * 0.01f;
        }
    }
}