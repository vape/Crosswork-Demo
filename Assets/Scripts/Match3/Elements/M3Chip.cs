using Crosswork.Core;

namespace Crosswork.Demo.Match3.Elements
{
    public class M3Chip : Element
    {
        public int Color
        { get { return model.Color; } }

        private M3ChipModel model;

        public M3Chip(M3ChipModel model) 
            : base(model)
        {
            this.model = model;
        }

        public override ulong GetCollisionMask()
        {
            return 0x1;
        }
    }
}