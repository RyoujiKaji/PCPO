using ApartmentRentals.Data.DTOs;
using ApartmentRentals.Data.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;
using kafka.KafkaProducer;
using System.Runtime.InteropServices.ObjectiveC;
using ApartmentRentals.WebAPI.Services.Repositories;

namespace ApartmentRentals.WebAPI.kafka
{
    public class UserService
    {
        private readonly MongoDbRepository<User> _userRepository;
        private readonly KafkaProducer _producer;
        private readonly KafkaProducerSettings _producerSettings;
        public UserService(MongoDbRepository<User> userRepository, KafkaProducer producer, IOptions<KafkaProducerSettings> producerSettings) {
            _userRepository = userRepository;
            _producer = producer;
            _producerSettings = producerSettings.Value;
        }

        public async Task registerObject(RegistrationObjectDTO reg) {
            var userId = reg.AuthorId;
            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null) {
                user.registered_objects++;
                await _userRepository.UpdateAsync(userId, user);

                await _producer.SendAsync(JsonSerializer.Serialize(new ResponseDTO
                {
                    UserId = userId,
                    Type = reg.Type,
                    ObjectId = reg.Id
                }));
            }
        }
    }
}