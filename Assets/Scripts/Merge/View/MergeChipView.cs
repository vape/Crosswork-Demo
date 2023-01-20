using Crosswork.Core.Intents;
using Crosswork.Demo.Merge.Elements;
using Crosswork.View;
using System;
using UnityEngine;

namespace Crosswork.Demo.Merge.View
{
    public class MergeChipView : ElementView<MergeChip>
    {
        [Serializable]
        public struct LevelConfig
        {
            public Sprite Main;
        }

        [SerializeField]
        private SpriteRenderer sprite;
        [SerializeField]
        private LevelConfig[] levels;

        protected override void OnCreated(MergeChip element, IIntent intent)
        {
            base.OnCreated(element, intent);

            UpdateSprite();
            element.LevelChanged += LevelChangedHandler;
        }

        private void UpdateSprite()
        {
            sprite.sprite = levels[element.Level].Main;
        }

        private void LevelChangedHandler()
        {
            UpdateSprite();
        }
    }
}
