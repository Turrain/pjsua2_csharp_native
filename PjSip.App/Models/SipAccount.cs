public class SipAccount
{
    public int Id { get; set; }
    public string AccountId { get; set; }  // Unique SIP identifier
    public string Username { get; set; }
    public string Password { get; set; }
    public string Domain { get; set; }
    public string RegistrarUri { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public List<SipCall> Calls { get; set; } = new();
}