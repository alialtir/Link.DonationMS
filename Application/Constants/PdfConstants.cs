namespace Application.Constants
{
    public static class PdfConstants
    {
        public static class FontSettings
        {
            public const string ArabicFontFamily = "Tahoma";
            public const int TitleFontSize = 20;
            public const int NormalFontSize = 12;
        }

        public static class PageSettings
        {
            public const int PageMargin = 20;
        }

        public static class ReceiptSettings
        {
            public const int ReceiptNumberLength = 10;
            public const string DateTimeFormat = "yyyy-MM-dd HH:mm";
            public const string CurrencyFormat = "C";
        }

        public static class Languages
        {
            public const string Arabic = "ar";
            public const string English = "en";
        }
    }
}
