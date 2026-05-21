using System.ComponentModel.DataAnnotations;

namespace RetailProcurement.Core.DTOs.Supplier;

public class CreateSupplierDto
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(200)]
    public string ContactPerson { get; set; } = string.Empty;
}
