using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TMSA.Data;
using TMSA.Data.Repositories;
using TMSA.Domain;
using TMSA.Models;

namespace TMSA.Controllers
{
    public class EventsBrowserController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IClientRepository _clientRepository;
        public EventsBrowserController(IEventRepository eventRepository, IBookingRepository bookingRepository,
            IClientRepository clientRepository)
        {
            _eventRepository = eventRepository;
            _bookingRepository = bookingRepository;
            _clientRepository = clientRepository;
        }
        public IActionResult Index(int id)
        {
            Event eventInfo = _eventRepository.Get(e => e.Id == id).SingleOrDefault();
            if (eventInfo is null)
            {
                return Json("Such data cannot be retrieved!");
            }
            var bookings = _bookingRepository.Get(b => b.Event.Id == eventInfo.Id);
            foreach (var item in bookings)
            {
                if (item.Client is null)
                {
                    item.Client = _clientRepository.Get(c => c.GetBookings.Contains(item)).Single();
                }
            }
            var model = new EventsBrowserViewModel()
            {
                Bookings = bookings,
                Event = eventInfo
            };
            return View(model);
        }
    }
}
