using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class AssetRefFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new PropertyField
            {
                label = jsProp.name
            };
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }

        protected override string GetBindablePath()
        {
            return "value.refValue";
        }

        protected override string GetListBindingPath()
        {
            return "refValue";
        }

        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("AssetRefField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<PropertyField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}