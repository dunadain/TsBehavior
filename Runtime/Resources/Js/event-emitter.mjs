import { arrayUtils } from "./array-utils.mjs";
var ListenerState;
(function (ListenerState) {
    ListenerState[ListenerState["INACTIVE"] = 0] = "INACTIVE";
    ListenerState[ListenerState["NORMAL"] = 1] = "NORMAL";
})(ListenerState || (ListenerState = {}));
export class EventEmitter {
    _listenerDic = new Map();
    once(eventName, listener, listenerObj = null) {
        this.register(eventName, listener, listenerObj, true);
    }
    /**
     *
     * @param eventName 消息类型
     * @param listener 监听
     * @param listenerObj 监听函数所属对象
     */
    on(eventName, listener, listenerObj = null) {
        this.register(eventName, listener, listenerObj, false);
    }
    register(evtName, listener, listenerObj, once) {
        if (!this._listenerDic.has(evtName))
            this._listenerDic.set(evtName, []);
        const arr = this._listenerDic.get(evtName);
        if (!arr)
            return;
        //检测是否已经存在
        let i = 0;
        const len = arr.length;
        for (i; i < len; i++) {
            const listenerData = arr[i];
            if (listenerData.fn === listener && listenerData.context === listenerObj) {
                console.error(`${evtName}'s listener has been registered more than once!`);
                return;
            }
        }
        const listenerData = {
            fn: listener,
            context: listenerObj,
            once: once,
            state: 1,
        };
        arr.push(listenerData);
    }
    /**
     * 移除监听
     * @param eventName
     * @param listener
     * @param listenerObj
     */
    off(eventName, listener, listenerObj = null) {
        const arr = this._listenerDic.get(eventName);
        if (!arr)
            return;
        const len = arr.length;
        for (let i = 0; i < len; i++) {
            const listenerData = arr[i];
            if (listenerData.fn === listener && listenerData.context === listenerObj) {
                arr.splice(i, 1);
                break;
            }
        }
        if (arr.length === 0)
            this._listenerDic.delete(eventName);
    }
    hasListeners(event) {
        return this._listenerDic.has(event);
    }
    removeAll(listenerObj) {
        this._listenerDic.forEach((v, k, map) => {
            for (let i = v.length - 1; i >= 0; --i) {
                if (v[i].context === listenerObj)
                    arrayUtils.fastRemoveAt(i, v);
            }
            if (v.length == 0)
                map.delete(k);
        });
    }
    removeByEventName(name) {
        if (this._listenerDic.has(name))
            this._listenerDic.delete(name);
    }
    emit(evtName, ...args) {
        const listeners = this._listenerDic.get(evtName);
        if (!listeners)
            return;
        const listenerClones = arrayUtils.clone(listeners);
        const len = listenerClones.length;
        for (let i = 0; i < len; ++i) {
            const listenerData = listenerClones[i];
            if (listenerData.state === ListenerState.INACTIVE)
                continue;
            try {
                listenerData.fn.apply(listenerData.context, args);
            }
            catch (e) {
                if (e instanceof Error) {
                    console.error(e.stack);
                }
                else if (typeof e === "string") {
                    console.error(e);
                }
            }
            // trigger once and to be removed
            if (listenerData.once) {
                listenerData.state = ListenerState.INACTIVE;
            }
        }
        // remove once triggered listeners
        this._listenerDic.forEach((arr) => {
            for (let i = arr.length - 1; i >= 0; --i) {
                const listenerData = arr[i];
                if (listenerData.state === ListenerState.INACTIVE)
                    arrayUtils.fastRemoveAt(i, arr);
            }
        });
        if (listeners.length <= 0)
            this._listenerDic.delete(evtName);
    }
    cleanup() {
        this._listenerDic = new Map();
    }
}
