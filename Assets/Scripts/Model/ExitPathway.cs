using UnityEngine;

namespace TestGame.Gameplay.Model
{
    public struct ExitPathway
    {
        public readonly Vector2Int ExitPos;
        public readonly Vector2Int AdjacentPos;

        public ExitPathway(Vector2Int exitPos, Vector2Int adjacentPos)
        {
            ExitPos = exitPos;
            AdjacentPos = adjacentPos;
        }
    }
}