namespace Restore.Tests
{
    public class LocalTestResource
    {

        public LocalTestResource(int correlationId, int localId) : this(localId)
        {
            CorrelationId = correlationId;
        }

        public LocalTestResource(int localId)
        {
            LocalId = localId;
        }

        public int? CorrelationId { get; }

        public int LocalId { get; }

        public string Name { get; set; }
    }
}