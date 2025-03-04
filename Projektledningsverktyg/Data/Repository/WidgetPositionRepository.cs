using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Projektledningsverktyg.Data.Repository
{
    public class WidgetPositionRepository
    {
        private readonly ApplicationDbContext _context;

        public WidgetPositionRepository()
        {
            _context = new ApplicationDbContext();
        }

        /// <summary>
        /// Hämtar alla widget-positioner för en specifik användare
        /// </summary>
        public List<WidgetPosition> GetWidgetPositionsForMember(int memberId)
        {
            return _context.WidgetPositions
                .Where(wp => wp.MemberId == memberId)
                .ToList();
        }

        /// <summary>
        /// Hämtar en specifik widget-position för en användare
        /// </summary>
        public WidgetPosition GetWidgetPosition(int memberId, string widgetId)
        {
            return _context.WidgetPositions
                .FirstOrDefault(wp => wp.MemberId == memberId && wp.WidgetId == widgetId);
        }

        /// <summary>
        /// Uppdaterar eller skapar en widget-position
        /// </summary>
        public void SaveWidgetPosition(WidgetPosition position)
        {
            var existingPosition = _context.WidgetPositions
                .FirstOrDefault(wp => wp.MemberId == position.MemberId && wp.WidgetId == position.WidgetId);

            if (existingPosition != null)
            {
                // Uppdatera befintlig position
                existingPosition.RowIndex = position.RowIndex;
                existingPosition.ColumnIndex = position.ColumnIndex;
                existingPosition.IsVisible = position.IsVisible;
                existingPosition.LastUpdated = DateTime.Now;
            }
            else
            {
                // Skapa ny position
                position.LastUpdated = DateTime.Now;
                _context.WidgetPositions.Add(position);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Sparar flera widget-positioner på en gång
        /// </summary>
        public void SaveWidgetPositions(List<WidgetPosition> positions)
        {
            foreach (var position in positions)
            {
                SaveWidgetPosition(position);
            }
        }
    }
}
