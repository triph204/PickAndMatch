using System.Collections;
using System.Collections.Generic;
using PickAndMatch.Core;
using PickAndMatch.Gameplay.Animal;
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
        [SerializeField] private float cellSize = 1.5f;

        [Header("Prefab")]
        [Tooltip("1 prefab Animal DUY NHẤT dùng chung cho mọi loại con vật.")]
        [SerializeField] private AnimalModel animalPrefab;

        [Header("Card")]
        [SerializeField] private Sprite cardBackSprite;
        [SerializeField] private Sprite cardFrontSprite;

        [Header("Match")]
        [SerializeField] private float mismatchDelay = 0.6f;

        private AnimalFactory animalFactory;
        private BoardGenerator boardGenerator;

        private readonly List<AnimalModel> spawnedAnimals =
            new List<AnimalModel>();

        private AnimalModel firstPick;
        private AnimalModel secondPick;
        private bool isBusy;

        public IReadOnlyList<AnimalModel> SpawnedAnimals =>
            spawnedAnimals;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (animalParent == null)
            {
                animalParent = transform;
            }

            animalFactory =
                new AnimalFactory(animalParent);

            boardGenerator =
                new BoardGenerator(cellSize);
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
                Debug.LogError("Animal Prefab is missing on BoardManager.");
                return;
            }

            ClearBoard();

            int totalCells = boardData.Rows * boardData.Columns;

            List<AnimalDataModel> deck = BuildDeck(totalCells);

            if (deck == null)
            {
                return;
            }

            int index = 0;

            for (int y = 0; y < boardData.Rows; y++)
            {
                for (int x = 0; x < boardData.Columns; x++)
                {
                    SpawnAnimal(x, y, deck[index]);
                    index++;
                }
            }
        }

        private List<AnimalDataModel> BuildDeck(int totalCells)
        {
            if (boardData.Animals == null ||
                boardData.Animals.Count == 0)
            {
                Debug.LogError("No AnimalData configured.");
                return null;
            }

            if (totalCells % 2 != 0)
            {
                Debug.LogError(
                    "Rows * Columns must be even to form pairs.");
                return null;
            }

            int pairsNeeded = totalCells / 2;

            if (boardData.Animals.Count < pairsNeeded)
            {
                Debug.LogError(
                    $"Not enough AnimalData types. Need {pairsNeeded}, has {boardData.Animals.Count}.");
                return null;
            }

            List<AnimalDataModel> deck =
                new List<AnimalDataModel>(totalCells);

            for (int i = 0; i < pairsNeeded; i++)
            {
                deck.Add(boardData.Animals[i]);
                deck.Add(boardData.Animals[i]);
            }

            Shuffle(deck);

            return deck;
        }

        private void Shuffle(List<AnimalDataModel> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);

                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private void SpawnAnimal(int x, int y, AnimalDataModel data)
        {
            Vector3 position =
                boardGenerator.GetPosition(
                    x,
                    y,
                    boardData.Columns,
                    boardData.Rows);

            AnimalModel animal =
                animalFactory.Create(
                    animalPrefab,
                    data,
                    position,
                    cardBackSprite,
                    cardFrontSprite);

            if (animal != null)
            {
                animal.OnClicked += HandleAnimalClicked;
                spawnedAnimals.Add(animal);
            }
        }

        private void HandleAnimalClicked(AnimalModel animal)
        {
            if (isBusy)
                return;

            if (animal == firstPick)
                return;

            // Lật thẻ lên (có animation), giữ nguyên chờ thẻ 2.
            animal.Reveal();

            if (firstPick == null)
            {
                firstPick = animal;
                return;
            }

            secondPick = animal;
            isBusy = true;

            StartCoroutine(CheckMatchRoutine());
        }

        private IEnumerator CheckMatchRoutine()
        {
            yield return new WaitForSeconds(mismatchDelay);

            if (firstPick != null && secondPick != null)
            {
                if (firstPick.Type == secondPick.Type)
                {
                    HandleMatch(firstPick, secondPick);
                }
                else
                {
                    HandleMismatch(firstPick, secondPick);
                }
            }

            firstPick = null;
            secondPick = null;
            isBusy = false;

            CheckWinCondition();
        }

        private void HandleMatch(AnimalModel a, AnimalModel b)
        {
            // Đúng cặp -> gỡ khỏi danh sách/ sự kiện ngay,
            // rồi chạy animation biến mất (Animal tự Destroy sau khi xong).
            a.OnClicked -= HandleAnimalClicked;
            b.OnClicked -= HandleAnimalClicked;

            spawnedAnimals.Remove(a);
            spawnedAnimals.Remove(b);

            a.Disappear();
            b.Disappear();
        }

        private void HandleMismatch(AnimalModel a, AnimalModel b)
        {
            // Sai -> chạy animation úp lại (có animation lật).
            a.Hide();
            b.Hide();
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

            foreach (AnimalModel animal in spawnedAnimals)
            {
                if (animal != null)
                {
                    animal.OnClicked -= HandleAnimalClicked;
                    Destroy(animal.gameObject);
                }
            }

            spawnedAnimals.Clear();
        }
    }
}