namespace Akka.Benchmarks.Remote.Serialization
{
    public class ComplexType
    {
        public ComplexType(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; }

        public int Age { get; }
    }
}
