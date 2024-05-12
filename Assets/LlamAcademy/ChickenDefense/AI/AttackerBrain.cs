using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;
using LlamAcademy.ChickenDefense.Units.Enemies;
using LlamAcademy.ChickenDefense.Units.Enemies.Fox.Behaviors;
using LlamAcademy.ChickenDefense.Units.Enemies.Snake.Behaviors;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace LlamAcademy.ChickenDefense.AI
{
    public class AttackerBrain : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface SnakeSurface;
        [SerializeField] private NavMeshSurface FoxSurface;
        [SerializeField] [Range(1, 5)] private int DifficultyFactor = 1;
        [SerializeField] private UnitSO Snake;
        [SerializeField] private UnitSO Fox;
        private int NumberAliveEggs;

        private List<Snake> AliveSnakes = new();
        private List<Fox> AliveFoxes = new();
        private List<Egg> AliveEggs = new();
        private List<Chicken> AliveChickens = new();

        private ObjectPool<Snake> SnakePool;
        private ObjectPool<Fox> FoxPool;

        [SerializeField]
        private DifficultyFactorScaling[] DifficultyScaling = new[]
        {
            new DifficultyFactorScaling(20, 35, 1.1f),
            new DifficultyFactorScaling(17, 30, 1.25f),
            new DifficultyFactorScaling(15, 25, 1.5f),
            new DifficultyFactorScaling(12, 20, 2),
            new DifficultyFactorScaling(12, 20, 3),
        };

        [Serializable]
        private struct DifficultyFactorScaling
        {
            public float SecondsBetweenSnakeSpawns;
            public float SecondsBetweenFoxSpawns;
            public float MultiplierPerSpawn;

            public DifficultyFactorScaling(float secondsBetweenSnakeSpawns, float secondsBetweenFoxSpawns, float multiplierPerSpawn)
            {
                SecondsBetweenFoxSpawns = secondsBetweenFoxSpawns;
                SecondsBetweenSnakeSpawns = secondsBetweenSnakeSpawns;
                MultiplierPerSpawn = multiplierPerSpawn;
            }
        }

        private void Awake()
        {
            SnakePool = new ObjectPool<Snake>(() => Instantiate(Snake.Prefab).GetComponent<Snake>());
            FoxPool = new ObjectPool<Fox>(() => Instantiate(Fox.Prefab).GetComponent<Fox>());

            Bus<EggRemovedEvent>.OnEvent += HandleLoseEgg;
            Bus<EggSpawnEvent>.OnEvent += HandleSpawnEgg;

            Bus<UnitSpawnEvent>.OnEvent += HandleUnitSpawn;
            Bus<UnitDeathEvent>.OnEvent += HandleUnitDeath;
            Bus<DifficultyChangedEvent>.OnEvent += HandleDifficultyChanged;

            gameObject.SetActive(false);
        }


        private void OnDestroy()
        {
            Bus<EggRemovedEvent>.OnEvent -= HandleLoseEgg;
            Bus<EggSpawnEvent>.OnEvent -= HandleSpawnEgg;

            Bus<UnitSpawnEvent>.OnEvent -= HandleUnitSpawn;
            Bus<UnitDeathEvent>.OnEvent -= HandleUnitDeath;
            Bus<DifficultyChangedEvent>.OnEvent -= HandleDifficultyChanged;
        }

        private void Start()
        {
            Bounds snakeSpawnBounds = SnakeSurface.navMeshData.sourceBounds;
            Bounds foxSpawnBounds = FoxSurface.navMeshData.sourceBounds;
            StartCoroutine(SpawnSnakes(snakeSpawnBounds));
            StartCoroutine(SpawnFoxes(foxSpawnBounds));
        }

        private IEnumerator SpawnSnakes(Bounds spawnBounds)
        {
            WaitForSeconds wait = new(DifficultyScaling[DifficultyFactor].SecondsBetweenSnakeSpawns);
            int iteration = 1;
            while (enabled)
            {
                foreach (Snake snake in SpawnEnemy(SnakePool, spawnBounds, iteration))
                {
                    if (NumberAliveEggs == 0)
                    {
                        snake.Wander();
                    }
                    else
                    {
                        snake.Follow(AliveEggs[Random.Range(0, AliveEggs.Count)].transform);
                    }
                }
                yield return wait;
                iteration++;
            }
        }
        private IEnumerator SpawnFoxes(Bounds spawnBounds)
        {
            WaitForSeconds wait = new(DifficultyScaling[DifficultyFactor].SecondsBetweenFoxSpawns);
            int iteration = 1;
            while (enabled)
            {
                yield return wait;
                foreach (Fox fox in SpawnEnemy(FoxPool, spawnBounds, iteration))
                {
                    if (AliveChickens.Count == 0)
                    {
                        fox.Wander();
                    }
                    else
                    {
                        fox.Follow(AliveChickens[Random.Range(0, AliveChickens.Count)].transform);
                    }
                }
                iteration++;
            }
        }

        private List<T> SpawnEnemy<T>(IObjectPool<T> pool, Bounds spawnBounds, int iteration) where T : EnemyBase
        {
            int spawns = iteration * Mathf.FloorToInt(DifficultyScaling[DifficultyFactor].MultiplierPerSpawn);
            List<T> enemies = new(spawns);
            for (int i = 0; i < spawns; i++)
            {
                T enemy = pool.Get();

                enemies.Add(enemy);
                Vector3 spawnLocation = PickSpawnLocationOnEdgeOfBounds(spawnBounds);
                NavMeshQueryFilter queryFilter = new()
                {
                    agentTypeID = enemy.Agent.agentTypeID,
                    areaMask = enemy.Agent.areaMask
                };
                if (NavMesh.SamplePosition(spawnLocation, out NavMeshHit hit, 1, queryFilter))
                {
                    enemy.transform.position = hit.position;
                    enemy.Agent.Warp(hit.position);
                }
                else
                {
                    Debug.LogError(
                        "Unable to find a suitable location on bounds of the NavMeshSurface. Scene is expected to have a rectangular NavMesh Surface for enemy spawns!");
                }
            }

            return enemies;
        }

        private static Vector3 PickSpawnLocationOnEdgeOfBounds(Bounds spawnBounds)
        {
            Vector3 spawnLocation = Random.value switch
            {
                < 0.25f => new Vector3(spawnBounds.min.x, 0, Random.Range(spawnBounds.min.z, spawnBounds.max.z)),
                < 0.5f => new Vector3(Random.Range(spawnBounds.min.x, spawnBounds.max.x), 0, spawnBounds.min.z),
                < 0.75f => new Vector3(spawnBounds.max.x, 0, Random.Range(spawnBounds.min.z, spawnBounds.max.z)),
                _ => new Vector3(Random.Range(spawnBounds.min.x, spawnBounds.max.x), 0, spawnBounds.max.z)
            };

            return spawnLocation;
        }

        private void HandleUnitDeath(UnitDeathEvent @event)
        {
            switch (@event.Unit)
            {
                case Snake snake:
                    AliveSnakes.Remove(snake);
                    break;
                case Fox fox:
                    AliveFoxes.Remove(fox);
                    break;
                case Chicken chicken:
                    AliveChickens.Remove(chicken);

                    foreach (Fox fox in AliveFoxes)
                    {
                        if (AliveChickens.Count == 0)
                        {
                            fox.MoveTo(PickSpawnLocationOnEdgeOfBounds(FoxSurface.navMeshData.sourceBounds));
                        }
                        else if (fox.TransformTarget == null || @event.Unit.transform == fox.TransformTarget)
                        {
                            fox.Follow(AliveChickens[Random.Range(0, AliveChickens.Count)].transform);
                        }
                    }
                    break;
            }
        }

        private void HandleDifficultyChanged(DifficultyChangedEvent evt)
        {
            DifficultyFactor = (int)evt.Difficulty;
        }

        private void HandleUnitSpawn(UnitSpawnEvent @event)
        {
            switch (@event.Unit)
            {
                case Snake snake:
                    AliveSnakes.Add(snake);
                    break;
                case Fox fox:
                    AliveFoxes.Add(fox);
                    break;
                case Chicken chicken:
                    AliveChickens.Add(chicken);

                    foreach (Fox fox in AliveFoxes.Where(fox => fox.TransformTarget == null))
                    {
                        fox.Follow(AliveChickens[Random.Range(0, AliveChickens.Count)].transform);
                    }

                    break;
            }
        }

        private void HandleLoseEgg(EggRemovedEvent @event)
        {
            NumberAliveEggs -= @event.Eggs.Length;

            foreach (Egg egg in @event.Eggs)
            {
                AliveEggs.Remove(egg);
            }

            foreach (Snake snake in AliveSnakes)
            {
                if (NumberAliveEggs == 0)
                {
                    snake.Wander();
                }
                else if (snake.TransformTarget != null && @event.Eggs.Contains(snake.TransformTarget.GetComponent<Egg>()))
                {
                    snake.Follow(AliveEggs[Random.Range(0, AliveEggs.Count)].transform);
                }
            }
        }

        private void HandleSpawnEgg(EggSpawnEvent @event)
        {
            if (NumberAliveEggs == 0)
            {
                foreach (Snake snake in AliveSnakes)
                {
                    snake.Follow(@event.Egg.transform);
                }
            }
            else
            {
                float currentPathDistance = 0;
                float pathToNewEggDistance = 0;

                // Challenge: Convert to jobs system for better performance!
                foreach (Snake snake in AliveSnakes)
                {
                    NavMeshPath path = new();
                    NavMesh.CalculatePath(snake.transform.position, @event.Egg.transform.position, snake.Agent.areaMask,
                        path);
                    pathToNewEggDistance = SumSquareDistances(path);

                    NavMesh.CalculatePath(snake.transform.position, snake.Target, snake.Agent.areaMask, path);
                    currentPathDistance = SumSquareDistances(path);

                    if (pathToNewEggDistance < currentPathDistance)
                    {
                        snake.Follow(@event.Egg.transform);
                    }
                }
            }

            NumberAliveEggs++;
            AliveEggs.Add(@event.Egg);
        }

        private static float SumSquareDistances(NavMeshPath path)
        {
            float squareDistance = 0;
            Vector3[] corners = path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                squareDistance += (corners[i] - corners[i + 1]).sqrMagnitude;
            }

            return squareDistance;
        }
    }
}
