using System.Collections;
using LlamAcademy.ChickenDefense.Behaviors;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace LlamAcademy.ChickenDefense.AI
{
    public class ChickenSpawner : MonoBehaviour
    {
        [SerializeField] private UnitSO Chicken;
        [SerializeField] private Egg EggPrefab;
        [SerializeField] [Range(5, 15)] private int InitialChickens = 10;
        [SerializeField] [Range(5, 60)] private float SpawnRate = 5f;
        [SerializeField] private NavMeshSurface ChickenSurface;
        [SerializeField] [Range(5, 20)] private int MaxChickens = 20;
        [SerializeField] [Range(5, 20)] private int InitialEggs = 5;
        private ObjectPool<Chicken> ChickenPool;

        private int AliveChickens;

        private EventBinding<UnitSpawnEvent> UnitSpawnEventBinding;
        private EventBinding<UnitDeathEvent> UnitDeathEventBinding;

        private void Awake()
        {
            ChickenPool = new ObjectPool<Chicken>(() => Instantiate(Chicken.Prefab).GetComponent<Chicken>());
            UnitSpawnEventBinding = new EventBinding<UnitSpawnEvent>(OnUnitSpawn);
            UnitDeathEventBinding = new EventBinding<UnitDeathEvent>(OnUnitDeath);
            Bus<UnitSpawnEvent>.Register(UnitSpawnEventBinding);
            Bus<UnitDeathEvent>.Register(UnitDeathEventBinding);
        }

        private void OnUnitSpawn(UnitSpawnEvent @event)
        {
            if (@event.Unit.Unit == Chicken)
            {
                AliveChickens++;
            }
        }

        private void OnUnitDeath(UnitDeathEvent @event)
        {
            if (@event.Unit.Unit == Chicken)
            {
                AliveChickens--;
            }
        }

        private void Start()
        {
            for (int i = 0; i < InitialChickens; i++)
            {
                SpawnChicken();
            }

            for (int i = 0; i < InitialEggs; i++)
            {
                SpawnEgg();
            }

            StartCoroutine(SpawnChickens());
        }

        private IEnumerator SpawnChickens()
        {
            WaitForSeconds wait = new(SpawnRate);
            while (enabled)
            {
                yield return wait;
                if (AliveChickens < MaxChickens)
                {
                    SpawnChicken();
                }
            }
        }

        private void SpawnChicken()
        {
            Chicken chicken = ChickenPool.Get();

            Vector3 position = new(
                ChickenSurface.center.x + Random.Range(-ChickenSurface.size.x / 2f, ChickenSurface.size.x / 2f),
                ChickenSurface.center.y,
                ChickenSurface.center.z + Random.Range(-ChickenSurface.size.z / 2f, ChickenSurface.size.z / 2f)
            );

            if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, chicken.Agent.areaMask))
            {
                chicken.transform.position = hit.position;
                chicken.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
            else
            {
                chicken.transform.position = transform.position;
            }
        }

        private void SpawnEgg()
        {
            Vector3 spawnLocation = new (
                Random.Range(ChickenCoop.CoopBounds.min.x, ChickenCoop.CoopBounds.max.x),
                ChickenCoop.CoopBounds.min.y,
                Random.Range(ChickenCoop.CoopBounds.min.z, ChickenCoop.CoopBounds.max.z)
            );

            Instantiate(EggPrefab, spawnLocation, Quaternion.identity);
        }
    }
}