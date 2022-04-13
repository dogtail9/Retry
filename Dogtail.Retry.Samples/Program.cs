using Dogtail.Retry;

int maxNumberOfRetries = 4;
TimeSpan retryDelay = TimeSpan.FromSeconds(1);
int numberOfRetries = 0;

Func<int> functionToRun = () =>
{
    if (numberOfRetries < 2)
    {
        numberOfRetries++;
        throw new ExpectedException();
    }

    return 1;
};

Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
{
    switch (ex)
    {
        case ExpectedException testExceptionOne:
            return true;

        case UnexpectedException testExceptionOne:
            return false;

        default:
            return false;
    }
};

try
{
    int result = Retry.Do(MethodToRun, ErrorEvaluationMethod, retryDelay, maxNumberOfRetries);
    Console.WriteLine($"Result: {result}, Number of errors: {numberOfRetries}");
}
catch (Exception ex)
{
    Console.WriteLine($"Number of retries: {numberOfRetries}");
    Console.WriteLine(ex);
}


static int MethodToRun()
{
    return 1;
}

static bool ErrorEvaluationMethod(Exception ex)
{
    switch (ex)
    {
        case ExpectedException testExceptionOne:
            return true;

        case UnexpectedException testExceptionOne:
            return false;

        default:
            return true;
    }
}

public class ExpectedException : Exception { }

public class UnexpectedException : Exception { }