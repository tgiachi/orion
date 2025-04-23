namespace Orion.Irc.Core.Filters;

/// <summary>
/// Represents filters for the IRC LIST command.
/// </summary>
public class ListQueryFilter
{
    /// <summary>
    /// Type of filter to apply.
    /// </summary>
    public enum FilterType
    {
        /// <summary>
        /// No filter specified.
        /// </summary>
        None,

        /// <summary>
        /// Filter by user count.
        /// </summary>
        UserCount,

        /// <summary>
        /// Filter by channel creation time.
        /// </summary>
        CreationTime,

        /// <summary>
        /// Filter by topic update time.
        /// </summary>
        TopicTime
    }

    /// <summary>
    /// Type of comparison operator.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// Greater than operator.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Less than operator.
        /// </summary>
        LessThan,

        /// <summary>
        /// Equal to operator.
        /// </summary>
        Equal
    }

    /// <summary>
    /// Type of filter to apply.
    /// </summary>
    public FilterType Type { get; set; } = FilterType.None;

    /// <summary>
    /// Comparison operator to use.
    /// </summary>
    public ComparisonOperator Operator { get; set; } = ComparisonOperator.Equal;

    /// <summary>
    /// Value to compare against.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Checks if the filter is valid and applicable.
    /// </summary>
    public bool IsValid => Type != FilterType.None;

    /// <summary>
    /// Parses a query string and creates a ListQueryFilter object.
    /// </summary>
    /// <param name="query">The query string (e.g., ">3", "C>60", "T<60")</param>
    /// <returns>A filter for the LIST command or a default filter if the query is not valid.</returns>
    public static ListQueryFilter Parse(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return new ListQueryFilter(); // No filter

        var filter = new ListQueryFilter();

        // Check for special prefixes C and T
        if (query.StartsWith("C", StringComparison.OrdinalIgnoreCase))
        {
            filter.Type = FilterType.CreationTime;
            query = query.Substring(1);
        }
        else if (query.StartsWith("T", StringComparison.OrdinalIgnoreCase))
        {
            filter.Type = FilterType.TopicTime;
            query = query.Substring(1);
        }
        else
        {
            filter.Type = FilterType.UserCount;
        }

        // Check for operator
        if (query.StartsWith(">"))
        {
            filter.Operator = ComparisonOperator.GreaterThan;
            query = query.Substring(1);
        }
        else if (query.StartsWith("<"))
        {
            filter.Operator = ComparisonOperator.LessThan;
            query = query.Substring(1);
        }
        else if (query.StartsWith("="))
        {
            filter.Operator = ComparisonOperator.Equal;
            query = query.Substring(1);
        }
        else
        {
            // If no explicit operator, assume "="
            filter.Operator = ComparisonOperator.Equal;
        }

        // Extract the numeric value
        if (int.TryParse(query, out int value))
        {
            filter.Value = value;
            return filter;
        }

        // If we can't extract a value, return an invalid filter
        return new ListQueryFilter();
    }

    /// <summary>
    /// Checks if a channel meets the conditions specified by the filter.
    /// </summary>
    /// <param name="userCount">The number of users in the channel.</param>
    /// <param name="creationTimeMinutes">The time since the channel was created, in minutes.</param>
    /// <param name="topicTimeMinutes">The time since the topic was last updated, in minutes.</param>
    /// <returns>True if the channel matches the filter criteria, False otherwise.</returns>
    public bool Matches(int userCount, int creationTimeMinutes, int topicTimeMinutes)
    {
        if (Type == FilterType.None)
        {
            return true;
        }


        int valueToCompare = Type switch
        {
            FilterType.UserCount => userCount,
            FilterType.CreationTime => creationTimeMinutes,
            FilterType.TopicTime => topicTimeMinutes,
            _ => 0
        };

        return Operator switch
        {
            ComparisonOperator.GreaterThan => valueToCompare > Value,
            ComparisonOperator.LessThan => valueToCompare < Value,
            ComparisonOperator.Equal => valueToCompare == Value,
            _ => false
        };
    }
}
