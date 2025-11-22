namespace milk.Core;

public static class Theme {
    public static int consoleOutputLength = 40;
    public static int valuePosition = 15;
    public static string CreateConsoleTitle(string title) {
        string output = "";
        output += title + " ";
        output += new string('-', consoleOutputLength - output.Length);
        output += "\n";
        return output;
    }
    public static string PrintConsoleVar(string name, string value) {
        string output = " ";
        output += name + ":";
        output += new string(' ', valuePosition - output.Length);
        output += value + "\n";
        return output;
    }
}