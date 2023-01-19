using Crosswork.Demo.Match3.View;
using Crosswork.View;
using UnityEngine;

namespace Crosswork.Demo.Match3.Elements
{
    [CreateAssetMenu(fileName = "Match3 Chip Element Config", menuName = "Crosswork/Demo/Match3/Chip Element Config")]
    public class M3ChipConfig : DemoBoardElementConfig<M3Chip, M3ChipModel>
    {
        [SerializeField]
        private M3ChipView template;

        public override M3Chip CreateElement(M3ChipModel model)
        {
            return new M3Chip(model);
        }

        public override ElementView GetTemplate(M3Chip element)
        {
            return template;
        }
    }
}
