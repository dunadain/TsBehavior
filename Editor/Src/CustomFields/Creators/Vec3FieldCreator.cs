using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class Vec3FieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2Vector3Field()
            {
                label = jsProp.name
            };
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2Vec3Field", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2Vector3Field>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}