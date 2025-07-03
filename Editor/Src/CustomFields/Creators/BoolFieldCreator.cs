using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class BoolFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2BoolField();
            field.label = jsProp.name;
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }

        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2BoolField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2BoolField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}