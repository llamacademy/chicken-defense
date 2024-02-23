using LlamAcademy.ChickenDefense.AI;
using LlamAcademy.ChickenDefense.Units.Chicken.Behaviors;
using UnityEngine;

namespace LlamAcademy.ChickenDefense.UI
{
    public class MainMenuChickenSpawner : MonoBehaviour
    {
        [SerializeField] private ChickenSpawner ChickenSpawner;

        private void Awake()
        {
            for (int i = 0; i < ChickenSpawner.InitialChickens; i++)
            {
                ChickenSpawner.SpawnChicken();
            }
        }
    }
}
