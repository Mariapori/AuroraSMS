
namespace AuroraSMS
{
    public class AuroraSmsMessage
    {
        public int Id { get; set; }
        public string Message { get; set; } = null!;
        public string To { get; set; } = null!;
        public MessageStatus Status { get; set; } = MessageStatus.Unknown;
    }

    public enum MessageStatus
    {
        UnSent,
        Sent,
        Unknown
    }
}
