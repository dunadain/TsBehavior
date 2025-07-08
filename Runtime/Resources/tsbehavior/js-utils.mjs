export function property(_type) {
    return (_target, _propName) => {
        // if (!target['__properties']) {
        //     target['__properties'] = {};
        // }
        // target['__properties'][propName] = {
        //     type: $typeof(type)
        // };
    };
}
export function listproperty(_type) {
    return (_target, _propName) => {
        // if (!target['__propertyLists']) {
        //     target['__propertyLists'] = {};
        // }
        // target['__propertyLists'][propName] = {
        //     type: $typeof(type)
        // };
    };
}
export function enumfield(enumType) {
    return (target, propName) => {
        if (!target['__enumFields']) {
            target['__enumFields'] = {};
        }
        target['__enumFields'][propName] = enumType;
    };
}
/**
 * 不用decorator factory的话，minify后类名全变了
 */
const nameToClass = new Map();
export function mapleclass(className) {
    return function (constructor) {
        if (nameToClass.has(className)) {
            console.error(`duplicate class: ${className}`);
        }
        nameToClass.set(className, constructor);
    };
}
export function createMonoJs(className, nativeMonoBehavior) {
    const constructor = nameToClass.get(className);
    if (constructor) {
        const jsComp = new constructor();
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
                    if (enumType[value] === undefined) {
                        value = 0;
                    }
                    else
                        value = enumType[value];
                }
                jsComp[prop.name] = value;
                if (prop.typeName === 'TsBehavior.JsMonoBehaviorHost') {
                    const jsHost = jsComp[prop.name];
                    if (jsHost && !jsHost.IsNull())
                        defineGet(jsComp, prop.name, jsHost);
                }
            }
        }
        if (nativeMonoBehavior.listProperties) {
            const size = nativeMonoBehavior.listProperties.Count;
            for (let i = 0; i < size; ++i) {
                const prop = nativeMonoBehavior.listProperties.get_Item(i);
                if (prop.typeName !== 'TsBehavior.JsMonoBehaviorHost')
                    jsComp[prop.name] = convertToListValue(prop);
                else {
                    defineListGet(jsComp, prop);
                }
            }
        }
        return jsComp;
    }
}
function defineGet(obj, propName, jsHost) {
    Object.defineProperty(obj, propName, {
        get() {
            const js = jsHost.MyJsComp();
            if (!js)
                return null;
            Object.defineProperty(this, propName, {
                value: js,
                configurable: true
            });
            return js;
        },
        configurable: true
    });
}
function defineListGet(obj, listProp) {
    Object.defineProperty(obj, listProp.name, {
        get() {
            const arr = convertToListValue(listProp);
            for (let i = 0; i < arr.length; ++i) {
                arr[i] = arr[i].MyJsComp();
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
function convertToListValue(listProp) {
    const list = listProp.list;
    const arr = [];
    const size = list.Count;
    for (let i = 0; i < size; ++i) {
        const propV = list.get_Item(i);
        arr.push(convertValue(listProp.typeName, propV));
    }
    return arr;
}
function convertValue(typeName, value) {
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
            return new CS.UnityEngine.Vector2(arr.length > 0 ? Number(arr[0]) : 0, arr.length > 1 ? Number(arr[1]) : 0);
        }
        case 'UnityEngine.Vector3': {
            const arr = value.primitiveValue.split(',');
            return new CS.UnityEngine.Vector3(arr.length > 0 ? Number(arr[0]) : 0, arr.length > 1 ? Number(arr[1]) : 0, arr.length > 2 ? Number(arr[2]) : 0);
        }
        case 'UnityEngine.Color': {
            const arr = value.primitiveValue.split(',');
            return new CS.UnityEngine.Color(arr.length > 0 ? Number(arr[0]) : 0, arr.length > 1 ? Number(arr[1]) : 0, arr.length > 2 ? Number(arr[2]) : 0, arr.length > 3 ? Number(arr[3]) : 1);
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
export function tryCall(obj, methodName, list) {
    const args = [];
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
function isInstanceof(obj, className) {
    const clazz = nameToClass.get(className);
    if (!clazz) {
        throw new Error(`Class not found: ${className}`);
    }
    return obj instanceof clazz;
}
Object.defineProperty(CS.UnityEngine.GameObject.prototype, 'getJsComponent', {
    value: function (clazz, includesDescendants = false) {
        let self = this;
        const hosts = self.GetMonoBehaviourHosts();
        let len = hosts.Count;
        for (let i = 0; i < len; ++i) {
            const host = hosts.get_Item(i);
            if (host && !host.IsNull()) {
                const jsComp = host.JsComponent();
                if (jsComp) {
                    if (host.jsClassName === clazz)
                        return jsComp;
                    if (includesDescendants && isInstanceof(jsComp, clazz))
                        return jsComp;
                }
            }
        }
        return null;
    },
    writable: false,
    enumerable: false,
    configurable: true
});
Object.defineProperty(CS.UnityEngine.GameObject.prototype, 'getJsComponents', {
    value: function (clazz, includesDescendants = false) {
        let arr = [];
        let self = this;
        const hosts = self.GetMonoBehaviourHosts();
        let len = hosts.Count;
        for (let i = 0; i < len; ++i) {
            const host = hosts.get_Item(i);
            if (host && !host.IsNull()) {
                const jsComp = host.JsComponent();
                if (jsComp) {
                    if (host.jsClassName === clazz || (includesDescendants && isInstanceof(jsComp, clazz))) {
                        arr.push(jsComp);
                    }
                }
            }
        }
        return arr;
    },
    writable: false,
    enumerable: false,
    configurable: true
});
