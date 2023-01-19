using Crosswork.Core;

namespace Crosswork.Demo.Match3.Elements
{
    public class M3ChipModel : IElementModel
    {
        public int Color;

        public M3ChipModel()
        { }

        public M3ChipModel(int color)
        {
            Color = color;
        }
    }
}