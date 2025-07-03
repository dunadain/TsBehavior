using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace TsBehavior
{
    public class ObjectFieldCreator : FieldCreatorBase
    {
        public override VisualElement getField()
        {
            var field = new ObjectField
            {
                label = jsProp.name,
                objectType = jsProp.type
            };
            field.BindProperty(property.FindPropertyRelative(GetBindablePath()));
            field.RegisterValueChangedCallback(changeEvent =>
            {
                ProcessObjValue(changeEvent, field, false);
            });
            var labels = field.Query<Label>().ToList();
            var labelToChange = labels[1];
            labelToChange.RegisterValueChangedCallback(evt =>
            {
                if (field.value is not JsMonoBehaviorHost host) return;
                ChangeLabelTxt(host, labelToChange);
            });
            return field;
        }

        private void ChangeLabelTxt(JsMonoBehaviorHost host, Label labelToChange)
        {
            var originaltext = labelToChange.text;
            var index = originaltext.LastIndexOf("(", StringComparison.Ordinal);
            var nodeTxt = originaltext[..index];
            labelToChange.text = $"{nodeTxt}(JS:{host.jsClassName})";
        }

        private void ProcessObjValue(ChangeEvent<Object> changeEvent, ObjectField field, bool isList)
        {
            if (changeEvent.newValue == null || changeEvent.newValue is not JsMonoBehaviorHost host) return;
            if (host.jsClassName == (string)jsProp.info)
            {
                var labels = field.Query<Label>().ToList();
                var labelToChange = isList ? labels[0] : labels[1];
                ChangeLabelTxt(host, labelToChange);
                var image = field.Q<Image>();
                image.image = Resources.Load<Texture2D>("ts-icon");
            }
            else
            {
                var value2Set = changeEvent.previousValue;
                var hosts = host.GetComponents<JsMonoBehaviorHost>();
                foreach (var jsMonoBehaviorHost in hosts)
                {
                    if (jsMonoBehaviorHost.jsClassName != (string)jsProp.info) continue;
                    value2Set = jsMonoBehaviorHost;
                    break;
                }
                field.value = value2Set;
            }
        }

        protected override string GetBindablePath()
        {
            return "value.objValue";
        }

        protected override string GetListBindingPath()
        {
            return "objValue";
        }
        public override VisualElement makeItem()
        {
            var tmp = Resources.Load("ObjectFieldTmp", typeof(VisualTreeAsset)) as VisualTreeAsset;
            var ve = tmp.CloneTree();
            var field = ve.Q<ObjectField>();
            field.bindingPath = GetListBindingPath();
            field.objectType = jsProp.type;
            field.RegisterValueChangedCallback(changeEvent =>
            {
                ProcessObjValue(changeEvent, field, true);
            });
            var labels = field.Query<Label>().ToList();
            var labelToChange = labels[0];
            labelToChange.RegisterValueChangedCallback(evt =>
            {
                if (field.value is not JsMonoBehaviorHost host) return;
                ChangeLabelTxt(host, labelToChange);
            });
            return ve;
        }
    }
}