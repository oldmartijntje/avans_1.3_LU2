using System.ComponentModel.DataAnnotations;

namespace LeerUitkomst2.WebApi.Models;

public class Environment2D
{
    public int Id { get; set; }

    public string Name { get; set; }

    [Range(10, 100)]
    public int MaxHeight { get; set; }

    [Range(20, 200)]
    public int MaxLength { get; set; }

}
