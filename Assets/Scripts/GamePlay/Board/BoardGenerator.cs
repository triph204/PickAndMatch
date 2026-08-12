using UnityEngine;

namespace PickAndMatch.Gameplay.Board
{
    public class BoardGenerator
    {
        private readonly float cellSize;

        public BoardGenerator(float cellSize = 1.5f)
        {
            this.cellSize = cellSize;
        }

        public Vector3 GetPosition(
            int x,
            int y,
            int width,
            int height)
        {
            float offsetX =
                (width - 1) * cellSize * 0.5f;

            float offsetY =
                (height - 1) * cellSize * 0.5f;

            return new Vector3(
                x * cellSize - offsetX,
                y * cellSize - offsetY,
                0f);
        }
    }
}