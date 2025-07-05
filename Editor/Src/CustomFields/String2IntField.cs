
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class String2IntField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            // UxmlStringAttributeDescription mLabel =
            //     new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            // public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            // {
            //     base.Init(ve, bag, cc);
            //     var ate = ve as String2IntField;

            //     if (ate is not null)
            //     {
            //         ate.label = mLabel.GetValueFromBag(bag, cc);
            //     }
            // }
        }

        public new class UxmlFactory : UxmlFactory<String2IntField, UxmlTraits> { }
        public string label
        {
            get => mIntField.label;
            set
            {
                mIntField.label = value;
            }
        }

        private IntegerField mIntField;
        private string mValue;

        public String2IntField()
        {
            mIntField = new IntegerField();
            mIntField.RegisterValueChangedCallback(OnIntFieldValueChanged);
            Add(mIntField);
        }

        void OnIntFieldValueChanged(ChangeEvent<int> evt)
        {
            value = evt.newValue.ToString();
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (newValue != mValue)
            {
                mValue = newValue;
                var realV = string.IsNullOrEmpty(mValue) ? "0" : mValue;
                mIntField.SetValueWithoutNotify(int.Parse(realV));
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

                using var evt = ChangeEvent<string>.GetPooled(previous, value);
                evt.target = this;
                SendEvent(evt);
            }
        }
    }
}