using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PjSip.App.Models
{

    public class SipCall
    {
        public int Id { get; set; }
        public required int CallId { get; set; }
        public required string RemoteUri { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public required string Status { get; set; }
        public int SipAccountId { get; set; }
        [JsonIgnore]
    public SipAccount Account { get; set; }

        public SipCall() { }

        [SetsRequiredMembers]
        public SipCall(int callId, string remoteUri, string status, SipAccount account)
        {
            CallId = callId;
            RemoteUri = remoteUri;
            Status = status;
            Account = account;
            SipAccountId = account.Id;
        }
    }
}