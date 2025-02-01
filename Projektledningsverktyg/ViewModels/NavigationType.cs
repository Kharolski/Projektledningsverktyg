using System;

namespace Projektledningsverktyg.ViewModels
{
    [Flags]
    public enum NavigationType
    {
        // Base value for showing all recipes (no filters)
        AllRecipes = 0,

        // Individual filter states, using powers of 2 for unique bit positions
        Favorites = 1,        // Binary: 0001
        RecentlyAdded = 2,    // Binary: 0010
        MealType = 4,         // Binary: 0100       
        MainIngredient = 8    // Binary: 1000
    }
}
