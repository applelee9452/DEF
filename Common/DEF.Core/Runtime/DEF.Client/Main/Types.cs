#if DEF_CLIENT

namespace DEF.Client
{
    public enum Platform
    {
        Android = 0,
        iOS,
        PC
    }

    public enum Language
    {
        English,// 英语，默认语言
        Estonian,
        Faroese,
        Finnish,
        French,
        German,
        Greek,
        Hebrew,
        Hugarian,
        Hungarian,
        Icelandic,
        Indonesian,// 印尼语
        Italian,
        Japanese,// 日语
        Korean,
        Latvian,
        Lithuanian,
        Mongolian,// 蒙古语
        Norwegian,
        Polish,
        Portuguese,
        Romanian,
        Russian,
        SerboCroatian,
        Slovak,
        Slovenian,
        Spanish,
        Swedish,
        Thai,
        Turkish,// 土耳其语
        ChineseSimplified,// 中文简体
        ChineseTraditional,// 中文繁体
        Unknown
    }

    // 缩写：[AF]非洲, [EU]欧洲, [AS]亚洲, [OA]大洋洲, [NA]北美洲, [SA]南美洲, [AN]南极洲
    public enum Continent
    {
        Africa,// 非洲
        Europe,// 欧洲
        Asia,// 亚洲
        NorthAmerica,// 北美洲
        SouthAmerica,// 南美洲
        Oceania,// 大洋洲
        Antarctica,// 南极洲
    }
}

#endif
