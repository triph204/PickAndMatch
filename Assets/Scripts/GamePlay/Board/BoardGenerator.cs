using UnityEngine;

namespace PickAndMatch.Gameplay.Board
{
    public class BoardGenerator
    {
        private readonly float cellSize;
        private readonly float spacing;

        public BoardGenerator(
            float cellSize,
            float spacing)
        {
            this.cellSize = cellSize;
            this.spacing = spacing;
        }

        public Vector3 GetPosition(
            int x,
            int y,
            int columns,
            int rows)
        {
            float step =
                cellSize + spacing;

            float width =
                (columns - 1) * step;

            float height =
                (rows - 1) * step;

            float startX =
                -width / 2f;

            float startY =
                -height / 2f;

            return new Vector3(
                startX + x * step,
                startY + y * step,
                0f
            );
        }
    }
}