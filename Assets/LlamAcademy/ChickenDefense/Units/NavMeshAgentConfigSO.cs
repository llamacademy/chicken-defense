using UnityEngine;
using UnityEngine.AI;

namespace LlamAcademy.ChickenDefense.Units
{
    [CreateAssetMenu(menuName="Units/NavMeshAgent Config", order = 0, fileName = "NavMeshAgent Config")]
    public class NavMeshAgentConfigSO : ScriptableObject
    {
        // How to do AgentTypeId editor? 🤷‍♂️
        public int AgentTypeId = 0; 
        public float BaseOffset = 0;
        
        [Header("Steering")]
        public float Speed = 3;
        public float AngularSpeed = 120;
        public float Acceleration = 8;
        public float StoppingDistance = 0.5f;
        
        [Header("Obstacle Avoidance")]
        public float Radius = 1;
        public float Height = 2;
        public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        public int AvoidancePriority = 50;

        public void SetupNavMeshAgent(NavMeshAgent agent)
        {
            agent.speed = Speed;
            agent.angularSpeed = AngularSpeed;
            agent.acceleration = Acceleration;
            agent.radius = Radius;
            agent.height = Height;
            agent.avoidancePriority = AvoidancePriority;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType;
            agent.stoppingDistance = StoppingDistance;
            agent.agentTypeID = AgentTypeId;
            agent.baseOffset = BaseOffset;
        }
    }
}