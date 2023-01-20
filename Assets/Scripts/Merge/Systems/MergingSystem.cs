using Crosswork.Core;
using Crosswork.Demo.Merge.Elements;
using Crosswork.View;

namespace Crosswork.Demo.Merge.Systems
{
    public class MergingSystem : DemoBoardSystem
    {
        public const int MaxLevel = 6;

        private CrossworkBoard board;

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;
        }

        public bool CanMerge(MergeChip element, MergeChip target)
        {
            if (element == target)
            {
                return false;
            }

            if (element.Level != target.Level || target.Level >= MaxLevel - 1)
            {
                return false;
            }

            if (!board.CanDestroyElement(element) || board.IsElementLocked(target))
            {
                return false;
            }

            return true;
        }

        public void Merge(MergeChip element, MergeChip target)
        {
            board.DestroyElement(element);
            target.SetLevel(target.Level + 1);
        }

        public bool TryMerge(MergeChip element, MergeChip target)
        {
            if (!CanMerge(element, target))
            {
                return false;
            }

            Merge(element, target);
            return true;
        }
    }
}
