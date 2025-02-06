using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Helpers
{
    public enum DragDropType
    {
        Schedule,    // For schedule-related items
        Meals,       // For meal planning items
        Household,   // For household tasks
        GeneralTask, // For general tasks and todos
        Events       // For calendar events
    }
}
