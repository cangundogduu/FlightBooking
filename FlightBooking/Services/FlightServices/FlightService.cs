using AutoMapper;
using FlightBooking.Dtos.FlightDtos;
using FlightBooking.Dtos.PassengerDtos;
using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

namespace FlightBooking.Services.FlightServices
{
    public class FlightService: IFlightService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Flight> _flightCollection;
        private readonly IMongoCollection<Booking> _bookingCollection;

        public FlightService(IMapper mapper, IDatabaseSettings _databaseSettings)
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _flightCollection = database.GetCollection<Flight>(_databaseSettings.FlightCollectionName);
            _bookingCollection = database.GetCollection<Booking>(_databaseSettings.BookingCollectionName);
            _mapper = mapper;
        }

        
        public async Task CreateFlightAsync(CreateFlightDto createFlightDto)
        {
            var value = _mapper.Map<Flight>(createFlightDto);
            await _flightCollection.InsertOneAsync(value);
        }

        public async Task DeleteFlightAsync(string id)
        {
            await _flightCollection.DeleteOneAsync(x => x.FlightId == id);
        }

        public async Task<List<ResultFlightDto>> GetAllFlightsAsync()
        {
            var values = await _flightCollection.Find(x => true).ToListAsync();
            var result = _mapper.Map<List<ResultFlightDto>>(values);
            return result;
        }

        public async Task<GetFlightByIdDto> GetFlightByIdAsync(string id)
        {
            var value =await _flightCollection.Find(x => x.FlightId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetFlightByIdDto>(value);
        }

        public async Task<List<PassengerListItemDto>> GetFlightDetailsWithPassengersAsync(string flightId)
        {
            // 1. O uçuşa ait tüm booking'leri çek
            var bookings = await _bookingCollection.Find(x => x.FlightId == flightId).ToListAsync();

            // 2. Her booking içindeki yolcuları düzleştir ve DTO'ya map et
            var passengers = bookings
                .SelectMany(b => b.Passengers.Select(p => new PassengerListItemDto
                {
                    Name = p.Name,
                    Surname = p.Surname,
                    Email = b.ContactEmail,   // yolcuya ait email yoksa iletişim emaili kullan
                    Gender = p.Gender,
                    PassengerType = p.PassengerType,
                    PnrNumber = b.PnrNumber,       // PNR olarak BookingId kullanılıyor
                    Phone = b.ContactPhone,
                    // Aşağıdaki alanlar Passenger entity'nde varsa doğrudan al
                    SeatNumber = p.SeatNumber,
                    CheckInStatus = p.CheckInStatus,
                    // PaymentStatus = b.PaymentStatus,
                    TicketStatus = p.TicketStatus,
                    PassengerId = p.PassengerId
                }))
                .ToList();

            return passengers;
        }

        public async Task UpdateFlightAsync(UpdateFlightDto updateFlightDto)
        {
            var value = _mapper.Map<Flight>(updateFlightDto);
            await _flightCollection.ReplaceOneAsync(x => x.FlightId == updateFlightDto.FlightId, value);
        }
    }
}
