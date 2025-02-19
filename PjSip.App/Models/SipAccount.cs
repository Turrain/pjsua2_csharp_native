using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using PjSip.App.Sip;

namespace PjSip.App.Models
{
  public class SipAccount
  {
    public int Id { get; set; }
    public required string AccountId { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Domain { get; set; }
    public required string RegistrarUri { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public List<SipCall> Calls { get; set; } = new();
     [Required]
        public required AgentConfig Agent { get; set; }
    public SipAccount() { }

    [SetsRequiredMembers]
    public SipAccount(string accountId, string username, string password, string domain, string registrarUri, AgentConfig agent)
    {
      AccountId = accountId;
      Username = username;
      Password = password;
      Domain = domain;
      RegistrarUri = registrarUri;
         Agent = agent;
    }


  }
}