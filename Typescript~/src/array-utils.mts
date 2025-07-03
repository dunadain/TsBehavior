class ArrayUtils {
    fastRemoveAt<T>(i: number, arr: T[]) {
        if (i < 0) return undefined;
        if (arr.length > 0) {
            const lastIndex = arr.length - 1;
            if (i !== lastIndex) {
                const tmp = arr[lastIndex];
                arr[lastIndex] = arr[i];
                arr[i] = tmp;
            }

            return arr.pop();
        }
    }

    clone<T>(source: T[], out?: T[]) {
        if (!out) out = [];
        const len = source.length;
        for (let i = 0; i < len; ++i) {
            out.push(source[i]);
        }
        return out;
    }
}

export const arrayUtils = new ArrayUtils();