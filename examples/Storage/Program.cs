using System.Security.Cryptography;
using GAIL.Serializing;
using GAIL.Serializing.Formatters;
using GAIL.Storage;
using GAIL.Storage.Hierarchy;
using GAIL.Storage.Members;
using LambdaKit.Terminal;

// NOTE: Aliases are used to make the code look better.
using IntField = GAIL.Storage.Members.BasicField<int>;
using StringField = GAIL.Storage.Members.BasicField<string>;

// Generate a key and IV for AES encryption.
byte[] key = new byte[32];
byte[] iv = new byte[16];

RandomNumberGenerator.Fill(key);
RandomNumberGenerator.Fill(iv);

{
	// There are 2 types of storages:
	// 1. LookupStorage, uses a lookup table, supports custom fields.
	// 2. SimpleStorage, is more lightweight, doesn't support custom fields.

	// Instantiates an new lookup storage.
	using LookupStorage storage = new();

	// Creates an int field and adds it to the storage.
	storage.AddChild(IntField.Create("MyNumber", MemberType.Int, 1248));

	// Creates a new container.
	Container person = new("person", storage);
	// Containers can contain more fields with keys.

	// Creates a string field in the 'person' container.
	StringField.Create("name", person, MemberType.String, "0xDED");

	// Creates an int field, but without the BasicField.Create utility.
	IntField id = new(
		"id", person,
		MemberType.Int, new IntSerializable(Random.Shared.Next())
	);
	// Removes the id.
	person.RemoveChild(id);
	// There are a lot of methods you can use to manipulate the hierarchy. 
	id.SetParent(person);

	// Creates a list of random numbers.
	IntField[] raw = new IntField[5];
	for (int i = 0; i < raw.Length; i++) {
		raw[i] = IntField.Create(MemberType.Int,  Random.Shared.Next());
	}
	
	// Creates an list field based on the previously generated numbers.
	ListField<IntField>.Create("numbers", storage, MemberType.Int, raw);
	// List fields can be used when you have multiple fields of the SAME type
	// and the keys do NOT need to be stored.

	// Storages can use formatters for compression or encryption.
	storage.Formatter = new AESFormatter(key, iv);

	// Saves the storage to a file.
	if (!storage.Save("./example.dat")) {
	    Terminal.WriteErrorLine("Failed to save to file...");
		Environment.Exit(1);
		return;
	}
}
{
	using LookupStorage storage = new();

	// Sets the formatter to a AES formatter.
	storage.Formatter = new AESFormatter(key, iv);

	// Loads the storage file.
	if (!storage.Load("./example.dat")) {
	    Terminal.WriteErrorLine("Failed to load from file...");
		Environment.Exit(1);
	    return;
	}

	// Examples of accessing a field.
	Terminal.WriteLine(((IParentNode)storage).Get<IntField>(["MyNumber"])!.Type);
	if (storage.TryGetValue(["MyNumber"], out int value)) {
		Terminal.WriteLine("Value: " + value);
	}

	Terminal.WriteLine("Name: " + storage.GetValueOrDefault<string>(["person", "name"])!);

	int ID = ((IParentNode)storage).Get<IntField>(["person", "id"])!.Value;
	Terminal.WriteLine("ID: "+ID);

	ListField<IField<int>> numbers = (ListField<IField<int>>)storage.Get<IField<int>>(["numbers"])!;
	foreach (IChildNode member in numbers) {
	    if (member is IntField field) {
	        Terminal.WriteLine(field.Value);
	    }
	}
}