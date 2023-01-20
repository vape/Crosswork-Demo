using Crosswork.Demo.Merge.Elements;
using Crosswork.Demo.Merge.Systems;
using UnityEngine;

namespace Crosswork.Demo.Merge
{
    public class MergeLoader : MonoBehaviour
    {
        public const int LevelCount = 6;

        private const int width = 9;
        private const int height = 9;

        [SerializeField]
        private DemoBoard board;

        private GeneratorSystem generator;

        private void Start()
        {
            Load();
        }

        public void Restart()
        {
            board.Unload();
            Load();
        }

        private void Load()
        {
            generator = new GeneratorSystem();

            var merging = new MergingSystem();
            var dragging = new DraggingSystem(merging);

            var manifest = new DemoLevelManifest()
            {
                Model = GenerateLevel(),
                Systems = new DemoBoardSystem[] { merging, dragging, generator }
            };

            board.Load(manifest);
        }

        public void GenerateElement()
        {
            generator.GenerateElement();
        }

        private DemoLevelModel GenerateLevel()
        {
            var level = new DemoLevelModel();
            level.SetCells(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((Mathf.Abs(x - 4) + Mathf.Abs(y - 4)) > 3)
                    {
                        level.AddElement(x, y, new MergeChipModel() { Level = Random.Range(0, LevelCount / 3) });
                    }
                }
            }

            return level;
        }
    }
}