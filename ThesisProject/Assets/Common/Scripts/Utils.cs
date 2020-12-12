using System.Text;

public static class Utils {

    public static void ClearConsole() {
        // Clear Unity's Log+Error console
        System.Type logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
        System.Reflection.MethodInfo clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }

    public static string ArrayToString<T>(T[] array) {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < array.Length; i++) {
            sb.Append($"{array[i]} ");
        }

        return sb.ToString();
    }
}