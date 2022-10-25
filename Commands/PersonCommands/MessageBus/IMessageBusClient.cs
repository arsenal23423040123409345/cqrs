using DAL.Models;

namespace PersonCommands.MessageBus
{
    public interface IMessageBusClient
    {
        Task PublishNewPerson(Person person);
    }
}
