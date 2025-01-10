using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Models
{
    public enum FilterOption
    {
        // Task filters
        All,        // Alla
        Active,     // Aktiva
        Done,       // Klara

        // Priority filters
        HighPriority,    
        MediumPriority,  
        LowPriority,     

        // Time filters
        Today,          
        ThisWeek,       
        ThisMonth,      

        // Status filters
        InProgress,     
        NotStarted,     
        Delayed         
    }
}
