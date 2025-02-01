using Projektledningsverktyg.Data.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Projektledningsverktyg.Data.Entities
{
    public enum MealTypes
    {
        Frukost,
        Lunch,
        Middag,
        Efterrätt,
        Mellanmål
    }

    public enum MainIngredients
    {
        Kött,
        Fisk,
        Kyckling,
        Pasta,
        Vegetariskt,
        Vegansk
    }

    public class Recipe : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }  // Information om receptet
        public MealTypes MealType { get; set; }
        public MainIngredients MainIngredient { get; set; }
        public int CookingTime { get; set; }
        public int Servings { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Instruction> Instructions { get; set; }

        // Load ImagePath and caches the image in memory
        private string _imagePath;
        [NotMapped]
        public BitmapImage Image { get; private set; }
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                Image = LoadImage(_imagePath);
            }
        }
        private BitmapImage LoadImage(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;

            var bitmap = new BitmapImage();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Gör att filen frikopplas direkt
                bitmap.StreamSource = new MemoryStream(File.ReadAllBytes(path)); // Läser in bilden i minnet
                bitmap.EndInit();
            }
            return bitmap;
        }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get => _isFavorite;
            set
            {
                _isFavorite = value;
                OnPropertyChanged(nameof(IsFavorite));
            }
        }
        public DateTime CreatedDate { get; set; }

        public Recipe()
        {
            Ingredients = new List<Ingredient>();
            Instructions = new List<Instruction>();
            CreatedDate = DateTime.Now;
        }

        public bool IsInMealPlan
        {
            get
            {
                var today = DateTime.Today;
                var weekFromNow = today.AddDays(7);

                using (var context = new ApplicationDbContext())
                {
                    return context.Meals.Any(m =>
                        m.Name == this.Name &&
                        m.Date >= today &&
                        m.Date <= weekFromNow);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
