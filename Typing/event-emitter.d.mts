type Listener = (...args: unknown[]) => void;
export declare class EventEmitter {
    private _listenerDic;
    once(eventName: string | number, listener: Listener, listenerObj?: unknown): void;
    /**
     *
     * @param eventName 消息类型
     * @param listener 监听
     * @param listenerObj 监听函数所属对象
     */
    on(eventName: string | number, listener: Listener, listenerObj?: unknown): void;
    private register;
    /**
     * 移除监听
     * @param eventName
     * @param listener
     * @param listenerObj
     */
    off(eventName: string | number, listener: Listener, listenerObj?: unknown): void;
    hasListeners(event: string | number): boolean;
    removeAll(listenerObj: unknown): void;
    removeByEventName(name: string | number): void;
    emit(evtName: string | number, ...args: unknown[]): void;
    cleanup(): void;
}
export {};
