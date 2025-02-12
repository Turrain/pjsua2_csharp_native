using System;

namespace PjSip.App.Utils
{
    public class CircuitBreaker
    {
        private readonly int _failureThreshold;
        private readonly TimeSpan _resetTimeout;
        private int _failureCount;
        private DateTime? _lastFailureTime;
        private readonly object _lock = new();

        public enum State
        {
            Closed,      // Normal operation
            Open,        // Failing, not accepting requests
            HalfOpen    // Testing if service is back to normal
        }

        public State CurrentState { get; private set; } = State.Closed;

        public CircuitBreaker(int failureThreshold, TimeSpan resetTimeout)
        {
            _failureThreshold = failureThreshold;
            _resetTimeout = resetTimeout;
        }

        public bool CanExecute()
        {
            lock (_lock)
            {
                if (CurrentState == State.Closed)
                    return true;

                if (CurrentState == State.Open && 
                    _lastFailureTime.HasValue && 
                    DateTime.UtcNow - _lastFailureTime.Value > _resetTimeout)
                {
                    CurrentState = State.HalfOpen;
                    return true;
                }

                return CurrentState == State.HalfOpen;
            }
        }

        public void OnSuccess()
        {
            lock (_lock)
            {
                _failureCount = 0;
                CurrentState = State.Closed;
            }
        }

        public void OnFailure()
        {
            lock (_lock)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                if (_failureCount >= _failureThreshold)
                {
                    CurrentState = State.Open;
                }
                else if (CurrentState == State.HalfOpen)
                {
                    CurrentState = State.Open;
                }
            }
        }

        public void Reset()
        {
            lock (_lock)
            {
                _failureCount = 0;
                _lastFailureTime = null;
                CurrentState = State.Closed;
            }
        }
    }
}
