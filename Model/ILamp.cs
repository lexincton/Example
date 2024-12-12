namespace Model
{
    public interface ILamp : IDevice
    {
        static ILamp Create() => new Lamp() ;

        class Lamp : ILamp
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}
