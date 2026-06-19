using System.Text.RegularExpressions;

namespace PruebaPayment.Utils;

public static partial class RegExUtils
{
    [GeneratedRegex(@"^\d{13,19}$")]
    public static partial Regex CardNumberRegex();

    [GeneratedRegex(@"^(0[1-9]|1[0-2])\/\d{2}$")]
    public static partial Regex ExpiryDateRegex();

    [GeneratedRegex(@"^\d{3,4}$")]
    public static partial Regex CvvRegex();
}
