using GAIL.Storage;

Storage storage = new();

Container exampleContainer = new("container1");
storage.AddChild(exampleContainer);

IntField exampleField = new("a field(1)", 42);
storage.AddChild(exampleField);

StringField subField = new("childField", "hello, world!");
storage.AddChild(subField);
subField.SetParent(exampleContainer);

storage.Save("./example.dat");

Storage storage2 = new();

storage2.Load("./example.dat");

IParentNode parentNode = storage2;

StringField? child;
if ((child = storage2.Get<StringField>("container1.childField")) == null) {
    Console.WriteLine("No child found.");
    return;
}