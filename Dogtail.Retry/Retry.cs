namespace Dogtail.Retry;

/// <summary>
/// Retry a function if it throws an Exception
/// </summary>
public static class Retry
{
    /// <summary>
    /// Retries the function if it throws an exception.
    /// </summary>
    /// <typeparam name="T">The type the function returns.</typeparam>
    /// <param name="func">The function to run.</param>
    /// <param name="evaluateError">Decides if the error should be rethrown immediately or if new attempts should be made. If <see langword="true"/> is returned the exception will be rethrown immediately without any atempt to execute the function again. If <see langword="false"/> is returned the function will be executed again if not the maxAttemptCount has been reached.</param>
    /// <param name="retryInterval">Number of seconds between retries.</param>
    /// <param name="maxAttemptCount">The maximum number of retry attempts. Defaults to three.</param>
    /// <returns>The result of the function.</returns>
    public static T Do<T>(Func<T> func, Func<Exception, bool> evaluateError, TimeSpan retryInterval, int maxAttemptCount = 3)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                attempt++;
                if (attempt == maxAttemptCount || evaluateError(ex))
                {
                    throw;
                }

                Thread.Sleep(retryInterval);
            }
        }
    }

    /// <summary>
    /// Retries the function if it throws an exception.
    /// </summary>
    /// <typeparam name="T">The type the function returns.</typeparam>
    /// <param name="func">The function to run.</param>
    /// <param name="evaluateError">Decides if the error should be rethrown immediately or if new attempts should be made. If <see langword="true"/> is returned the exception will be rethrown immediately without any atempt to execute the function again. If <see langword="false"/> is returned the function will be executed again if not the maxAttemptCount has been reached.</param>
    /// <param name="retryInterval">Number of seconds between retries.</param>
    /// <param name="maxAttemptCount">The maximum number of retry attempts. Defaults to three.</param>
    /// <returns>The result of the function.</returns>
    public static async Task<T> DoAsync<T>(Func<Task<T>> func, Func<Exception, bool> evaluateError, TimeSpan retryInterval, int maxAttemptCount = 3)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                attempt++;
                if (attempt == maxAttemptCount || evaluateError(ex))
                {
                    throw;
                }

                await Task.Delay(retryInterval);
            }
        }
    }
}
