using Crosswork.Core;
using Crosswork.Demo.Match3.Elements;
using Crosswork.Demo.Match3.Systems;
using Crosswork.Demo.Match3.Systems.Matching;
using System.Collections.Generic;
using UnityEngine;

namespace Crosswork.Demo.Match3
{
    public class Match3Loader : MonoBehaviour
    {
        public const int ColorsCount = 6;

        [SerializeField]
        private DemoBoard board;

        private void Start()
        {
            var matching = new MatchingSystem();
            var swapping = new SwappingSystem(matching);
            var gravity = new GravitySystem();
            var spawn = new SpawnSystem(ColorsCount);

            var manifest = new DemoLevelManifest()
            {
                Model = GenerateLevel(),
                Systems = new DemoBoardSystem[] { matching, swapping, gravity, spawn }
            };

            board.Load(manifest);
        }

        private DemoLevelModel GenerateLevel()
        {
            Vector2Int[] offsetsToCheck = new Vector2Int[] { Vector2Int.down, Vector2Int.left };

            var width = 9;
            var height = 9;

            var level = new DemoLevelModel();

            var cells = new Cell[height, width];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((Mathf.Abs(x - 4) + Mathf.Abs(y - 4)) < 7)
                    {
                        cells[y, x] = new Cell(new Vector2Int(x, y), active: true);
                    }
                }
            }

            level.SetCells(cells);

            var colors = new List<int>(capacity: ColorsCount);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!cells[y, x].Active)
                    {
                        continue;
                    }

                    colors.Clear();

                    for (int i = 0; i < ColorsCount; ++i)
                    {
                        colors.Add(i);
                    }

                    for (int i = 0; i < offsetsToCheck.Length; ++i)
                    {
                        var xx = x + offsetsToCheck[i].x;
                        var yy = y + offsetsToCheck[i].y;

                        if (xx >= 0 && yy >= 0 && xx < width && yy < height)
                        {
                            if (!level.TryGetCellModel(xx, yy, out var cell))
                            {
                                continue;
                            }

                            if (cell.Elements != null && cell.Elements.Length > 0)
                            {
                                for (int k = 0; k < cell.Elements.Length; ++k)
                                {
                                    var chip = cell.Elements[k] as M3ChipModel;
                                    if (chip != null)
                                    {
                                        colors.Remove(chip.Color);
                                    }
                                }
                            }
                        }
                    }

                    var color = colors[Random.Range(0, colors.Count)];
                    level.AddElement(x, y, new M3ChipModel() { Color = color });
                }
            }

            return level;
        }
    }
}