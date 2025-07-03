using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class String2Vector3Field : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            // UxmlStringAttributeDescription mLabel =
            //     new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            // public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            // {
            //     base.Init(ve, bag, cc);
            //     var ate = ve as String2Vector3Field;

            //     if (ate is not null)
            //     {
            //         ate.label = mLabel.GetValueFromBag(bag, cc);
            //     }
            // }
        }

        public new class UxmlFactory : UxmlFactory<String2Vector3Field, UxmlTraits> { }
        public string label
        {
            get => mField.label;
            set => mField.label = value;
        }

        private readonly Vector3Field mField;
        private string mValue;

        public String2Vector3Field()
        {
            mField = new Vector3Field();
            mField.RegisterValueChangedCallback(OnFieldValueChanged);
            Add(mField);
        }

        private void OnFieldValueChanged(ChangeEvent<Vector3> evt)
        {
            var newV = evt.newValue;
            value = newV.x + "," + newV.y + "," + newV.z;
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (newValue != mValue)
            {
                mValue = newValue;
                var strArr = string.IsNullOrEmpty(mValue) ? Array.Empty<string>() : mValue.Split(",");
                var vec = new Vector3(
                    strArr.Length <= 0 ? 0 : float.Parse(strArr[0]),
                    strArr.Length <= 1 ? 0 : float.Parse(strArr[1]),
                    strArr.Length <= 2 ? 0 : float.Parse(strArr[2]));
                mField.SetValueWithoutNotify(vec);
            }
        }

        public string value
        {
            get => mValue;

            set
            {
                if (value == mValue)
                    return;

                var previous = this.value;
                SetValueWithoutNotify(value);

                using (var evt = ChangeEvent<string>.GetPooled(previous, value))
                {
                    evt.target = this;
                    SendEvent(evt);
                }
            }
        }
    }
}