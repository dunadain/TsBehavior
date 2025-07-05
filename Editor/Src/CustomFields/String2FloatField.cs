
using UnityEngine.UIElements;

namespace TsBehavior
{
    public class String2FloatField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlTraits : BindableElement.UxmlTraits
        {
            /// 这玩意儿是在ui builder的inspector里显示的
            // UxmlStringAttributeDescription mLabel =
            //     new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            // public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            // {
            //     base.Init(ve, bag, cc);
            //     var ate = ve as String2FloatField;

            //     if (ate is not null)
            //     {
            //         ate.label = mLabel.GetValueFromBag(bag, cc);
            //     }
            // }
        }

        public new class UxmlFactory : UxmlFactory<String2FloatField, UxmlTraits> { }
        public string label
        {
            get => mField.label;
            set
            {
                mField.label = value;
            }
        }

        private FloatField mField;
        private string mValue;

        public String2FloatField()
        {
            mField = new FloatField();
            mField.RegisterValueChangedCallback(OnFieldValueChanged);
            Add(mField);
        }

        void OnFieldValueChanged(ChangeEvent<float> evt)
        {
            value = evt.newValue.ToString();
        }

        public void SetValueWithoutNotify(string newValue)
        {
            if (newValue != mValue)
            {
                mValue = newValue;
                var realV = string.IsNullOrEmpty(mValue) ? "0" : mValue;
                mField.SetValueWithoutNotify(float.Parse(realV));
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