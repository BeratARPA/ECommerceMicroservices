namespace ECommerce.Shared.Messaging
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task HandleAsync(TEvent @event);
    }
}
