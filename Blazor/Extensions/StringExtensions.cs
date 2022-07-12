namespace NameBadgeAutomator
{
  public static class StringExtensions
  {
    public static string ToSentenceCase(this String s) {
      if (s.Length == 0) return s;
      if (s.Length == 1) return s.ToUpperInvariant();

      return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
    }
  }
}