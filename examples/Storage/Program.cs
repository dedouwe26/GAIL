using System.Security.Cryptography;
using GAIL.Serializing.Formatters;
using GAIL.Storage;
using GAIL.Storage.Members;
using OxDED.Terminal;

// Generate a key and IV for AES encryption.
byte[] key = new byte[32];
byte[] iv = new byte[16];

RandomNumberGenerator.Fill(key);
RandomNumberGenerator.Fill(iv);

{
    // Create a new storage with a AES formatter.
    Storage storage = new(new AESFormatter(key, iv));

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
        Terminal.WriteLine("Failed to save to file...");
        return;
    }
}
{
    Storage storage = new();

    // Sets the formatter to a AES formatter.
    storage.Formatter = new AESFormatter(key, iv);
    
    // Loads the storage file.
    if (!storage.Load("./example.dat")) {
        Terminal.WriteLine("Failed to load from file...");
        return;
    }

    Terminal.WriteLine(storage.Get("MyNumber")?.Type);
    int myNumber = storage.Get<IntField>("MyNumber")!.Value;
    Terminal.WriteLine(myNumber);

    string personName = storage.Get<StringField>("person.name")!.Value;
    Terminal.WriteLine("Name: "+personName);

    int ID = storage.Get<IntField>(["person", "id"])!.Value;
    Terminal.WriteLine("ID: "+ID);

    List numbers = storage.Get<List>("numbers")!;
    foreach (IMember member in numbers) {
        Terminal.WriteLine(member.Type);
        if (member is IntField field) {
            Terminal.WriteLine(field.Value);
        }
    }
}