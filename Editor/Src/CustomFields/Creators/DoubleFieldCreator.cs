using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class DoubleFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2DoubleField();
            field.label = jsProp.name;
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2DoubleField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2DoubleField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}