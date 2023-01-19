using Crosswork.Core;
using Crosswork.View;
using System;
using UnityEngine;

namespace Crosswork.Demo.Match3.Systems.Matching
{
    public class GroupFinder
    {
        public delegate int ElementSelectionDelegate(Element element);

        [Flags]
        public enum Flag : short
        {
            None = 0,
            Active = 1,
            Matchable = 2,
            Traversed = 4
        }

        public struct GroupElement
        {
            public readonly Vector2Int Position;
            public readonly int BucketIndex;

            public GroupElement(Vector2Int position, int bucketIndex)
            {
                Position = position;
                BucketIndex = bucketIndex;
            }
        }

        public struct Group
        {
            public int Kind;
            public GroupElement[] Elements;
        }

        public struct ElementState
        {
            public Flag Flag;
            public int Kind;
            public int BucketIndex;
        }

        private static readonly Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        public Group[] Groups;
        public int Count;

        private CrossworkBoard board;
        private ElementState[,] data;
        private GroupElement[] groupBuffer;
        private ElementSelectionDelegate elementSelector;

        public GroupFinder(CrossworkBoard board, ElementSelectionDelegate selector)
        {
            this.board = board;

            data = new ElementState[board.Height, board.Width];
            groupBuffer = new GroupElement[board.Height * board.Width];
            Groups = new Group[board.Height * board.Width];
            elementSelector = selector;
        }

        private void Clear()
        {
            for (int y = 0; y < board.Height; ++y)
            {
                for (int x = 0; x < board.Width; ++x)
                {
                    data[y, x] = new ElementState();

                    if (board.Cells[y, x].Active && !board.IsCellLocked(x, y))
                    {
                        data[y, x].Flag |= Flag.Active;
                    }
                    else
                    {
                        continue;
                    }

                    ref var bucket = ref board.GetBucketAt(x, y);
                    for (int i = 0; i < bucket.Count; ++i)
                    {
                        if (board.IsElementLocked(bucket.Elements[i]))
                        {
                            continue;
                        }

                        var kind = elementSelector(bucket.Elements[i]);
                        if (kind < 0)
                        {
                            continue;
                        }

                        data[y, x].Flag |= Flag.Matchable;
                        data[y, x].BucketIndex = i;
                        data[y, x].Kind = kind;
                    }
                }
            }
        }

        public void Find(int minGroupSize = 3)
        {
            Clear();
            
            var width = board.Width;
            var height = board.Height;

            Count = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; ++x)
                {
                    if ((data[y, x].Flag & Flag.Traversed) != 0)
                    {
                        continue;
                    }

                    if ((data[y, x].Flag & Flag.Matchable) == 0)
                    {
                        continue;
                    }

                    FindGroup(x, y, out var elementsCount);
                    
                    if (elementsCount < minGroupSize)
                    {
                        continue;
                    }

                    if (Groups[Count].Elements == null || Groups[Count].Elements.Length != elementsCount)
                    {
                        Groups[Count].Elements = new GroupElement[elementsCount];
                    }

                    // Debug_DrawGroup(board, in Groups[Count]);

                    Array.Copy(groupBuffer, Groups[Count].Elements, elementsCount);
                    Count++;
                }
            }
        }

        private void FindGroup(int x, int y, out int count)
        {
            Traverse(board, ref data, ref groupBuffer, x, y, out count);
        }

        private static void Traverse(CrossworkBoard board, ref ElementState[,] data, ref GroupElement[] buffer, int posX, int posY, out int count)
        {
            count = 0;
            TraverseRecursive(board, ref data, ref buffer, posX, posY, ref count);
        }

        private static void TraverseRecursive(CrossworkBoard board, ref ElementState[,] data, ref GroupElement[] buffer, int posX, int posY, ref int count)
        {
            ref var element = ref data[posY, posX];

            buffer[count++] = new GroupElement(new Vector2Int(posX, posY), element.BucketIndex);
            data[posY, posX].Flag |= Flag.Traversed;

            for (int i = 0; i < offsets.Length; ++i)
            {
                var x = posX + offsets[i].x;
                var y = posY + offsets[i].y;

                if (!board.InsideBounds(x, y))
                {
                    continue;
                }

                ref var e = ref data[y, x];

                if ((e.Flag & Flag.Traversed) != 0 || (e.Flag & Flag.Matchable) == 0 || e.Kind != element.Kind)
                {
                    continue;
                }

                TraverseRecursive(board, ref data, ref buffer, x, y, ref count);
            }
        }

        public static void Debug_DrawGroup(CrossworkBoard board, in Group group)
        {
            Color GenerateColor(int seed)
            {
                var random = new System.Random(seed);
                var r = random.Next(32, 256);
                var g = random.Next(32, 256);
                var b = random.Next(32, 256);

                return new Color(r / 255f, g / 255f, b / 255f);
            }

            var view = board.View as CrossworkBoardView;
            var position = group.Elements[0].Position;
            var element = board.GetBucketAt(position).Elements[group.Elements[0].BucketIndex];
            var color = GenerateColor(element.GetHashCode());

            for (int i = 0; i < group.Elements.Length - 1; ++i)
            {
                var p0 = view.GridToWorldPosition(group.Elements[i].Position);
                var p1 = view.GridToWorldPosition(group.Elements[i + 1].Position);

                for (int k = 0; k < offsets.Length; ++k)
                {
                    var pp0 = p0 + new Vector3(offsets[k].x * 0.01f, offsets[k].y * 0.01f);
                    var pp1 = p1 + new Vector3(offsets[k].x * 0.01f, offsets[k].y * 0.01f);

                    Debug.DrawLine(pp0, pp1, color);
                }
            }
        }
    }
}
