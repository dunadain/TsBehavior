using UnityEditor;

namespace TsBehavior
{
    public class FieldFactory
    {
        public static FieldCreatorBase Create(JsProperty jsProp, SerializedProperty property)
        {
            FieldCreatorBase creator;
            switch (jsProp.type.FullName)
            {
                case "System.String":
                    if (jsProp.isEnum)
                    {
                        creator = new TsEnumFieldCreator();
                    }
                    else
                    {
                        creator = new TextFieldCreator();
                    }
                    break;
                case "System.Int32":
                    creator = new IntFieldCreator();
                    break;
                case "System.Int64":
                    creator = new LongFieldCreator();
                    break;
                case "System.Boolean":
                    creator = new BoolFieldCreator();
                    break;
                case "System.Double":
                    creator = new DoubleFieldCreator();
                    break;
                case "System.Single":
                    creator = new FloatFieldCreator();
                    break;
                case "UnityEngine.Color":
                    creator = new ColorFieldCreator();
                    break;
                case "UnityEngine.Vector2":
                    creator = new Vec2FieldCreator();
                    break;
                case "UnityEngine.Vector3":
                    creator = new Vec3FieldCreator();
                    break;
                case "UnityEngine.AddressableAssets.AssetReference":
                    creator = new AssetRefFieldCreator();
                    break;
                default:
                    creator = new ObjectFieldCreator();
                    break;
            }

            creator.jsProp = jsProp;
            creator.property = property;
            return creator;
        }
    }
}