Console.WriteLine("Hello, World!");
MyClass myClass = new() { Id = 1 };
Console.WriteLine(myClass.Id);
MyStruct myStruct = new() { Id = 2 };
var x = true ? 1 : 2;
Console.WriteLine(myStruct.Id);
class MyClass
{
    public int Id { get; set; }
    public static string DoTheThing()
    {
        return (true ? "" : true && true || false ? "a" : 
            false && false ? "true" : "hello");
    }
}
public struct MyStruct
{
    public int Id { get; set; }
}