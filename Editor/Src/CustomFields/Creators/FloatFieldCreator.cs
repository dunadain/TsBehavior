using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class FloatFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2FloatField();
            field.label = jsProp.name;
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2FloatField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2FloatField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}