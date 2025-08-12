namespace Application.Resources
{
    public static class PdfResources
    {
        public static class ReceiptLabels
        {
            public static class Arabic
            {
                public const string Title = "إيصال تبرع";
                public const string ReceiptNumber = "رقم الإيصال";
                public const string Date = "التاريخ";
                public const string Campaign = "الحملة";
                public const string Amount = "المبلغ";
                public const string Donor = "المتبرع";
                public const string Anonymous = "مجهول";
            }

            public static class English
            {
                public const string Title = "Donation Receipt";
                public const string ReceiptNumber = "Receipt Number";
                public const string Date = "Date";
                public const string Campaign = "Campaign";
                public const string Amount = "Amount";
                public const string Donor = "Donor";
                public const string Anonymous = "Anonymous";
            }
        }

        public static string GetLocalizedText(string key, bool isArabic)
        {
            return key switch
            {
                "Title" => isArabic ? ReceiptLabels.Arabic.Title : ReceiptLabels.English.Title,
                "ReceiptNumber" => isArabic ? ReceiptLabels.Arabic.ReceiptNumber : ReceiptLabels.English.ReceiptNumber,
                "Date" => isArabic ? ReceiptLabels.Arabic.Date : ReceiptLabels.English.Date,
                "Campaign" => isArabic ? ReceiptLabels.Arabic.Campaign : ReceiptLabels.English.Campaign,
                "Amount" => isArabic ? ReceiptLabels.Arabic.Amount : ReceiptLabels.English.Amount,
                "Donor" => isArabic ? ReceiptLabels.Arabic.Donor : ReceiptLabels.English.Donor,
                "Anonymous" => isArabic ? ReceiptLabels.Arabic.Anonymous : ReceiptLabels.English.Anonymous,
                _ => string.Empty
            };
        }
    }
}
