using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiProject.Models;

[Table("suppliers")]
public class Supplier
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = null!;
    [Column("contact_person")]
    public string? ContactPerson { get; set; }
    [Column("phone")]
    public string? Phone { get; set; }
    [Column("address")]
    public string? Address { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
