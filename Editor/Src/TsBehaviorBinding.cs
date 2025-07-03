using System;
using System.Collections.Generic;
using Puerts;
using TsBehavior;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Configure]
public class TsBehaviorBinding
{
    [Binding]
    private static IEnumerable<Type> Bindings
    {
        get
        {
            return new List<Type>()
                {
                    typeof(Debug),
                    typeof(Color),
                    typeof(Vector2),
                    typeof(Vector3),
                    typeof(List<int>),
                    typeof(UnityEngine.Object),
                    typeof(double),
                    typeof(long),
                    typeof(AssetReference),
                    typeof(PropertyValue),
                    typeof(Property),
                    typeof(ListProperty),
                    typeof(JsProperty),
                    typeof(JsMonoBehaviorHost),
                    typeof(GameObjExtensions),
                    typeof(Component),
                };
        }
    }
}