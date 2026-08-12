using System.Collections.Generic;
using UnityEngine;
using PickAndMatch.Gameplay.Animal;

namespace PickAndMatch.Gameplay.Board
{
    [CreateAssetMenu(
        fileName = "BoardData",
        menuName = "Pick And Match/Board Data")]
    public class BoardData : ScriptableObject
    {
        [Header("Level")]
        [SerializeField] private int levelNumber;

        [Header("Board")]
        [SerializeField] private int rows;
        [SerializeField] private int columns;

        [Header("Cards")]
        [SerializeField] private List<AnimalData> animals;

        [Header("Game")]
        [SerializeField] private float timeLimit = 60f;

        public int LevelNumber => levelNumber;

        public int Rows => rows;

        public int Columns => columns;

        public IReadOnlyList<AnimalData> Animals =>
            animals;

        public float TimeLimit => timeLimit;
    }
}