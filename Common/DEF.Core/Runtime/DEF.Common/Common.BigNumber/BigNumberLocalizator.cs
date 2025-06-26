namespace DEF
{
    public static class BigNumberLocalizator
    {
        private static IBigNumberDictionary simpleDictionary;
        private static string currentLanguage;

        private static void DefineSimpleDictionary(string language)
        {
            currentLanguage = language;
            switch (language)
            {
                case "English":
                    simpleDictionary = new BigNumberDictionaryEN();
                    break;
                case "Russian":
                    simpleDictionary = new BigNumberDictionaryRU();
                    break;
                default:
                    simpleDictionary = new BigNumberDictionaryEN();
                    break;
            }
        }

        public static IBigNumberDictionary GetSimpleDictionary(string language = "English")
        {
            if (simpleDictionary == null || language != currentLanguage)
            {
                DefineSimpleDictionary(language);
            }

            return simpleDictionary;
        }
    }
}