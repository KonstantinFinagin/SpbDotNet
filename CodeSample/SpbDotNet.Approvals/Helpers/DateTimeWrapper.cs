namespace SpbDotNet.Approvals.Helpers;

public static class DateTimeWrapper
{
    private static Func<DateTime> _utcNowProvider = () => DateTime.UtcNow;

    public static DateTime UtcNow => _utcNowProvider();

    public static void Set(Func<DateTime> customProvider)
    {
        _utcNowProvider = customProvider ?? throw new ArgumentNullException(nameof(customProvider));
    }

    public static void Reset()
    {
        _utcNowProvider = () => DateTime.UtcNow;
    }
}
