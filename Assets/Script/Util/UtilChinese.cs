using Microsoft.International.Converters.PinYinConverter;
using System.Text;


internal class UtilChinese
{
    public static string GetPinYins(string Ch)
    {
        StringBuilder pingying = new StringBuilder();
        foreach (char ch in Ch)
        {
            if (ChineseChar.IsValidChar(ch))
            {
                ChineseChar str = new ChineseChar(ch);
                pingying.Append(str.Pinyins[0].Substring(0, str.Pinyins[0].Length - 1));
            }
            else { pingying.Append(ch); }
        }
        char[] c = pingying.ToString().ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] >= 'A' && c[i] <= 'Z')
                c[i] = (char)(c[i] + 32);
        }
        return new string(c);
    }

    public static string ConveExit(string str)
    {
        switch (str)
        {
            case "东":
                return "east";
            case "西":
                return "west";
            case "北":
                return "north";
            case "南":
                return "south";
            case "东南":
                return "eastsouth";
            case "东北":
                return "eastnorth";
            case "西南":
                return "westsouth";
            case "西北":
                return "westnorth";
            default:
                return str;
        }
    }
}

