namespace Minimal.API;


public record Person(string FullName);
public class PeopleService
{
    private readonly List<Person> _people = new()
    {
        new Person("John Doe"),
        new Person("Jane Smith"),
        new Person("Alice Johnson")
    };

    public IEnumerable<Person> Search(string searchTerm)
    {
        return _people.Where(p => p.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
    }
}
