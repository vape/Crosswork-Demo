using Crosswork.Core;
using Crosswork.Core.Intents;
using Crosswork.Demo.Match3.Elements;
using Crosswork.Demo.Match3.Intents;
using Crosswork.View;
using UnityEngine;

namespace Crosswork.Demo.Match3.Systems.Matching
{
    public class MatchingSystem : DemoBoardSystem
    {
        public class MatchIntent : IIntent
        {
            public static readonly MatchIntent Instance = new MatchIntent();
        }

        private CrossworkBoard board;
        private GroupFinder groupFinder;
        private PatternMatcher patternMatcher;

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;
            groupFinder = new GroupFinder(board, GroupElementSelector);
            patternMatcher = new PatternMatcher(board.Width, board.Height);
        }

        //private void DebugDragPatterns()
        //{
        //    for (int i = 0; i < patternMatcher.Count; ++i)
        //    {
        //        ref var pattern = ref patternMatcher.Matches[i];
        //        ref var group = ref groupFinder.Groups[pattern.Group];

        //        GroupFinder.Debug_DrawGroup(board, in group);
        //    }
        //}

        public void ForceUpdate()
        {
            UpdateMatches();
        }

        public bool HasMatchAt(Vector2Int position)
        {
            for (int i = 0; i < patternMatcher.Count; i++)
            {
                ref var group = ref groupFinder.Groups[patternMatcher.Matches[i].Group];

                for (int k = 0; k < group.Elements.Length; ++k)
                {
                    if (group.Elements[k].Position == position)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdateMatches()
        {
            groupFinder.Find();
            patternMatcher.Find(in groupFinder.Groups, groupFinder.Count);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            UpdateMatches();
            // DebugDragPatterns();
            ClearMatchedElements();
        }

        private void ClearMatchedElements()
        {
            for (int i = 0; i < patternMatcher.Count; ++i)
            {
                ref var pattern = ref patternMatcher.Matches[i];
                ref var group = ref groupFinder.Groups[pattern.Group];

                var canDestroyGroup = true;

                for (int k = 0; k < group.Elements.Length; ++k)
                {
                    ref var bucket = ref board.GetBucketAt(group.Elements[k].Position);

                    if (!board.CanDestroyElement(bucket.Elements[group.Elements[k].BucketIndex]))
                    {
                        canDestroyGroup = false;
                        break;
                    }
                }

                if (canDestroyGroup)
                {
                    for (int k = 0; k < group.Elements.Length; ++k)
                    {
                        ref var bucket = ref board.GetBucketAt(group.Elements[k].Position);

                        board.DestroyElement(bucket.Elements[group.Elements[k].BucketIndex], MatchedIntent.Instance);
                    }
                }
            }
        }

        private int GroupElementSelector(Element element)
        {
            var chip = element as M3Chip;
            if (chip == null)
            {
                return -1;
            }

            return chip.Color;
        }
    }
}
