using System.Text.RegularExpressions;

namespace Orion.Foundations.Extensions;

/// <summary>
/// Provides extension methods for replacing environment variables in strings.
/// </summary>
public static class EnvReplacerMethodEx
{
    /// <summary>
    /// Regular expression that matches text enclosed in curly braces {variable}.
    /// </summary>
    private static readonly Regex EnvReplaceRegex = new(@"\{(.*?)\}");

    /// <summary>
    /// Replaces environment variable placeholders in a string with their actual values.
    /// </summary>
    /// <param name="value">The string containing environment variable placeholders in the format {VARIABLE_NAME}.</param>
    /// <returns>
    /// A string with all environment variable placeholders replaced with their corresponding values.
    /// If an environment variable is not found or is empty, the placeholder remains unchanged.
    /// </returns>
    /// <remarks>
    /// The method searches for patterns like {VARIABLE_NAME} in the input string and replaces them
    /// with the value of the corresponding environment variable if it exists and is not empty.
    /// </remarks>
    /// <example>
    /// Input: "Hello, {USERNAME}! Your home directory is {HOME}."
    /// Output (example): "Hello, John! Your home directory is /home/john."
    /// 
    /// If the environment variable does not exist, the placeholder remains unchanged:
    /// Input: "The value is {NONEXISTENT_VARIABLE}."
    /// Output: "The value is {NONEXISTENT_VARIABLE}."
    /// </example>
    public static string ReplaceEnvVariable(this string value)
    {
        var matches = EnvReplaceRegex.Matches(value);
        for (var count = 0; count < matches.Count; count++)
        {
            if (matches[count].Groups.Count != 2)
            {
                continue;
            }

            var env = Environment.GetEnvironmentVariable(matches[count].Groups[1].Value) ?? "";
            if (!string.IsNullOrEmpty(env))
            {
                value = value.Replace(matches[count].Value, env);
            }
        }

        return value;
    }
}