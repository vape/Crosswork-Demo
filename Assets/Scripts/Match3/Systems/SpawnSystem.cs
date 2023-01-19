using Crosswork.Core;
using Crosswork.Demo.Match3.Elements;
using Crosswork.Demo.Tweens;
using Crosswork.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Crosswork.Demo.Match3.Systems
{
    public class SpawnSystem : DemoBoardSystem
    {
        private IElementModel nextElement;
        private System.Random random;
        private CrossworkBoard board;
        private Vector2Int[] spawners;
        private int spawnersCount;
        private CrossworkBoardView view;
        private int colorsCount;

        public SpawnSystem(int colorsCount)
        {
            this.colorsCount = colorsCount;
        }

        public override void Load(CrossworkBoard board, CrossworkBoardView view, DemoBoardInput input)
        {
            base.Load(board, view, input);

            this.board = board;
            this.view = view;

            random = new System.Random();
            spawners = new Vector2Int[board.Width];

            for (int x = 0; x < board.Width; x++)
            {
                var foundActive = false;

                for (int y = 0; y < board.Height; y++)
                {
                    var active = board.Cells[y, x].Active;

                    if (!foundActive && active)
                    {
                        foundActive = true;
                    }
                    else if (foundActive && !active)
                    {
                        spawners[spawnersCount++] = new Vector2Int(x, y - 1);
                        break;
                    }
                    else if (foundActive && y == board.Height - 1)
                    {
                        spawners[spawnersCount++] = new Vector2Int(x, y);
                        break;
                    }
                }
            }

            GenerateNextElement();
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            for (int i = 0; i < spawnersCount; ++i)
            {
                ref var p = ref spawners[i];

                if (board.IsCellLocked(p.x, p.y))
                {
                    continue;
                }

                ref var bucket = ref board.Buckets[p.y, p.x];
                if (bucket.Count > 0)
                {
                    continue;
                }

                if (board.TryCreateElement(nextElement, p, out var createdElement))
                {
                    SpawnAsync(createdElement);
                    GenerateNextElement();
                }
            }
        }

        private async void SpawnAsync(Element element)
        {
            const float duration = 0.1f;

            var @lock = board.LockElement(element);

            var elementView = view.GetView(element);
            var cell = element.GetCell().Position;
            var local = view.GridToLocalPosition(cell.x, cell.y + 0.75f);

            elementView.transform.position = view.LocalToWorldPosition(local);
            elementView.transform.localScale = Vector3.zero;
            elementView.transform.TweenLocalMove(view.GridToWorldPosition(cell), duration).Play();
            elementView.transform.TweenScale(Vector3.one, duration).Play();

            await Task.Delay((int)(duration * 1000 / Time.timeScale));

            board.UnlockElement(@lock);
        }

        private void GenerateNextElement()
        {
            nextElement = new M3ChipModel(random.Next(0, colorsCount));
        }
    }
}
