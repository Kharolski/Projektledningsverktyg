using Newtonsoft.Json;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Data.SeedData
{
    public class RecipeImporter
    {
        private readonly string _jsonFilePath;
        private readonly ApplicationDbContext _dbContext;

        public RecipeImporter(string jsonFilePath, ApplicationDbContext dbContext)
        {
            _jsonFilePath = jsonFilePath;
            _dbContext = dbContext;
        }

        public async System.Threading.Tasks.Task ImportRecipesAsync()
        {
            // Kontrollera om vi redan har recept i databasen
            if (_dbContext.Recipes.Any())
            {
                Console.WriteLine("Databasen innehåller redan recept. Import avbröts.");
                return;
            }

            if (!File.Exists(_jsonFilePath))
            {
                Console.WriteLine($"Hittade inte receptfilen på sökvägen: {_jsonFilePath}");
                return;
            }

            try
            {
                string jsonContent = File.ReadAllText(_jsonFilePath, System.Text.Encoding.UTF8);
                var recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsonContent);

                if (recipes == null || !recipes.Any())
                {
                    Console.WriteLine("Inga recept hittades i JSON-filen eller formatet är ogiltigt.");
                    return;
                }

                // Skapa Images/Recipes-mappen om den inte finns
                string appImagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Recipes");
                Directory.CreateDirectory(appImagesDir);

                // Lägg till recept i databasen
                foreach (var recipe in recipes)
                {
                    if (!string.IsNullOrEmpty(recipe.ImagePath))
                    {
                        // Få filnamnet från sökvägen
                        string fileName = Path.GetFileName(recipe.ImagePath);

                        // Källfilen i SeedData
                        string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", recipe.ImagePath);

                        // Målfilen i applikationsmappen
                        string destPath = Path.Combine(appImagesDir, fileName);

                        if (File.Exists(sourcePath))
                        {
                            // Kopiera filen om den inte redan finns
                            if (!File.Exists(destPath))
                            {
                                File.Copy(sourcePath, destPath);
                            }

                            // Uppdatera sökvägen i receptet
                            recipe.ImagePath = Path.Combine("Images", "Recipes", fileName);
                        }
                        else
                        {
                            recipe.ImagePath = null;
                            Console.WriteLine($"Varning: Hittade inte bildfilen för {recipe.Name}: {sourcePath}");
                        }
                    }

                    _dbContext.Recipes.Add(recipe);
                }

                await _dbContext.SaveChangesAsync();
                Console.WriteLine($"Importerade {recipes.Count} recept till databasen.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel uppstod vid importering av recept: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
