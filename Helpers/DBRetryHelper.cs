using Npgsql;
using Polly;

/// <summary>
/// Provides helper methods to execute database operations with retry logic.
/// Uses Polly to handle transient SQL errors (e.g., error 40613, timeout) 
/// and retries with exponential backoff.
/// </summary>
public static class DbRetryHelper
{
    public static async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, ILogger logger)
    {
        var retryPolicy = CreateRetryPolicy(logger);

        return await retryPolicy.ExecuteAsync(operation);
    }

    public static async Task ExecuteWithRetryAsync(Func<Task> operation, ILogger logger)
    {
        var retryPolicy = CreateRetryPolicy(logger);

        await retryPolicy.ExecuteAsync(operation);
    }

    /// <summary>
    /// Builds a retry policy that retries up to 3 times with exponential backoff.
    /// Logs warnings on each retry attempt.
    /// </summary>
    private static IAsyncPolicy CreateRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<PostgresException>(ex =>
                ex.SqlState == "08006" || // connection failure
                ex.SqlState == "08001" || // cannot establish connection
                ex.SqlState == "57P01" || // admin shutdown
                ex.SqlState == "53300" || // too many connections
                ex.SqlState == "40001"    // serialization failure
            )
            .Or<NpgsqlException>(ex => ex.IsTransient)
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                (exception, timespan, retryCount, context) =>
                {
                    logger.LogWarning(exception,
                        "Retry {RetryCount} after {Delay} due to transient DB error",
                        retryCount, timespan);
                });
    }

}