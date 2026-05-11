namespace MuseSpace.Core.Entities;

public sealed class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty; // User, Creator, Admin
    public string Description { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}
