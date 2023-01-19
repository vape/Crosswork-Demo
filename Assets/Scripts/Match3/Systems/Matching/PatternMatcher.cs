using UnityEngine;

namespace Crosswork.Demo.Match3.Systems.Matching
{
    public class PatternMatcher
    {
        public enum PatternCategory
        {
            None = 0,
            HorizontalLine = 1,
            VerticalLine = 2,
            Corner = 3,
            Square = 4,
            Star = 5
        }

        public struct Pattern
        {
            public PatternCategory Category;
            public Vector2Int[] Positions;
        }

        public struct Match
        {
            public PatternCategory Category;
            public int Pattern;
            public int Group;
        }

        public static readonly Pattern[] Patterns = new Pattern[]
{
            new Pattern()
            {
                Category = PatternCategory.Star,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2),
                    new Vector2Int(2, 3),
                    new Vector2Int(2, 4),
                    new Vector2Int(-1, 2),
                    new Vector2Int(-2, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Star,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(3, 0),
                    new Vector2Int(4, 0),
                    new Vector2Int(2, 1),
                    new Vector2Int(2, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Star,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2),
                    new Vector2Int(0, 3),
                    new Vector2Int(0, 4)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Star,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2),
                    new Vector2Int(-1, 2),
                    new Vector2Int(-2, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Star,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                    new Vector2Int(0, 3),
                    new Vector2Int(0, 4),
                    new Vector2Int(-1, 2),
                    new Vector2Int(-2, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Star,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2),
                    new Vector2Int(1, 2),
                    new Vector2Int(2, 2),
                    new Vector2Int(0, 3),
                    new Vector2Int(0, 4),
                    new Vector2Int(-1, 2),
                    new Vector2Int(-2, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Corner,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Corner,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(-2, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Corner,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(0, -2)
                }
            },
            new Pattern()
            {
                Category = PatternCategory.Corner,
                Positions = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(-1, 0),
                    new Vector2Int(-2, 0),
                    new Vector2Int(0, -1),
                    new Vector2Int(0, -2)
                }
            },

            new Pattern() { Category = PatternCategory.HorizontalLine, Positions = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0), new Vector2Int(4, 0) } },
            new Pattern() { Category = PatternCategory.VerticalLine, Positions = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4) } },

            new Pattern() { Category = PatternCategory.HorizontalLine, Positions = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0) } },
            new Pattern() { Category = PatternCategory.VerticalLine, Positions = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3) } },

            new Pattern() { Category = PatternCategory.Square, Positions = new Vector2Int[] {new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1,1), new Vector2Int(1,0) } },

            new Pattern() { Category = PatternCategory.HorizontalLine, Positions = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) } },
            new Pattern() { Category = PatternCategory.VerticalLine, Positions = new Vector2Int[] { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) } },
        };

        public Match[] Matches;
        public int Count;

        public PatternMatcher(int width, int height)
        {
            Matches = new Match[width * height];
        }

        public void Find(in GroupFinder.Group[] groups, int groupsCount)
        {
            Count = 0;

            for (int i = 0; i < groupsCount; ++i)
            {
                ref var group = ref groups[i];

                if (IsLine(in group, 0))
                {
                    Matches[Count++] = new Match()
                    {
                        Category = PatternCategory.VerticalLine,
                        Group = i
                    };

                    continue;
                }
                else if (IsLine(in group, 1))
                {
                    Matches[Count++] = new Match()
                    {
                        Category = PatternCategory.HorizontalLine,
                        Group = i
                    };

                    continue;
                }

                for (int k = 0; k < Patterns.Length; ++k)
                {
                    if (TryMatch(in group, in Patterns[k]))
                    {
                        Matches[Count++] = new Match()
                        {
                            Category = Patterns[k].Category,
                            Group = i,
                            Pattern = k
                        };

                        break;
                    }
                }
            }
        }

        public static bool IsLine(in GroupFinder.Group group, int axis)
        {
            for (int i = 1; i < group.Elements.Length; ++i)
            {
                if (group.Elements[i].Position[axis] != group.Elements[0].Position[axis])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool TryMatch(in GroupFinder.Group group, in Pattern pattern)
        {
            if (pattern.Positions.Length > group.Elements.Length)
            {
                return false;
            }

            for (int i = 0; i < group.Elements.Length; ++i)
            {
                if (IsMatching(in group, in pattern, group.Elements[i].Position))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsMatching(in GroupFinder.Group group, in Pattern pattern, Vector2Int offset)
        {
            for (int i = 0; i < pattern.Positions.Length; ++i)
            {
                var matched = false;

                for (int k = 0; k < group.Elements.Length; ++k)
                {
                    var xmatch = group.Elements[k].Position.x - offset.x == pattern.Positions[i].x;
                    var ymatch = group.Elements[k].Position.y - offset.y == pattern.Positions[i].y;

                    if (xmatch && ymatch)
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    return false;
                }
            }

            return true;
        }
    }
}