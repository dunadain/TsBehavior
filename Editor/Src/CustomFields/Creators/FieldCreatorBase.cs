using UnityEditor;
using UnityEngine.UIElements;

namespace TsBehavior
{
    public abstract class FieldCreatorBase
    {
        public JsProperty jsProp;
        public SerializedProperty property;
        public abstract VisualElement getField();

        protected virtual string GetBindablePath()
        {
            return "value.primitiveValue";
        }

        protected virtual string GetListBindingPath()
        {
            return "primitiveValue";
        }


        /// <summary>
        /// This method is used to create a new item for the list
        /// 之所以用uxml文件是因为之前直接new 有bug
        /// </summary>
        /// <returns></returns>
        public abstract VisualElement makeItem();
    }
}