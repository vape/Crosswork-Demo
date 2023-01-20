using Crosswork.Core;
using System;

namespace Crosswork.Demo.Merge.Elements
{
    public class MergeChip : Element
    {
        public event Action LevelChanged;

        public int Level
        { get { return model.Level; } }

        private MergeChipModel model;

        public MergeChip(MergeChipModel model)
            : base(model)
        {
            this.model = model;
        }

        public override ulong GetCollisionMask()
        {
            return 0x1;
        }

        public void SetLevel(int level)
        {
            model.Level = level;
            LevelChanged?.Invoke();
        }
    }
}
