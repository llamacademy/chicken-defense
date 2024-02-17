using System.Collections;
using System.Collections.Generic;
using LlamAcademy.ChickenDefense.EventBus;
using LlamAcademy.ChickenDefense.Events;
using LlamAcademy.ChickenDefense.Units;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;
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
        [SerializeField] [Range(1, 5)] private int DifficultyFactor = 2;
        [SerializeField] private UnitSO Snake;
        [SerializeField] private UnitSO Fox;
        private int NumberAliveEggs;

        private EventBinding<EggRemovedEvent> EggRemovedBinding;
        private EventBinding<EggSpawnEvent> EggSpawnedBinding;
        private EventBinding<UnitSpawnEvent> SpawnedUnitBinding;
        private EventBinding<UnitDeathEvent> UnitDeathBinding;

        private List<Snake> AliveSnakes = new();
        private List<Egg> AliveEggs = new();
        private ObjectPool<Snake> SnakePool;

        private void Awake()
        {
            EggRemovedBinding = new EventBinding<EggRemovedEvent>(HandleLoseEgg);
            EggSpawnedBinding = new EventBinding<EggSpawnEvent>(HandleSpawnEgg);
            Bus<EggRemovedEvent>.Register(EggRemovedBinding);
            Bus<EggSpawnEvent>.Register(EggSpawnedBinding);

            SpawnedUnitBinding = new EventBinding<UnitSpawnEvent>(HandleUnitSpawn);
            UnitDeathBinding = new EventBinding<UnitDeathEvent>(HandleUnitDeath);
            Bus<UnitSpawnEvent>.Register(SpawnedUnitBinding);
            Bus<UnitDeathEvent>.Register(UnitDeathBinding);

            SnakePool = new ObjectPool<Snake>(() => Instantiate(Snake.Prefab).GetComponent<Snake>());
        }

        private void Start()
        {
            Bounds spawnableBounds = SnakeSurface.navMeshData.sourceBounds;
            StartCoroutine(SpawnEnemies(spawnableBounds));
        }

        private IEnumerator SpawnEnemies(Bounds spawnBounds)
        {
            WaitForSeconds snakeBurstSpawnWait = new WaitForSeconds(25f);
            int iteration = 1;
            while (enabled)
            {
                // TODO: maybe more complex spawning logic
                SpawnSnakes(spawnBounds, iteration);
                yield return snakeBurstSpawnWait;
            }
        }

        private void SpawnSnakes(Bounds spawnBounds, int iteration)
        {
            for (int i = 0; i < iteration; i++)
            {
                Snake snake = SnakePool.Get();
                
                Vector3 spawnLocation;
                if (Random.value < 0.25f)
                {
                    // pick location on min x, random z 
                    spawnLocation = new Vector3(
                        spawnBounds.min.x,
                        0,
                        Random.Range(spawnBounds.min.z, spawnBounds.max.z)
                    );
                }
                else if (Random.value < 0.5f)
                {
                    // pick location on min z, random x
                    spawnLocation = new Vector3(
                        Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                        0,
                        spawnBounds.min.z
                    );
                }
                else if (Random.value < 0.75f)
                {
                    // pick location on max x, random z
                    spawnLocation = new Vector3(
                        spawnBounds.max.x,
                        0,
                        Random.Range(spawnBounds.min.z, spawnBounds.max.z)
                    );
                }
                else
                {
                    // pick location on max z, random x
                    spawnLocation = new Vector3(
                        Random.Range(spawnBounds.min.x, spawnBounds.max.x),
                        0,
                        spawnBounds.max.z
                    );
                }

                if (NavMesh.SamplePosition(spawnLocation, out NavMeshHit hit, 1, snake.Agent.areaMask))
                {
                    snake.transform.position = hit.position;
                    snake.Agent.Warp(hit.position);
                    if (NumberAliveEggs == 0)
                    {
                        snake.Wander();
                    }
                    else
                    {
                        snake.GoToEgg(AliveEggs[Random.Range(0, AliveEggs.Count)]);
                    }
                }
                else
                {
                    Debug.LogError("Unable to find a suitable location on bounds of the NavMeshSurface. Scene is expected to have a rectangular NavMesh Surface for enemy spawns!");
                }
            }
        }

        private void HandleUnitDeath(UnitDeathEvent @event)
        {
            if (@event.Unit is Snake snake)
            {
                AliveSnakes.Remove(snake);
            }
        }

        private void HandleUnitSpawn(UnitSpawnEvent @event)
        {
            if (@event.Unit is Snake snake)
            {
                AliveSnakes.Add(snake);
            }
        }

        private void HandleLoseEgg(EggRemovedEvent @event)
        {
            NumberAliveEggs -= @event.Eggs.Length;
            
            foreach (Egg egg in @event.Eggs)
            {
                AliveEggs.Remove(egg);
            }

            if (NumberAliveEggs == 0)
            {
                foreach (Snake snake in AliveSnakes)
                {
                    snake.Wander();
                }
            }
        }

        private void HandleSpawnEgg(EggSpawnEvent @event)
        {
            if (NumberAliveEggs == 0)
            {
                foreach (Snake snake in AliveSnakes)
                {
                    snake.GoToEgg(@event.Egg);
                }
            }
            else
            {
                float currentPathDistance = 0;
                float pathToNewEggDistance = 0;

                // great jobs system opportunity here
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
                        snake.GoToEgg(@event.Egg);
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