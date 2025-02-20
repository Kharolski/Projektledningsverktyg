using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;
using System.Reflection;
using Projektledningsverktyg.Data.Entities;

namespace Projektledningsverktyg.Helpers
{
    public static class EnumDisplayHelper
    {
        public static IEnumerable<string> GetDisplayNames()
        {
            return Enum.GetValues(typeof(TaskPriority))
                .Cast<TaskPriority>()
                .Select(e => GetDisplayName(e));
        }

        public static string GetDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
