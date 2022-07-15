namespace CommandService.Models;

public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ExternalId { get; set; }

    public List<Command> Commands { get; set; } = new List<Command>();
}