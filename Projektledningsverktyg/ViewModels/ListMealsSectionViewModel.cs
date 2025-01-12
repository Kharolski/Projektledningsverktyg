using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data.Entity;
using System.Windows.Input;
using Projektledningsverktyg.Commands;

namespace Projektledningsverktyg.ViewModels
{
    public class ListMealsSectionViewModel : ViewModelBase
    {
        private ObservableCollection<Meal> _meals;
        public static event Action MealsUpdated;

        #region Properties

        public ObservableCollection<Meal> Meals
        {
            get => _meals;
            set
            {
                _meals = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeleteMealCommand { get; private set; }
        public ICommand SaveNotesCommand { get; private set; }

        #endregion

        #region Constractor

        public ListMealsSectionViewModel()
        {
            LoadMeals();
            DeleteMealCommand = new RelayCommand<Meal>(ExecuteDeleteMeal);
            SaveNotesCommand = new RelayCommand<Meal>(ExecuteSaveNotes);

        }

        #endregion

        #region Load

        private void LoadMeals()
        {
            using (var context = new ApplicationDbContext())
            {
                var mealsList = context.Meals
                    .Include(m => m.Ingredients)
                    .ToList();
                Meals = new ObservableCollection<Meal>(mealsList);
            }
        }

        public void RefreshMeals()
        {
            LoadMeals();
        }

        public static void NotifyMealsUpdated()
        {
            System.Diagnostics.Debug.WriteLine("NotifyMealsUpdated called");
            MealsUpdated?.Invoke();
            System.Diagnostics.Debug.WriteLine("NotifyMealsUpdated completed");
        }

        #endregion

        #region Delete

        private void ExecuteDeleteMeal(Meal meal)
        {
            using (var context = new ApplicationDbContext())
            {
                var mealToDelete = context.Meals.Include(m => m.Ingredients).FirstOrDefault(m => m.Id == meal.Id);
                if (mealToDelete != null)
                {
                    context.Meals.Remove(mealToDelete);
                    context.SaveChanges();
                    RefreshMeals();
                }
            }
        }

        #endregion

        #region Notes

        private void ExecuteSaveNotes(Meal meal)
        {
            using (var context = new ApplicationDbContext())
            {
                var mealToUpdate = context.Meals.Find(meal.Id);
                if (mealToUpdate != null)
                {
                    mealToUpdate.Notes = meal.Notes;
                    context.SaveChanges();
                    RefreshMeals();
                }
            }
        }

        #endregion

    }

}
