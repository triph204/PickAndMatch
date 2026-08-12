using PickAndMatch.Gameplay.Entities;
using UnityEngine;

namespace PickAndMatch.Gameplay.Animal
{
    [CreateAssetMenu(
        fileName = "AnimalData",
        menuName = "Pick And Match/Animal Data")]
    public class AnimalData : ScriptableObject
    {
        [Header("Animal")]
        [SerializeField]
        private AnimalType type;

        [Header("Visual")]
        [SerializeField]
        private Sprite sprite;

        [Header("Prefab")]
        [SerializeField]
        private Animal prefab;

        public AnimalType Type => type;

        public Sprite Sprite => sprite;

        public Animal Prefab => prefab;
    }
}