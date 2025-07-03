using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class TsEnumField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits { }

        public new class UxmlFactory : UxmlFactory<TsEnumField, UxmlTraits> { }
        public string label
        {
            get => mField.label;
            set => mField.label = value;
        }

        private readonly DropdownField mField;
        private string mValue;

        public TsEnumField()
        {
            mField = new DropdownField();
            mField.RegisterValueChangedCallback(OnFieldValueChanged);
            Add(mField);
        }

        void OnFieldValueChanged(ChangeEvent<string> evt)
        {
            value = evt.newValue;
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (choices.IndexOf(newValue) < 0)
            {
                newValue = choices[0];
            }
            mValue = newValue;
            mField.SetValueWithoutNotify(mValue);
        }

        public List<string> choices
        {
            get => mField.choices;
            set => mField.choices = value;
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

                using var evt = ChangeEvent<string>.GetPooled(previous, value);
                evt.target = this;
                SendEvent(evt);
            }
        }
    }
}