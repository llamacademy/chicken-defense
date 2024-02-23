using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Splines;

namespace LlamAcademy.ChickenDefense.Units.Enemies.Snake.Behaviors
{
    [RequireComponent(typeof(SplineContainer))]
    public class SnakeSplineAnimator : MonoBehaviour
    {
        [SerializeField] private float Speed = 1f;
        [SerializeField] private float MaxWidthOffset = 1f;
        [SerializeField] private Transform Head;
        [SerializeField] private Transform Tail;
        private Transform Root;
        private SplineContainer Container;
        private Quaternion DefaultHeadRotation;
        private Quaternion DefaultTailRotation;

        private GameObject HeadGameObject;
        private ProBuilderMesh HeadMesh;

        private void Awake()
        {
            Root = GetComponentInParent<Snake>().transform;
            Container = GetComponent<SplineContainer>();
            DefaultHeadRotation = Head.localRotation;
            DefaultTailRotation = Tail.localRotation;
            Spline.Changed += HandleSplineChanged;
        }


        private void LateUpdate()
        {
            BezierKnot knot = Container.Spline[Container.Spline.Count - 1];
            Quaternion knotRotation = new(
                knot.Rotation.value.x,
                knot.Rotation.value.y,
                knot.Rotation.value.z,
                knot.Rotation.value.w
            );

            Head.transform.localRotation = Quaternion.Euler(
                knotRotation.eulerAngles - DefaultHeadRotation.eulerAngles
            );

            Head.transform.localPosition =
                new Vector3(knot.Position.x, knot.Position.y, knot.Position.z);
        }

        private void HandleSplineChanged(Spline spline, int index, SplineModification change)
        {
            if (spline != Container.Spline || index != spline.Count - 1)
            {
                return;
            }

            BezierKnot knot = spline[index];
            Quaternion knotRotation = new(
                knot.Rotation.value.x,
                knot.Rotation.value.y,
                knot.Rotation.value.z,
                knot.Rotation.value.w
            );

            Head.transform.localRotation = Quaternion.Euler(
                knotRotation.eulerAngles - DefaultHeadRotation.eulerAngles
            );

            Head.transform.localPosition =
                new Vector3(knot.Position.x, knot.Position.y, knot.Position.z);
        }

        private void Update()
        {
            Spline spline = Container.Spline;
            int knotCount = spline.Count;
            BezierKnot knot;

            for (int i = knotCount - 1; i > 0; i--)
            {
                float distanceDamping = (float)i / knotCount;
                knot = spline[i];
                knot.Position.x = Mathf.Sin(i * (Mathf.PI / 2) + Time.time * Speed) * distanceDamping * MaxWidthOffset;
                spline[i] = knot;
            }

            knot = spline[0];
            Quaternion knotRotation = new(
                knot.Rotation.value.x,
                knot.Rotation.value.y,
                knot.Rotation.value.z,
                knot.Rotation.value.w
            );

            Tail.transform.localRotation = Quaternion.Euler(
                knotRotation.eulerAngles + DefaultTailRotation.eulerAngles
            );

            Tail.transform.localPosition =
                new Vector3(knot.Position.x, knot.Position.y, knot.Position.z);
        }
    }
}