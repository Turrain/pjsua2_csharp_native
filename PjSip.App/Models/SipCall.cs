public class SipCall
{
    public int Id { get; set; }
    public int CallId { get; set; }  // PJSUA2 call ID
    public string RemoteUri { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public string Status { get; set; }
    public int SipAccountId { get; set; }
    public SipAccount Account { get; set; }
}