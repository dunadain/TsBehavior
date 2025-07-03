using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class LongFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2LongField();
            field.label = jsProp.name;
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2LongField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2LongField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}