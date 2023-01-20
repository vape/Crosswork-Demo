using Crosswork.Core;
using Crosswork.Demo.Merge.Elements;
using Crosswork.View;
using UnityEngine;

namespace Crosswork.Demo.Merge.Systems
{
    public class GeneratorSystem : DemoBoardSystem
    {
        public const int MaxGeneratedElementLevel = 2;

        private CrossworkBoard board;

        private Vector2Int[] availablePositions;
        private int availablePositionsCount;

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;

            availablePositions = new Vector2Int[board.Width * board.Height];
            availablePositionsCount = 0;
        }

        public override void Unload()
        {
            base.Unload();

            board = null;
        }

        public void GenerateElement()
        {
            if (board == null)
            {
                return;
            }

            UpdateAvailablePositions();

            if (availablePositionsCount == 0)
            {
                return;
            }

            var element = new MergeChipModel() { Level = UnityEngine.Random.Range(0, MaxGeneratedElementLevel) };
            var position = availablePositions[UnityEngine.Random.Range(0, availablePositionsCount)];

            board.TryCreateElement(element, position);
        }

        private void UpdateAvailablePositions()
        {
            availablePositionsCount = 0;

            for (int y = 0; y < board.Height; ++y)
            {
                for (int x = 0; x < board.Width; ++x)
                {
                    if (!board.Cells[y, x].Active)
                    {
                        continue;
                    }

                    ref var bucket = ref board.GetBucketAt(x, y);
                    if (bucket.Count == 0)
                    {
                        availablePositions[availablePositionsCount++] = new Vector2Int(x, y);
                    }
                }
            }
        }
    }
}
