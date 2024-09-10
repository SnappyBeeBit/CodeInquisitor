Console.WriteLine("Hello, World!");
MyClass myClass = new() { Id = 1 };
Console.WriteLine(myClass.Id);
MyStruct myStruct = new() { Id = 2 };
bool x = (10 == myStruct.Id) ?
    true : 


    false;
bool z = (true) ? x : false ? false : false ? x : false ? true : false;
Console.WriteLine(myStruct.Id);
class MyClass
{
    public int Id { get; set; }
}
public struct MyStruct
{
    public int Id { get; set; }
}