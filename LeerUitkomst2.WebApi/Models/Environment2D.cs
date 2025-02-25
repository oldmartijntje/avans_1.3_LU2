using MiNET.Utils;
using System.ComponentModel.DataAnnotations;

namespace LeerUitkomst2.WebApi.Models;

public class Environment2DTemplate
{
    public string Name { get; set; }

    [Range(10, 100)]
    public int MaxHeight { get; set; }

    [Range(20, 200)]
    public int MaxLength { get; set; }

}

/// <summary>
/// We don't want to give back the user id to the user.
/// </summary>
public class AnonymousEnvironment2D: Environment2DTemplate
{
    public int Id { get; set; }
}



public class Environment2D: AnonymousEnvironment2D
{
    public string UserId { get; set; }

}
