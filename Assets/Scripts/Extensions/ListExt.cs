using System.Collections.Generic;

namespace Extensions {
    public static class ListExt {
        public static void AddIfNewAndNotNull<T>(this IList<T> list, T element) {
            if (element == null) {
                return;
            }

            if (list.Contains(element)) {
                return;
            }

            list.Add(element);
        }

        public static bool IsNullOrEmpty<T>(this IList<T> list) {
            return (list is null || list.Count == 0);
        }

        public static bool IsFilled<T>(this IList<T> list) {
            if (list is null || list.Count == 0)
                return false;
            return true;
        }
    }
}