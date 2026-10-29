using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;


public class User
{
    // pk
    [Key]
    public int Id { get; set; }

    // id de conexion
    [Required]
    public string Login { get; set; } = string.Empty;

    // role de l'user
    public RoleEnum Role { get; set; }
}