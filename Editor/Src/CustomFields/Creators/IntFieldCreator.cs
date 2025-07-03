using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class IntFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2IntField();
            field.label = jsProp.name;
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2IntField", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2IntField>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}