using LlamAcademy.ChickenDefense.Behaviors;
using LlamAcademy.ChickenDefense.Units.Chicken.FSM;
using LlamAcademy.ChickenDefense.Units.FSM.Common;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using UnityHFSM;
using Random = UnityEngine.Random;

namespace LlamAcademy.ChickenDefense.Units.Chicken.Behaviors
{
    public class Chicken : UnitBase<ChickenStates>
    {
        [SerializeField] private Egg EggPrefab;
        [SerializeField] private Vector2 HungerRegenerationRange = new(1f, 3);
        [SerializeField] public float Hunger;
        [SerializeField] private Vector2 LayEggRange = new(5, 7);
        [SerializeField] private Vector2 MaxHungerRange = new(7, 12);
        private float MaxHunger;
        private float LayEggDelay;
        private float LastEggTime;

        private static ObjectPool<Egg> EggPool;
        public float HungerRegenerationRate { get; private set; }

        protected override void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            Animator = GetComponent<Animator>();
            HungerRegenerationRate = Random.Range(HungerRegenerationRange.x, HungerRegenerationRange.y);
            LayEggDelay = Random.Range(LayEggRange.x, LayEggRange.y);
            MaxHunger = Random.Range(MaxHungerRange.x, MaxHungerRange.y);
            PickTargetLocation();
            base.Awake();
        }

        protected override void Update()
        {
            if (FSM.ActiveStateName != ChickenStates.Eat)
            {
                Hunger += Time.deltaTime;
            }

            base.Update();
        }

        protected override void AddStates()
        {
            FSM.AddState(ChickenStates.Move, new MoveState<ChickenStates>(this));
            FSM.AddState(ChickenStates.Eat, new EatState(this));
            FSM.AddState(ChickenStates.LayEgg, new LayEggState(this));
        }

        protected override void AddTransitions()
        {
            FSM.AddTransition(new Transition<ChickenStates>(ChickenStates.Eat, ChickenStates.Move, IsFull,
                (_) => PickTargetLocation()));
            FSM.AddTransition(new TransitionAfter<ChickenStates>(ChickenStates.LayEgg, ChickenStates.Move, 2f, null,
                (_) => SpawnEgg()));
            FSM.AddTransition(new Transition<ChickenStates>(ChickenStates.Move, ChickenStates.LayEgg, CanLayEgg, null,
                (_) => LastEggTime = Time.time));
            FSM.AddTransition(new TransitionAfter<ChickenStates>(ChickenStates.Move, ChickenStates.Move, 3f,
                IsCloseToTarget, (_) => PickTargetLocation()));
            FSM.AddTransition(new Transition<ChickenStates>(ChickenStates.Move, ChickenStates.Eat, CanEat));
        }

        private bool IsFull(Transition<ChickenStates> _) => Hunger <= 0;
        private bool IsHungry(Transition<ChickenStates> _) => Hunger > MaxHunger;
        private bool CanEat(Transition<ChickenStates> _) => !IsInChickenCoop() && IsHungry(_);

        private bool CanLayEgg(Transition<ChickenStates> _) =>
            LastEggTime + LayEggDelay <= Time.time && IsInChickenCoop() && IsCloseToTarget(null);

        private bool IsCloseToTarget(TransitionAfter<ChickenStates> _) =>
            Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance;

        private bool IsInChickenCoop() => ChickenCoop.CoopBounds.Intersects(transform.GetComponent<Collider>().bounds);

        private void SpawnEgg()
        {
            EggPool ??= new ObjectPool<Egg>(() => Instantiate(EggPrefab));
            Egg egg = EggPool.Get();
            egg.transform.position = transform.position;
            Hunger = -5;
            Vector2 offset = Random.onUnitSphere * 5f;
            if (NavMesh.SamplePosition(
                    transform.position + new Vector3(
                        Random.Range(-offset.x, offset.x),
                        -2,
                        Random.Range(-offset.y, offset.y)),
                    out NavMeshHit hit, 2f, Agent.areaMask
                ))
            {
                Target = hit.position;
            }
            else
            {
                PickTargetLocation();
            }
        }

        private void PickTargetLocation()
        {
            if (LastEggTime + LayEggDelay <= Time.time)
            {
                if (NavMesh.SamplePosition(
                        new Vector3(
                            Random.Range(ChickenCoop.CoopBounds.min.x, ChickenCoop.CoopBounds.max.x),
                            ChickenCoop.CoopBounds.min.y,
                            Random.Range(ChickenCoop.CoopBounds.min.z, ChickenCoop.CoopBounds.max.z)
                        ),
                        out NavMeshHit hit, 0.125f, Agent.areaMask
                    ))
                {
                    Target = hit.position;
                }
                else
                {
                    PickTargetLocation();
                }
            }
            else
            {
                Vector2 offset = Random.insideUnitCircle * 2f;
                if (NavMesh.SamplePosition(
                        transform.position + new Vector3(
                            Random.Range(-offset.x, offset.x),
                            0,
                            Random.Range(-offset.y, offset.y)),
                        out NavMeshHit hit, 1f, Agent.areaMask
                    ))
                {
                    Target = hit.position;
                }
                else
                {
                    PickTargetLocation();
                }
            }
        }
    }
}