using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class String2ColorField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            // UxmlStringAttributeDescription mLabel =
            //     new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            // public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            // {
            //     base.Init(ve, bag, cc);
            //     var ate = ve as String2ColorField;

            //     if (ate is not null)
            //     {
            //         ate.label = mLabel.GetValueFromBag(bag, cc);
            //     }
            // }
        }

        public new class UxmlFactory : UxmlFactory<String2ColorField, UxmlTraits> { }
        public string label
        {
            get => mField.label;
            set => mField.label = value;
        }

        private readonly ColorField mField;
        private string mValue;

        public String2ColorField()
        {
            mField = new ColorField();
            mField.RegisterValueChangedCallback(OnFieldValueChanged);
            Add(mField);
        }

        private void OnFieldValueChanged(ChangeEvent<Color> evt)
        {
            var newV = evt.newValue;
            value = newV.r + "," + newV.g + "," + newV.b + "," + newV.a;
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (newValue != mValue)
            {
                mValue = newValue;
                var strArr = string.IsNullOrEmpty(mValue) ? Array.Empty<string>() : mValue.Split(",");
                var vec = new Color(
                    strArr.Length <= 0 ? 0 : float.Parse(strArr[0]),
                    strArr.Length <= 1 ? 0 : float.Parse(strArr[1]),
                    strArr.Length <= 2 ? 0 : float.Parse(strArr[2]),
                    strArr.Length <= 3 ? 1 : float.Parse(strArr[3]));
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