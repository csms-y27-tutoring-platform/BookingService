using Confluent.Kafka;
using Google.Protobuf;

namespace BookingService.Presentation.Kafka.Serializer;

public class KafkaSerializer<T> : IDeserializer<T>, ISerializer<T> where T : IMessage<T>, new()
{
    private readonly MessageParser<T> _parser = new MessageParser<T>(() => new T());

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull is false) return _parser.ParseFrom(data);
        throw new ArgumentNullException(nameof(data));
    }

    public byte[] Serialize(T data, SerializationContext context)
    {
        return data.ToByteArray();
    }
}