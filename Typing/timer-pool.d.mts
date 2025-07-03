export declare class Timer {
    valid: boolean;
    delay: number;
    repeat: number;
    interval: number;
    target: any;
    callback: ((dt?: number) => void) | null;
    private _counter;
    private _delayPassed;
    update(dt: number): void;
    reset(): void;
}
export declare class TimerPool {
    private static Pool;
    static get(): Timer;
    static recycle(timer: Timer): void;
}
