using Crosswork.Demo.Merge.View;
using Crosswork.View;
using UnityEngine;

namespace Crosswork.Demo.Merge.Elements
{
    [CreateAssetMenu(fileName = "Merge Chip Element Config", menuName = "Crosswork/Demo/Merge/Chip Element Config")]
    public class MergeChipConfig : DemoBoardElementConfig<MergeChip, MergeChipModel>
    {
        [SerializeField]
        private MergeChipView template;

        public override MergeChip CreateElement(MergeChipModel model)
        {
            return new MergeChip(model);
        }

        public override ElementView GetTemplate(MergeChip element)
        {
            return template;
        }
    }
}
