using System;
using System.ComponentModel.DataAnnotations;

namespace ChatApi.Models
{
 public class Message
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ChatId { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Sender { get; set; }
    
    [Required]
    public string Content { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
}
