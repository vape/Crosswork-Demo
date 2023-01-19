using Crosswork.Core;
using Crosswork.View;
using System;
using UnityEngine;

namespace Crosswork.Demo
{
    [CreateAssetMenu(fileName = "Demo Board Config", menuName = "Crosswork/Demo/Board Config")]
    public class DemoBoardConfig : ScriptableObject, IBoardViewFactory, IBoardFactory
    {
        [SerializeField]
        private DemoBoardElementConfig[] elements;

        public Element CreateElement(IElementModel model)
        {
            return FindConfigByModelType(model.GetType()).Build(model);
        }

        public ElementView CreateView(Element element, Transform container)
        {
            var template = FindConfigByElementType(element.GetType()).GetTemplate(element);
            var instance = Instantiate(template, container);

            return instance;
        }

        public void PurgeView(ElementView view)
        {
            GameObject.Destroy(view.gameObject);
        }

        private DemoBoardElementConfig FindConfigByElementType(Type elementType)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].ElementType == elementType)
                {
                    return elements[i];
                }
            }

            return null;
        }

        private DemoBoardElementConfig FindConfigByModelType(Type modelType)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].ElementModelType == modelType)
                {
                    return elements[i];
                }
            }

            return null;
        }
    }
}