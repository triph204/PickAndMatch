using System.Collections;
using System.Collections.Generic;
using PickAndMatch.Core;
using PickAndMatch.Gameplay.Animal;
using PickAndMatch.Gameplay.Score;
using UnityEngine;

using AnimalDataModel = PickAndMatch.Gameplay.Animal.AnimalData;
using AnimalModel = PickAndMatch.Gameplay.Animal.Animal;

namespace PickAndMatch.Gameplay.Board
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Level Data")]
        [SerializeField] private BoardData boardData;

        [Header("Board")]
        [SerializeField] private Transform animalParent;
        [SerializeField] private float maxCellSize = 1.5f;
        [SerializeField] private float minCellSize = 0.4f;
        [SerializeField] private float spacing = 0.1f;
        [SerializeField] private float screenPadding = 0.5f;

        [Header("Prefab")]
        [SerializeField] private AnimalModel animalPrefab;

        [Header("Card")]
        [SerializeField] private Sprite cardBackSprite;
        [SerializeField] private Sprite cardFrontSprite;

        [Header("Match")]
        [SerializeField] private float mismatchDelay = 0.6f;

        [Header("Score")]
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private int scorePerMatch = 10;
        [SerializeField] private int comboBonus = 5;

        private AnimalFactory animalFactory;
        private BoardGenerator boardGenerator;

        private readonly List<AnimalModel> spawnedAnimals =
            new List<AnimalModel>();

        private AnimalModel firstPick;
        private AnimalModel secondPick;

        private bool isBusy;
        private bool inputEnabled = true;
        private int comboCount;

        public IReadOnlyList<AnimalModel> SpawnedAnimals =>
            spawnedAnimals;

        public float TimeLimit =>
            boardData != null ? boardData.TimeLimit : 0f;
        public int LevelNumber =>
            boardData != null ? boardData.LevelNumber : 0;

        public int ComboCount => comboCount;

        private void Awake()
        {
            if (animalParent == null)
                animalParent = transform;

            animalFactory = new AnimalFactory(animalParent);
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }

        public void GenerateBoard()
        {
            if (boardData == null)
            {
                Debug.LogError("BoardData is missing.");
                return;
            }

            if (animalPrefab == null)
            {
                Debug.LogError("Animal Prefab is missing.");
                return;
            }

            ClearBoard();

            inputEnabled = true;
            comboCount = 0;

            if (scoreManager != null)
                scoreManager.ResetScore();

            float cellSize = CalculateCellSize();

            boardGenerator = new BoardGenerator(
                cellSize,
                spacing
            );

            int totalCells =
                boardData.Rows *
                boardData.Columns;

            List<AnimalDataModel> deck =
                BuildDeck(totalCells);

            if (deck == null)
                return;

            int index = 0;

            for (int y = 0; y < boardData.Rows; y++)
            {
                for (int x = 0; x < boardData.Columns; x++)
                {
                    SpawnAnimal(
                        x,
                        y,
                        deck[index],
                        cellSize
                    );

                    index++;
                }
            }
        }

        private float CalculateCellSize()
        {
            Camera cam = Camera.main;

            if (cam == null)
            {
                Debug.LogWarning(
                    "Main Camera not found."
                );

                return maxCellSize;
            }

            if (!cam.orthographic)
            {
                Debug.LogWarning(
                    "Camera must be Orthographic."
                );

                return maxCellSize;
            }

            float screenHeight =
                cam.orthographicSize * 2f;

            float screenWidth =
                screenHeight * cam.aspect;

            float availableWidth =
                screenWidth -
                screenPadding * 2f;

            float availableHeight =
                screenHeight -
                screenPadding * 2f;

            float horizontalSpacing =
                spacing *
                (boardData.Columns - 1);

            float verticalSpacing =
                spacing *
                (boardData.Rows - 1);

            float sizeByWidth =
                (availableWidth -
                 horizontalSpacing) /
                boardData.Columns;

            float sizeByHeight =
                (availableHeight -
                 verticalSpacing) /
                boardData.Rows;

            float size =
                Mathf.Min(
                    sizeByWidth,
                    sizeByHeight
                );

            return Mathf.Clamp(
                size,
                minCellSize,
                maxCellSize
            );
        }

        private List<AnimalDataModel> BuildDeck(
            int totalCells)
        {
            if (boardData.Animals == null ||
                boardData.Animals.Count == 0)
            {
                Debug.LogError(
                    "No AnimalData configured."
                );

                return null;
            }

            if (totalCells % 2 != 0)
            {
                Debug.LogError(
                    "Rows * Columns must be even."
                );

                return null;
            }

            int pairsNeeded =
                totalCells / 2;

            if (boardData.Animals.Count <
                pairsNeeded)
            {
                Debug.LogError(
                    $"Need {pairsNeeded} animal types, " +
                    $"but only {boardData.Animals.Count} available."
                );

                return null;
            }

            List<AnimalDataModel> deck =
                new List<AnimalDataModel>(
                    totalCells
                );

            for (int i = 0;
                i < pairsNeeded;
                i++)
            {
                deck.Add(
                    boardData.Animals[i]
                );

                deck.Add(
                    boardData.Animals[i]
                );
            }

            Shuffle(deck);

            return deck;
        }

        private void Shuffle(
            List<AnimalDataModel> list)
        {
            for (int i = list.Count - 1;
                i > 0;
                i--)
            {
                int j =
                    Random.Range(
                        0,
                        i + 1
                    );

                (list[i], list[j]) =
                    (list[j], list[i]);
            }
        }

        private void SpawnAnimal(
            int x,
            int y,
            AnimalDataModel data,
            float cellSize)
        {
            Vector3 position =
                boardGenerator.GetPosition(
                    x,
                    y,
                    boardData.Columns,
                    boardData.Rows
                );

            AnimalModel animal =
                animalFactory.Create(
                    animalPrefab,
                    data,
                    position,
                    cardBackSprite,
                    cardFrontSprite
                );

            if (animal == null)
                return;

            float scale =
                cellSize / maxCellSize;

            animal.SetCardScale(scale);

            animal.OnClicked +=
                HandleAnimalClicked;

            spawnedAnimals.Add(animal);
        }

        private void HandleAnimalClicked(
            AnimalModel animal)
        {
            if (!inputEnabled)
                return;

            if (isBusy)
                return;

            if (animal == firstPick)
                return;

            animal.Reveal();

            if (firstPick == null)
            {
                firstPick = animal;
                return;
            }

            secondPick = animal;
            isBusy = true;

            StartCoroutine(
                CheckMatchRoutine()
            );
        }

        private IEnumerator CheckMatchRoutine()
        {
            yield return new WaitForSeconds(
                mismatchDelay
            );

            if (firstPick != null &&
                secondPick != null)
            {
                if (firstPick.Type ==
                    secondPick.Type)
                {
                    HandleMatch(
                        firstPick,
                        secondPick
                    );
                }
                else
                {
                    HandleMismatch(
                        firstPick,
                        secondPick
                    );
                }
            }

            firstPick = null;
            secondPick = null;
            isBusy = false;

            CheckWinCondition();
        }

        private void HandleMatch(
            AnimalModel a,
            AnimalModel b)
        {
            a.OnClicked -=
                HandleAnimalClicked;

            b.OnClicked -=
                HandleAnimalClicked;

            spawnedAnimals.Remove(a);
            spawnedAnimals.Remove(b);

            a.Disappear();
            b.Disappear();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySound(
                    AudioManager.Instance.conf
                );
            }

            comboCount++;

            if (scoreManager != null)
            {
                int bonus =
                    comboBonus *
                    (comboCount - 1);

                scoreManager.AddScore(
                    scorePerMatch + bonus
                );
            }
        }

        private void HandleMismatch(
            AnimalModel a,
            AnimalModel b)
        {
            a.Hide();
            b.Hide();

            comboCount = 0;
        }

        private void CheckWinCondition()
        {
            if (spawnedAnimals.Count == 0)
            {
                GameManager.Instance?.WinGame();
            }
        }

        public void ClearBoard()
        {
            firstPick = null;
            secondPick = null;
            isBusy = false;
            comboCount = 0;

            foreach (AnimalModel animal in spawnedAnimals)
            {
                if (animal != null)
                {
                    animal.OnClicked -=
                        HandleAnimalClicked;

                    Destroy(
                        animal.gameObject
                    );
                }
            }

            spawnedAnimals.Clear();
        }
    }
}