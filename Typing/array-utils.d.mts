declare class ArrayUtils {
    fastRemoveAt<T>(i: number, arr: T[]): T | undefined;
    clone<T>(source: T[], out?: T[]): T[];
}
export declare const arrayUtils: ArrayUtils;
export {};
