namespace Weekly.Serializers
{
    public interface IWorkLogSerializer
    {
        string Version { get; }
        bool CanDeserialize(string serializedWorkLog);
        string Serialize(WorkLog workLog);
        WorkLog Deserialize(string serializedWorkLog);
    }
}
