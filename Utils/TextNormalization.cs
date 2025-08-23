namespace ResumeAi.Api.Utils
{
    public class TextNormalization
    {
        public static string Normalize(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            s = s.ToLowerInvariant();
            
            return System.Text.RegularExpressions.Regex.Replace(s, @"\s+", " ").Trim();
        }
    }
}
