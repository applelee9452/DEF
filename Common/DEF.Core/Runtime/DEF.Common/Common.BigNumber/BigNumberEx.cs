
using DEF;
namespace DEF
{
    public static class BigNumberEx
    {
        public static string ToSimpleXXXC(this BigNumber number)
        {
            return number.ToString(BigNumber.FORMAT_XXXC, BigNumberLocalizator.GetSimpleDictionary("English"));
        }
    }
}