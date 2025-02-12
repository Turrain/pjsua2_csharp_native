using System;
using System.Threading.Tasks;

namespace PjSip.App.Utils
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly int _delayMs;

        public RetryPolicy(int maxRetries, int delayMs)
        {
            _maxRetries = maxRetries;
            _delayMs = delayMs;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            var attempts = 0;
            while (true)
            {
                try
                {
                    attempts++;
                    return await operation();
                }
                catch (Exception) when (attempts < _maxRetries)
                {
                    await Task.Delay(_delayMs * attempts); // Exponential backoff
                    continue;
                }
            }
        }

        public async Task ExecuteAsync(Func<Task> operation)
        {
            await ExecuteAsync(async () =>
            {
                await operation();
                return true;
            });
        }
    }
}
