using GAIL.Storage;
using GAIL.Storage.Members;

{
    Storage storage = new();

    // Creates a new field.
    _ = new IntField("MyNumber", 1248, storage);

    // Creates a new container.
    Container person = new("person", storage);

    // Creates a new field in 'person'.
    _ = new StringField("name", "0xDED", person);

    // Creates a new field.
    IntField ID = new("id", Random.Shared.Next());
    // Adds the field to the container.
    person.AddChild(ID);
    // ID.SetParent(person);

    // Creates a list.
    List numbers = new("numbers", storage);

    // Populates the list.
    const int amount = 5;
    for (int i = 0; i < amount; i++) {
        // Adds a int field to the list (can be anything). Key is ignored.
        numbers.Add(new IntField("", Random.Shared.Next())); 
    }

    // Saves the storage to a file.
    if (!storage.Save("./example.dat")) {
        Console.WriteLine("Failed to save to file...");
    }
}
{
    Storage storage = new();
    
    // Loads the storage file.
    storage.Load("./example.dat");

    Console.WriteLine(storage.Get("MyNumber")?.Type);
    int myNumber = storage.Get<IntField>("MyNumber")!.Value;
    Console.WriteLine(myNumber);

    string personName = storage.Get<StringField>("person.name")!.Value;
    Console.WriteLine("Name: "+personName);

    int ID = storage.Get<IntField>(["person", "id"])!.Value;
    Console.WriteLine("ID: "+ID);

    List numbers = storage.Get<List>("numbers")!;
    foreach (IMember member in numbers) {
        Console.WriteLine(member.Type);
        if (member is IntField field) {
            Console.WriteLine(field.Value);
        }
    }
}