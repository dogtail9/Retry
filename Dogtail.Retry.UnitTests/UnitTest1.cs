using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Dogtail.Retry.UnitTests;

[TestClass]
public class RetryTests
{
    [TestMethod]
    public void RunAFunctionThatWorks()
    {
        // Arrange
        int expected = 0;
        int expectedNumberOfErrors = 0;
        int numberOfErrors = 0;
        int maxNumberOfRetries = 4;
        TimeSpan retryDelay = TimeSpan.FromSeconds(1);

        Func<int> functionToRun = () =>
        {
            return expected;
        };

        Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
        {
            return true;
        };

        // Act
        int result = Retry.Do(functionToRun, errorEvaluationFunction, retryDelay, maxNumberOfRetries);

        // Assert
        Assert.AreEqual(expected, result);
        Assert.AreEqual(expectedNumberOfErrors, numberOfErrors);
    }

    [TestMethod]
    public void RunAFunctionThatFails()
    {
        // Arrange
        int expectedNumberOfErrors = 4;
        int numberOfErrors = 0;
        int maxNumberOfRetries = 4;
        TimeSpan retryDelay = TimeSpan.FromSeconds(1);

        Func<int> functionToRun = () =>
        {
            numberOfErrors++;
            throw new Exception();
        };

        Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
        {
            return false;
        };

        // Act
        Assert.ThrowsException<Exception>(() => Retry.Do(functionToRun, errorEvaluationFunction, retryDelay, maxNumberOfRetries));

        // Assert
        Assert.AreEqual(expectedNumberOfErrors, numberOfErrors);
    }

    [TestMethod]
    public void RunAFunctionThatFailsTwoTimesThenWorksOnTheThirdTry()
    {
        // Arrange
        int expected = 0;
        int expectedNumberOfErrors = 2;
        int numberOfErrors = 0;
        int maxNumberOfRetries = 4;
        TimeSpan retryDelay = TimeSpan.FromSeconds(1);

        Func<int> functionToRun = () =>
        {
            if (numberOfErrors < 2)
            {
                numberOfErrors++;
                throw new UnexpectedTestException();
            }

            return expected;
        };


        Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
        {
            switch (ex)
            {
                case UnexpectedTestException testExceptionOne:
                    return false;

                case ExpectedTestException testExceptionOne:
                    return true;

                default:
                    return false;
            }
        };

        // Act
        int result = Retry.Do(functionToRun, errorEvaluationFunction, retryDelay, maxNumberOfRetries);

        // Assert
        Assert.AreEqual(expectedNumberOfErrors, numberOfErrors);
    }

    [TestMethod]
    public async Task RunAFunctionThatWorksAsync()
    {
        // Arrange
        int expected = 0;
        int expectedNumberOfErrors = 0;
        int numberOfErrors = 0;
        int maxNumberOfRetries = 4;
        TimeSpan retryDelay = TimeSpan.FromSeconds(1);

        Func<Task<int>> functionToRun = () =>
        {
            return Task.FromResult(expected);
        };

        Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
        {
            return true;
        };

        // Act
        int result = await Retry.DoAsync(functionToRun, errorEvaluationFunction, retryDelay, maxNumberOfRetries);

        // Assert
        Assert.AreEqual(expected, result);
        Assert.AreEqual(expectedNumberOfErrors, numberOfErrors);
    }

    [TestMethod]
    public async Task RunAFunctionThatFailsAsync()
    {
        // Arrange
        int expectedNumberOfErrors = 4;
        int numberOfErrors = 0;
        int maxNumberOfRetries = 4;
        TimeSpan retryDelay = TimeSpan.FromSeconds(1);

        Func<Task<int>> functionToRun = () =>
        {
            numberOfErrors++;
            throw new Exception();
        };

        Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
        {
            return false;
        };

        // Act
        await Assert.ThrowsExceptionAsync<Exception>(async () => await Retry.DoAsync(functionToRun, errorEvaluationFunction, retryDelay, maxNumberOfRetries));

        // Assert
        Assert.AreEqual(expectedNumberOfErrors, numberOfErrors);
    }

    [TestMethod]
    public async Task RunAFunctionThatFailsTwoTimesThenWorksOnTheThirdTryAsync()
    {
        // Arrange
        int expected = 0;
        int expectedNumberOfErrors = 2;
        int numberOfErrors = 0;
        int maxNumberOfRetries = 4;
        TimeSpan retryDelay = TimeSpan.FromSeconds(1);

        Func<Task<int>> functionToRun = () =>
        {
            if (numberOfErrors < 2)
            {
                numberOfErrors++;
                throw new UnexpectedTestException();
            }

            return Task.FromResult(expected);
        };


        Func<Exception, bool> errorEvaluationFunction = (Exception ex) =>
        {
            switch (ex)
            {
                case UnexpectedTestException testExceptionOne:
                    return false;

                case ExpectedTestException testExceptionOne:
                    return true;

                default:
                    return false;
            }
        };

        // Act
        int result = await Retry.DoAsync(functionToRun, errorEvaluationFunction, retryDelay, maxNumberOfRetries);

        // Assert
        Assert.AreEqual(expectedNumberOfErrors, numberOfErrors);
        Assert.AreEqual(expected, result);
    }
}

public class ExpectedTestException : Exception { }

public class UnexpectedTestException : Exception { }

