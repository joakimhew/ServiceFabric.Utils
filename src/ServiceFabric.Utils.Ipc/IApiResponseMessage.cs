namespace ServiceFabric.Utils.Ipc
{
    public interface IApiResponseMessage<out TCode>
    {
        TCode Code { get; }
        object Message { get; }
        object AdditionalInfo { get; }
    }
}