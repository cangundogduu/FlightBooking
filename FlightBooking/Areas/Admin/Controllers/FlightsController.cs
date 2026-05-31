using FlightBooking.Dtos.FlightDtos;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlightsController(IFlightService _flightService) : Controller
    {
        public async Task<IActionResult> FlightList()
        {
            var values = await _flightService.GetAllFlightsAsync();
            return View(values);
        }


        [HttpGet]
        public IActionResult CreateFlight()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateFlight(CreateFlightDto createFlightDto)
        {
            await _flightService.CreateFlightAsync(createFlightDto);
            return RedirectToAction("FlightList");
        }
    }
}
