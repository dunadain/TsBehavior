using UnityEngine.UIElements;

namespace TsBehavior
{
    public class String2BoolField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            // UxmlStringAttributeDescription mLabel =
            //     new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            // public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            // {
            //     base.Init(ve, bag, cc);
            //     var ate = (String2BoolField)ve;

            //     if (ate is not null)
            //     {
            //         ate.label = mLabel.GetValueFromBag(bag, cc);
            //     }
            // }
        }

        public new class UxmlFactory : UxmlFactory<String2BoolField, UxmlTraits> { }
        public string label
        {
            get => mField.label;
            set => mField.label = value;
        }

        private readonly Toggle mField;
        private string mValue;

        public String2BoolField()
        {
            mField = new Toggle();
            mField.RegisterValueChangedCallback(OnFieldValueChanged);
            Add(mField);
        }

        void OnFieldValueChanged(ChangeEvent<bool> evt)
        {
            value = evt.newValue.ToString();
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (newValue != mValue)
            {
                mValue = newValue;
                var realV = string.IsNullOrEmpty(mValue) ? "False" : newValue;
                var finalv = false;
                try
                {
                    finalv = bool.Parse(realV);
                }
                catch
                {
                    finalv = false;
                }
                mField.SetValueWithoutNotify(finalv);
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