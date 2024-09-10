Console.WriteLine("Hello, World!");
MyClass myClass = new() { Id = 1 };
Console.WriteLine(myClass.Id);
MyStruct myStruct = new() { Id = 2 };

Console.WriteLine(myStruct.Id);
class MyClass
{
    public int Id { get; set; }
}
public struct MyStruct
{
    public int Id { get; set; }
}