using System.Collections;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class CameraBoundsMinimapDisplay : MonoBehaviour
    {
        [SerializeField] private float InitialDelay = 2.5f;
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

        private void Start()
        {
            StartCoroutine(TraceCameraPosition());
        }

        private IEnumerator TraceCameraPosition()
        {
            yield return new WaitForSeconds(InitialDelay); // camera animation from "spawn location" to "game view" shows this buggy
            // if we show it immediately

            while (enabled)
            {
                Rays[0] = Camera.ViewportPointToRay(new Vector3(0, 0, 0)); // bottom left
                Rays[1] = Camera.ViewportPointToRay(new Vector3(0, 1, 0)); // top left
                Rays[2] = Camera.ViewportPointToRay(new Vector3(1, 1, 0)); // top right
                Rays[3] = Camera.ViewportPointToRay(new Vector3(1, 0, 0)); // bottom right

                bool allRaycastsHit = true;
                for (int i = 0; i < 4; i++)
                {
                    if (Physics.Raycast(Rays[i], out RaycastHit hit, float.MaxValue, FloorLayers))
                    {
                        LineRenderer.SetPosition(i, hit.point + Vector3.up * 0.01f);
                    }
                    else
                    {
                        allRaycastsHit = false;
                    }
                }

                LineRenderer.enabled = allRaycastsHit;

                yield return null;
            }
        }
    }
}