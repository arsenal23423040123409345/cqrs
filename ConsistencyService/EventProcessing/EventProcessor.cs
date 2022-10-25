using DAL;
using DAL.Models;
using System.Text.Json;

namespace ConsistencyService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessor(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task ProcessEventAsync(string message)
            => AddPerson(message);


        private async Task AddPerson(string personPublishedMessage)
        {
            var context = _scopeFactory.CreateScope().ServiceProvider.GetService<AppDbContext>();
            var person = JsonSerializer.Deserialize<Person>(personPublishedMessage);

            try
            {
                if (person is not null)
                {
                    await context.Persons.AddAsync(person);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"--> Could not add person to DB: {e.Message}");
            }
        }
    }
}
