
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class String2LongField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            // UxmlStringAttributeDescription mLabel =
            //     new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            // public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            // {
            //     base.Init(ve, bag, cc);
            //     var ate = ve as String2LongField;

            //     if (ate is not null)
            //     {
            //         ate.label = mLabel.GetValueFromBag(bag, cc);
            //     }
            // }
        }

        public new class UxmlFactory : UxmlFactory<String2LongField, UxmlTraits> { }
        public string label
        {
            get => mField.label;
            set => mField.label = value;
        }

        private readonly LongField mField;
        private string mValue;

        public String2LongField()
        {
            mField = new LongField();
            mField.RegisterValueChangedCallback(OnFieldValueChanged);
            Add(mField);
        }

        void OnFieldValueChanged(ChangeEvent<long> evt)
        {
            value = evt.newValue.ToString();
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (newValue != mValue)
            {
                mValue = newValue;
                var realV = string.IsNullOrEmpty(mValue) ? "0" : mValue;
                mField.SetValueWithoutNotify(long.Parse(realV));
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