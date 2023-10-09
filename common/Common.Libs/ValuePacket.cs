namespace Common.Libs
{
    public sealed class ValuePacket<T> where T : struct
    {
        public T Value { get; set; }
    }
}
