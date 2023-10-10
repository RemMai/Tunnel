namespace Common.Proxy.Implementations
{
    public sealed class Wrap<T>
    {
        public T Value { get; set; }
        public Wrap<T> Next { get; set; }
    }
}