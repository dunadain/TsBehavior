declare namespace maple {
    class MonoBehaviorJs {
        get enabled(): boolean;
        get valid(): boolean;
        get nativeMonoBehavior(): CS.TsBehavior.JsMonoBehaviorHost | null;
        get gameObject(): CS.UnityEngine.GameObject | null;
        removeComponent(className: string | any): void;
        protected callByCSharpAwake(): void;
        protected awake?(): void;
        protected callByCSharpOnEnable(): void;
        protected onEnable?(): void;
        protected start?(): void;
        fixedUpdate?(dt: number, unscaleDt: number): void;
        protected update?(dt: number, unscaleDt: number): void;
        lateUpdate?(dt: number, unscaleDt: number): void;
        protected onDisable?(): void;
        protected onDestroy?(): void;
        protected schedule(callback: (dt?: number) => void, interval: number, repeat?: number, delay?: number): void;
        protected unschedule(callback: (dt?: number) => void): void;
        protected unscheduleAll(): void;
        delay(period: number): Promise<void>;
    }

    namespace arrayUtils {
        function fastRemoveAt<T>(i: number, arr: T[]): T | undefined;
        function clone<T>(source: T[], out?: T[]): T[];
    }
}

declare function property(_type: PropertyType): any;
declare function listproperty(_type: PropertyType): any;
declare function mapleclass(className: string): any;
declare function enumfield(enumType: any): any;