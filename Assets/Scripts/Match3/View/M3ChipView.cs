using Crosswork.Core.Intents;
using Crosswork.Demo.Match3.Elements;
using Crosswork.Demo.Match3.Intents;
using Crosswork.View;
using System;
using UnityEngine;

namespace Crosswork.Demo.Match3.View
{
    public class M3ChipView : ElementView<M3Chip>
    {
        [Serializable]
        public struct ColorConfig
        {
            public int Color;
            public Sprite Main;
            public GameObject DestroyEffect;
        }

        [SerializeField]
        private SpriteRenderer sprite;
        [SerializeField]
        private ColorConfig[] colors;

        protected override void OnCreated(M3Chip element, IIntent intent)
        {
            base.OnCreated(element, intent);

            sprite.sprite = FindColorConfig(element.Color).Main;
        }

        private ColorConfig FindColorConfig(int color)
        {
            for (int i = 0; i < colors.Length; ++i)
            {
                if (colors[i].Color == color)
                {
                    return colors[i];
                }
            }

            return colors[0];
        }

        public override void OnDestroying(IIntent intent)
        {
            base.OnDestroying(intent);

            if (intent is MatchedIntent)
            {
                var config = FindColorConfig(element.Color);
                if (config.DestroyEffect != null)
                {
                    Instantiate(config.DestroyEffect, transform.position, Quaternion.identity, transform.parent);
                }
            }
        }
    }
}
