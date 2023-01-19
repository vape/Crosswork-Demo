using Crosswork.Core;
using Crosswork.View;
using System;
using UnityEngine;

namespace Crosswork.Demo
{
    public abstract class DemoBoardElementConfig : ScriptableObject
    {
        public abstract Type ElementType
        { get; }
        public abstract Type ElementModelType
        { get; }

        public abstract Element Build(IElementModel model);
        public abstract ElementView GetTemplate(Element element);
    }

    public abstract class DemoBoardElementConfig<TElement, TElementModel> : DemoBoardElementConfig
        where TElement : Element
        where TElementModel: class, IElementModel
    {
        public override Type ElementType => typeof(TElement);
        public override Type ElementModelType => typeof(TElementModel);

        public override sealed Element Build(IElementModel model)
        {
            return CreateElement(model as TElementModel);
        }

        public override sealed ElementView GetTemplate(Element element)
        {
            return GetTemplate(element as TElement);
        }

        public abstract TElement CreateElement(TElementModel model);
        public abstract ElementView GetTemplate(TElement element);
    }
}
