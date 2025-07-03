using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class TsEnumFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new TsEnumField
            {
                label = jsProp.name,
                choices = jsProp.info as List<string>
            };
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            return field;
        }
        public override VisualElement makeItem()
        {
            return null;
        }
    }
}