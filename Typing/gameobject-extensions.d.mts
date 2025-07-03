// GameObject extensions - Custom methods added via defineProperty
declare namespace CS.UnityEngine {
    interface GameObject {
        getJsComponent<T>(clazz: string, includesDescendants?: boolean): T | null;
        getJsComponents<T>(clazz: string, includesDescendants?: boolean): T[];
    }
}