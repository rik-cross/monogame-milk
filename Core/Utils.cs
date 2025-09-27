using System.Collections;
using System.Collections.Generic;

namespace milk;

public static class Utils {

    public static bool CompareBitArrays(BitArray ba1, BitArray ba2) {
        if (ba1.Count != ba2.Count) {
            return false;
        }
        for (int i=0; i<ba1.Count; i++) {
            if (ba1.Get(i) != ba2.Get(i)) {
                return false;
            }
        }
        return true;
    }
    public static string PrintList<T>(List<T> list, string separator = "") {
        if (list.Count == 0) {
            return "[none]";
        }
        string output = "";
        foreach (T value in list) {
            output += value.ToString();
            if (list.IndexOf(value) != list.Count - 1) {
                output += separator;
            }
        }
        return output;
    }
    public static string PrintBitArray(BitArray bitArray) {
        string output = "";
        for (int i=0; i<bitArray.Length; i++) {
            if (bitArray.Get(i) == true)
                output += "1";
            else
                output += "0";
        }
        return output;
    }
}