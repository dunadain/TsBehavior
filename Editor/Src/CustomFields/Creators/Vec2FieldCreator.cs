using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class Vec2FieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new String2Vector2Field()
            {
                label = jsProp.name
            };
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("String2Vec2Field", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            ve.Q<String2Vector2Field>().bindingPath = GetListBindingPath();
            return ve;
        }
    }
}