namespace RedisMessaging.Util
{
  internal static class StringExtensions
  {
    public static string ToCamelCase(this string input)
    {
      return $"{input.Substring(0, 1).ToLowerInvariant()}{input.Substring(1)}";
    }
  }
}