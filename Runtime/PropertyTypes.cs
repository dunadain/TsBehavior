using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace TsBehavior
{
    [Serializable]
    public class PropertyValue
    {
        public UnityEngine.Object objValue;
        public string primitiveValue;
        public AssetReference refValue;
    }

    [Serializable]
    public class Property
    {
        public string name;
        public string typeName;
        public PropertyValue value;
    }

    [Serializable]
    public class ListProperty
    {
        public string name;
        public string typeName;
        public List<PropertyValue> list;
    }

    // public enum PropertyValueType {
    //     Primitive,
    //     Object
    // }

    public class JsProperty
    {
        public string name;
        public Type type;
        public object info;
        public string defaultValue;
        public bool isEnum;
    }
}