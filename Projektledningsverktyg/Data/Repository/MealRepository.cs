using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Projektledningsverktyg.Data.Repository
{
    public class MealRepository
    {
        private readonly ApplicationDbContext _context;

        public MealRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hämta alla måltider med ingredienser och instruktioner
        public IEnumerable<Meal> GetAllMeals()
        {
            return _context.Meals
                .Include(m => m.Ingredients)
                .Include(m => m.Instructions)
                .ToList();
        }

        // Hämta måltider för ett specifikt datum
        public IEnumerable<Meal> GetMealsByDate(System.DateTime date)
        {
            // Skapa start- och slutdatum för hela dagen
            var startDate = date.Date;
            var endDate = startDate.AddDays(1).AddSeconds(-1);

            // Hämta alla måltider för debuggning
            var allMeals = _context.Meals.ToList();

            var mealsForDate = _context.Meals
                .Include(m => m.Ingredients)
                .Include(m => m.Instructions)
                .Where(m => m.Date >= startDate && m.Date <= endDate)
                .ToList();

            return mealsForDate;
        }

        // Lägg till ny måltid med ingredienser och instruktioner
        public void AddMeal(Meal newMeal)
        {
            // Se till att relationer är korrekt konfigurerade
            foreach (var ingredient in newMeal.Ingredients)
            {
                ingredient.MealId = newMeal.Id;
            }

            foreach (var instruction in newMeal.Instructions)
            {
                instruction.MealId = newMeal.Id;
            }

            _context.Meals.Add(newMeal);
            _context.SaveChanges();
        }

        // Hämta måltid med ID
        public Meal GetMealById(int id)
        {
            return _context.Meals
                .Include(m => m.Ingredients)
                .Include(m => m.Instructions)
                .FirstOrDefault(m => m.Id == id);
        }

        // Uppdatera måltid
        public void UpdateMeal(Meal mealToUpdate)
        {
            _context.Entry(mealToUpdate).State = EntityState.Modified;

            // Uppdatera ingredienser
            foreach (var ingredient in mealToUpdate.Ingredients)
            {
                if (ingredient.Id == 0)
                {
                    _context.Entry(ingredient).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(ingredient).State = EntityState.Modified;
                }
            }

            // Uppdatera instruktioner
            foreach (var instruction in mealToUpdate.Instructions)
            {
                if (instruction.Id == 0)
                {
                    _context.Entry(instruction).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(instruction).State = EntityState.Modified;
                }
            }

            _context.SaveChanges();
        }

        // Ta bort måltid och dess relaterade ingredienser och instruktioner
        public void DeleteMeal(int mealId)
        {
            var meal = _context.Meals
                .Include(m => m.Ingredients)
                .Include(m => m.Instructions)
                .FirstOrDefault(m => m.Id == mealId);

            if (meal != null)
            {
                // Ta bort relaterade ingredienser
                foreach (var ingredient in meal.Ingredients.ToList())
                {
                    _context.Set<MealIngredient>().Remove(ingredient);
                }

                // Ta bort relaterade instruktioner
                foreach (var instruction in meal.Instructions.ToList())
                {
                    _context.Set<MealInstruction>().Remove(instruction);
                }

                // Ta bort själva måltiden
                _context.Meals.Remove(meal);
                _context.SaveChanges();
            }
        }
    }
}
