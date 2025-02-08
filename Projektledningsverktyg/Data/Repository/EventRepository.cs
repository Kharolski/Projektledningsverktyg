using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace Projektledningsverktyg.Data.Repository
{
    public class EventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all events with participants and project info
        public IEnumerable<Event> GetAllEvents()
        {
            return _context.Events
                .Include(e => e.Participants)
                .Include(e => e.Project)
                .ToList();
        }

        // Add new event
        public void AddEvent(Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
        }

        // Add participant to event
        public void AddParticipant(int eventId, Member participant)
        {
            var eventEntity = _context.Events.Find(eventId);
            eventEntity.Participants.Add(participant);
            _context.SaveChanges();
        }

        // Remove participant from event
        public void RemoveParticipant(int eventId, Member participant)
        {
            var eventEntity = _context.Events.Find(eventId);
            eventEntity.Participants.Remove(participant);
            _context.SaveChanges();
        }

        // Delete event
        public void DeleteEvent(int eventId)
        {
            var eventEntity = _context.Events.Find(eventId);
            _context.Events.Remove(eventEntity);
            _context.SaveChanges();
        }
    }
}
