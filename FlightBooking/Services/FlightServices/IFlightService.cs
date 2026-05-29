using FlightBooking.Dtos.FlightDtos;

namespace FlightBooking.Services.FlightServices
{
    public interface IFlightService
    {
            Task<List<ResultFlightDto>> GetAllFlightsAsync();
            Task<GetFlightByIdDto> GetFlightByIdAsync(string id);
            Task CreateFlightAsync(CreateFlightDto createFlightDto);
            Task UpdateFlightAsync(UpdateFlightDto updateFlightDto);
            Task DeleteFlightAsync(string id);
    }
}
