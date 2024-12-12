namespace Model
{
    public interface IPerson
    {
        long Id { get; set; }
        string LastName { get; set; }
        string FistName { get; set; }

        static IPerson Create() => new Person();


        class Person : IPerson
        {
            public long Id { get; set; }
            public string LastName { get; set; }
            public string FistName { get; set; }
        }
    }
}
