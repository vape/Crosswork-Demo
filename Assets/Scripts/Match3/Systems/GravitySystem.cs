using Crosswork.Core;
using Crosswork.Demo.Match3.Elements;
using Crosswork.Demo.Tweens;
using Crosswork.View;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Crosswork.Demo.Match3.Systems
{
    public class GravitySystem : DemoBoardSystem
    {
        private struct GravityState
        {
            public int ElementId;
            public ElementLockKey StagingLock;
        }

        private CrossworkBoard board;
        private CrossworkBoardView view;

        private GravityState[,] gravityState;

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;
            this.view = view;

            gravityState = new GravityState[board.Height, board.Width];
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            var width = board.Width;
            var height = board.Height;

            for (int y = 0; y < height - 1; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (!board.Cells[y, x].Active || board.IsCellLocked(x, y))
                    {
                        continue;
                    }

                    ref var bucket = ref board.Buckets[y, x];

                    if (bucket.Count == 0)
                    {
                        TryDropTo(x, y);
                    }
                    else if (gravityState[y, x].ElementId != 0)
                    {
                        gravityState[y, x].ElementId = 0;
                        board.UnlockElement(gravityState[y, x].StagingLock);
                    }
                }
            }
        }

        private bool TryDropTo(int targetX, int targetY)
        {
            var fromX = targetX;
            var fromY = targetY + 1;

            if (!board.Cells[fromY, fromX].Active || board.IsCellLocked(fromX, fromY))
            {
                return false;
            }

            ref var upBucket = ref board.Buckets[fromY, fromX];
            
            if (upBucket.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < upBucket.Count; ++i)
            {
                var element = upBucket.Elements[i];

                if (element is not M3Chip)
                {
                    continue;
                }

                if (gravityState[fromY, fromX].ElementId == element.Id)
                {
                    gravityState[fromY, fromX].ElementId = 0;
                    board.UnlockElement(gravityState[fromY, fromX].StagingLock);
                }

                if (board.CanMoveElement(element, targetX, targetY))
                {
                    DropAsync(upBucket.Elements[i], fromX, fromY, targetX, targetY);
                }

                break;
            }

            return false;
        }

        private async void DropAsync(Element element, int fromX, int fromY, int toX, int toY)
        {
            const float duration = 0.1f;

            var fromCellLock = board.LockCell(fromX, fromY);
            var toCellLock = board.LockCell(toX, toY);
            var elementLock = board.LockElement(element);

            var elementView = view.GetView(element);
            elementView.transform.TweenMove(view.GridToWorldPosition(toX, toY), duration).Play();

            await Task.Delay((int)(1000 * duration / Time.timeScale) + 1);

            board.UnlockCell(fromCellLock);
            board.UnlockCell(toCellLock);
            board.UnlockElement(elementLock);

            if (!board.TryMoveElement(element, toX, toY))
            {
                view.ResetPosition(element);
            }
            else
            {
                gravityState[toY, toX].ElementId = element.Id;
                gravityState[toY, toX].StagingLock = board.LockElement(element);
            }
        }
    }
}
