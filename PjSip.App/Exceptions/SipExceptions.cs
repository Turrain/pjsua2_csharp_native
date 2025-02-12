namespace PjSip.App.Exceptions;

public class SipException : Exception
{
    public string ErrorCode { get; }
    public string CorrelationId { get; }

    public SipException(string message, string errorCode, string correlationId = null, Exception innerException = null) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
    }
}
  public class AIProcessingException : SipException
    {
        public AIProcessingException(string message, string errorCode, Exception innerException)
            : base(message, errorCode, null, innerException)
        {
        }
    }
public class SipRegistrationException : SipException
{
    public string AccountId { get; }
    public int StatusCode { get; }

    public SipRegistrationException(string message, string accountId, int statusCode, Exception innerException = null)
        : base(message, "REG_ERROR", null, innerException)
    {
        AccountId = accountId;
        StatusCode = statusCode;
    }
}

public class SipCallException : SipException
{
    public int CallId { get; }
    public string CallState { get; }

    public SipCallException(string message, int callId, string callState, Exception innerException = null)
        : base(message, "CALL_ERROR", null, innerException)
    {
        CallId = callId;
        CallState = callState;
    }
}

public class MediaOperationException : SipException
{
    public int CallId { get; }
    public string Operation { get; }

    public MediaOperationException(string message, int callId, string operation, Exception innerException = null)
        : base(message, "MEDIA_ERROR", null, innerException)
    {
        CallId = callId;
        Operation = operation;
    }
}

public class TransportException : SipException
{
    public string TransportType { get; }
    public int Port { get; }

    public TransportException(string message, string transportType, int port, Exception innerException = null)
        : base(message, "TRANSPORT_ERROR", null, innerException)
    {
        TransportType = transportType;
        Port = port;
    }
}

public class DatabaseOperationException : SipException
{
    public string Operation { get; }
    public string Entity { get; }

    public DatabaseOperationException(string message, string operation, string entity, Exception innerException = null)
        : base(message, "DB_ERROR", null, innerException)
    {
        Operation = operation;
        Entity = entity;
    }
}
