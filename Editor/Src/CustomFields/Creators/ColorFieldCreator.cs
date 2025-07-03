using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class ColorFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2ColorField();
            field.label = jsProp.name;
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2ColorField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2ColorField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}