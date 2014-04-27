namespace Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Retry routine in case a method is failed
    /// This is usefule when API connection closed due to unstable environment
    /// </summary>
    public static class Retry
    {
        public static void Do(Action action, TimeSpan retryInterval, int maxRetry = 3)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, maxRetry);
        }

        public static TResult Do<TResult>(Func<TResult> func, TimeSpan retryInterval, int maxRetry = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < maxRetry; retry++)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}
