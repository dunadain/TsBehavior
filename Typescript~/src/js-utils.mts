import type { Constructor } from "./typedef.mjs";



type PropertyType = typeof CS.UnityEngine.Object
    | typeof CS.UnityEngine.Color
    | typeof CS.UnityEngine.Vector2
    | typeof CS.UnityEngine.Vector3
    | typeof CS.System.Int32
    | typeof CS.System.String
    | typeof CS.System.Boolean
    | typeof CS.System.Double
    | typeof CS.System.Single
    | typeof CS.System.Int64
    | typeof CS.UnityEngine.AddressableAssets.AssetReference
    | typeof CS.TsBehavior.JsMonoBehaviorHost

export function property(_type: PropertyType) {
    return (_target: any, _propName: string) => {
        // if (!target['__properties']) {
        //     target['__properties'] = {};
        // }
        // target['__properties'][propName] = {
        //     type: $typeof(type)
        // };
    };
}

export function listproperty(_type: PropertyType) {
    return (_target: any, _propName: string) => {
        // if (!target['__propertyLists']) {
        //     target['__propertyLists'] = {};
        // }
        // target['__propertyLists'][propName] = {
        //     type: $typeof(type)
        // };
    };
}

export function enumfield(enumType: any) {
    return (target: any, propName: string) => {
        if (!target['__enumFields']) {
            target['__enumFields'] = {};
        }
        target['__enumFields'][propName] = enumType;
    };
}

/**
 * 不用decorator factory的话，minify后类名全变了
 */
const nameToClass: Map<string, Constructor> = new Map();
export function mapleclass(className: string) {
    return function (constructor: Constructor) {
        if (nameToClass.has(className)) {
            console.error(`duplicate class: ${className}`);
        }
        nameToClass.set(className, constructor);
    };
}

export function createMonoJs(className: string, nativeMonoBehavior: CS.TsBehavior.JsMonoBehaviorHost) {
    const constructor = nameToClass.get(className);
    if (constructor) {
        const jsComp = new constructor() as any;
        nativeMonoBehavior.jsAwake = jsComp.callByCSharpAwake.bind(jsComp);
        nativeMonoBehavior.jsOnEnable = jsComp.callByCSharpOnEnable.bind(jsComp);
        nativeMonoBehavior.jsStart = jsComp.start?.bind(jsComp);
        nativeMonoBehavior.jsOnDisable = jsComp.callByCSharpOnDisable.bind(jsComp);
        nativeMonoBehavior.jsOnDestroy = jsComp.callByCSharpOnDestroy.bind(jsComp);
        jsComp.setNativeMonoBehavior(nativeMonoBehavior);
        if (nativeMonoBehavior.properties) {
            const size = nativeMonoBehavior.properties.Count;
            for (let i = 0; i < size; ++i) {
                const prop = nativeMonoBehavior.properties.get_Item(i);
                let value = convertValue(prop.typeName, prop.value);
                if (Object.prototype.hasOwnProperty.call(Object.getPrototypeOf(jsComp), '__enumFields') && jsComp['__enumFields'][prop.name]) {
                    const enumType = jsComp['__enumFields'][prop.name];
                    if (enumType[value as string] === undefined) {
                        value = 0;
                    } else
                        value = enumType[value as string];
                }

                jsComp[prop.name] = value;
                if (prop.typeName === 'TsBehavior.JsMonoBehaviorHost') {
                    const jsHost = jsComp[prop.name];
                    if (jsHost && !jsHost.IsNull()) defineGet(jsComp, prop.name, jsHost);
                }
            }
        }

        if (nativeMonoBehavior.listProperties) {
            const size = nativeMonoBehavior.listProperties.Count;
            for (let i = 0; i < size; ++i) {
                const prop = nativeMonoBehavior.listProperties.get_Item(i);
                if (prop.typeName !== 'TsBehavior.JsMonoBehaviorHost') jsComp[prop.name] = convertToListValue(prop);
                else {
                    defineListGet(jsComp, prop);
                }
            }
        }
        return jsComp;
    }
}

function defineGet(obj: any, propName: string, jsHost: any) {
    Object.defineProperty(obj, propName, {
        get() {
            const js = jsHost.MyJsComp();
            if (!js) return null;
            Object.defineProperty(this, propName, {
                value: js,
                configurable: true
            });
            return js;
        },
        configurable: true
    });
}

function defineListGet(obj: any, listProp: CS.TsBehavior.ListProperty) {
    Object.defineProperty(obj, listProp.name, {
        get() {
            const arr = convertToListValue(listProp);
            for (let i = 0; i < arr.length; ++i) {
                arr[i] = (arr[i] as any).MyJsComp();
            }
            Object.defineProperty(this, listProp.name, {
                value: arr,
                configurable: true
            });
            return arr;
        },
        configurable: true
    });
}

function convertToListValue(listProp: CS.TsBehavior.ListProperty) {
    const list = listProp.list;
    const arr = [];
    const size = list.Count;
    for (let i = 0; i < size; ++i) {
        const propV = list.get_Item(i);
        arr.push(convertValue(listProp.typeName, propV));
    }
    return arr;
}

function convertValue(typeName: string, value: CS.TsBehavior.PropertyValue) {
    switch (typeName) {
        case 'System.Int32':
        case 'System.Int64':
        case 'System.Single':
        case 'System.Double':
            return Number(value.primitiveValue);
        case 'System.Boolean':
            return (value.primitiveValue == 'True' || value.primitiveValue == 'true') ? true : false;
        case 'UnityEngine.AddressableAssets.AssetReference':
            return value.refValue;
        case 'UnityEngine.Vector2': {
            const arr = value.primitiveValue.split(',');
            return new CS.UnityEngine.Vector2(
                arr.length > 0 ? Number(arr[0]) : 0,
                arr.length > 1 ? Number(arr[1]) : 0
            );
        }
        case 'UnityEngine.Vector3': {
            const arr = value.primitiveValue.split(',');
            return new CS.UnityEngine.Vector3(
                arr.length > 0 ? Number(arr[0]) : 0,
                arr.length > 1 ? Number(arr[1]) : 0,
                arr.length > 2 ? Number(arr[2]) : 0
            );
        }
        case 'UnityEngine.Color': {
            const arr = value.primitiveValue.split(',');
            return new CS.UnityEngine.Color(
                arr.length > 0 ? Number(arr[0]) : 0,
                arr.length > 1 ? Number(arr[1]) : 0,
                arr.length > 2 ? Number(arr[2]) : 0,
                arr.length > 3 ? Number(arr[3]) : 1
            );
        }
        case 'System.String': {
            return value.primitiveValue;
        }

        // case 'core.forJs.JsMonoBehaviorHost':
        //     return (value.objValue as CS.core.forJs.JsMonoBehaviorHost).MyJsComp();
        default: // case object
            return (value.objValue && !value.objValue.IsNull()) ? value.objValue : null;
    }
}

export function tryCall(obj: any, methodName: string, list: any) {
    const args: any[] = [];
    if (list) {
        const len = list.Length;
        for (let i = 0; i < len; ++i) {
            args.push(list.get_Item(i));
        }
    }

    const method = obj[methodName];
    if (method) {
        method.apply(obj, args);
    }
}

export function isInstanceof(obj: unknown, className: string) {
    const clazz = nameToClass.get(className);
    if (!clazz) {
        throw new Error(`Class not found: ${className}`);
    }
    return obj instanceof clazz;
}