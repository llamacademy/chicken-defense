using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;

namespace LlamAcademy.ChickenDefense.Units
{
    public enum OffMeshLinkMoveMethod
    {
        Teleport,
        NormalSpeed,
        Parabola,
        Curve
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentLinkMover : MonoBehaviour
    {
        public OffMeshLinkMoveMethod DefaultMoveMethod = OffMeshLinkMoveMethod.Parabola;
        public AnimationCurve Curve = new();
        public float ParabolaHeight = 8f;

        public delegate void LinkEvent(OffMeshLinkMoveMethod MoveMethod);

        public LinkEvent OnLinkStart;
        public LinkEvent OnLinkEnd;
        public List<LinkTraversalConfiguration> NavMeshLinkTraversalTypes = new();

        IEnumerator Start()
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.autoTraverseOffMeshLink = false;
            while (true)
            {
                if (agent.isOnOffMeshLink)
                {
                    OffMeshLinkData offMeshLinkData = agent.currentOffMeshLinkData;
                    NavMeshLink link = (NavMeshLink)agent.navMeshOwner;
                    int AreaType = link.area;

                    if (Vector3.Distance(offMeshLinkData.endPos, agent.destination) <
                        Vector3.Distance(offMeshLinkData.startPos, agent.destination))
                    {
                        LinkTraversalConfiguration configuration =
                            NavMeshLinkTraversalTypes.Find((type) => type.AreaType == AreaType);

                        if ((configuration != null && configuration.MoveMethod == OffMeshLinkMoveMethod.NormalSpeed) ||
                            (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.NormalSpeed))
                        {
                            OnLinkStart?.Invoke(OffMeshLinkMoveMethod.NormalSpeed);
                            yield return StartCoroutine(MoveAtNormalSpeed(agent));
                            OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.NormalSpeed);
                        }
                        else if ((configuration != null &&
                                  configuration.MoveMethod == OffMeshLinkMoveMethod.Parabola) ||
                                 (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Parabola))
                        {
                            OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Parabola);
                            yield return StartCoroutine(MoveParabola(agent, ParabolaHeight,
                                Vector3.Distance(offMeshLinkData.startPos, offMeshLinkData.endPos) / agent.speed));
                            OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Parabola);
                        }
                        else if ((configuration != null && configuration.MoveMethod == OffMeshLinkMoveMethod.Curve) ||
                                 (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Curve))
                        {
                            OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Curve);
                            yield return StartCoroutine(MoveCurve(agent,
                                Vector3.Distance(offMeshLinkData.startPos, offMeshLinkData.endPos) / agent.speed));
                            OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Curve);
                        }
                        else if ((configuration != null &&
                                  configuration.MoveMethod == OffMeshLinkMoveMethod.Teleport) ||
                                 (configuration == null && DefaultMoveMethod == OffMeshLinkMoveMethod.Teleport))
                        {
                            OnLinkStart?.Invoke(OffMeshLinkMoveMethod.Teleport);
                            OnLinkEnd?.Invoke(OffMeshLinkMoveMethod.Teleport);
                        }
                    }

                    agent.CompleteOffMeshLink();
                }

                yield return null;
            }
        }

        IEnumerator MoveAtNormalSpeed(NavMeshAgent agent)
        {
            OffMeshLinkData data = agent.currentOffMeshLinkData;
            Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
            while (agent.transform.position != endPos)
            {
                agent.transform.position =
                    Vector3.MoveTowards(agent.transform.position, endPos, agent.speed * Time.deltaTime);
                yield return null;
            }
        }

        IEnumerator MoveParabola(NavMeshAgent agent, float height, float duration)
        {
            OffMeshLinkData data = agent.currentOffMeshLinkData;
            Vector3 startPos = agent.transform.position;
            Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
            float normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                float yOffset = height * (normalizedTime - normalizedTime * normalizedTime);
                agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

        IEnumerator MoveCurve(NavMeshAgent agent, float duration)
        {
            OffMeshLinkData data = agent.currentOffMeshLinkData;
            Vector3 startPos = agent.transform.position;
            Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;
            float normalizedTime = 0.0f;
            while (normalizedTime < 1.0f)
            {
                float yOffset = Curve.Evaluate(normalizedTime);
                agent.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * Vector3.up;
                normalizedTime += Time.deltaTime / duration;
                yield return null;
            }
        }

        [Serializable]
        public class LinkTraversalConfiguration
        {
            public OffMeshLinkMoveMethod MoveMethod;
            public int AreaType;
        }
    }
}