using Projektledningsverktyg.Commands;
using Projektledningsverktyg.Data.Context;
using Projektledningsverktyg.Data.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Forms;

using Application = System.Windows.Application;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Projektledningsverktyg.Views.RecipeBook.Components;
using Projektledningsverktyg.Views.RecipeBook.Windows;


namespace Projektledningsverktyg.ViewModels
{
    public class RecipeViewModel : ViewModelBase
    {
        private readonly ApplicationDbContext _context;
        private Recipe _recipe;
        private readonly Ingredient _ingredient;
        private string _currentInstruction;
        private Instruction _selectedInstruction;
        private bool _isEditing;        // For editing instructions
        private bool _isEditMode;       // For editing recipe
        public event Action RecipeAdded;
        public object DataContext { get; set; }

        public string AddButtonText => IsEditing ? "Spara ändring" : "Lägg till";               // For editing instructions
        public string WindowTitle => IsEditMode ? "Redigera Recept" : "Lägg till Recept";       // For editing recipe
        public string SaveButtonText => IsEditMode ? "Spara ändringar" : "Spara recept";        // For editing recipe
        public Recipe CurrentRecipe => _recipe;


        private void InitializeCommands()
        {
            SelectImageCommand = new RelayCommand(ExecuteSelectImage);

            AddIngredientCommand = new RelayCommand(ExecuteAddIngredient);
            RemoveIngredientCommand = new RelayCommand<Ingredient>(ExecuteRemoveIngredient);

            AddInstructionCommand = new RelayCommand(ExecuteAddInstruction);
            RemoveInstructionCommand = new RelayCommand<Instruction>(ExecuteRemoveInstruction);

            EditInstructionCommand = new RelayCommand<Instruction>(ExecuteEditInstruction);
            CancelEditCommand = new RelayCommand<Instruction>(ExecuteCancelEdit);

            SaveRecipeCommand = new RelayCommand(ExecuteSaveRecipe);
            EditRecipeCommand = new RelayCommand<Recipe>(ExecuteEditRecipe);

        }

        #region Properties

        // For editing recipe
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(WindowTitle));
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        // Recipe properties
        public string Name
        {
            get => _recipe.Name;
            set
            {
                _recipe.Name = value;
                OnPropertyChanged(nameof(NameValidationMessage));
                OnPropertyChanged();
            }
        }
        public MealTypes MealType
        {
            get => _recipe.MealType;
            set
            {
                _recipe.MealType = value;
                OnPropertyChanged();
            }
        }

        public MainIngredients MainIngredient
        {
            get => _recipe.MainIngredient;
            set
            {
                _recipe.MainIngredient = value;
                OnPropertyChanged();
            }
        }

        public int CookingTime
        {
            get => _recipe.CookingTime;
            set
            {
                _recipe.CookingTime = value;
                OnPropertyChanged(nameof(CookingTimeValidationMessage));
                OnPropertyChanged();
            }
        }

        public int Servings
        {
            get => _recipe.Servings;
            set
            {
                _recipe.Servings = value;
                OnPropertyChanged(nameof(ServingsValidationMessage));
                OnPropertyChanged();
            }
        }

        public string ImagePath
        {
            get => _recipe.ImagePath;
            set
            {
                _recipe.ImagePath = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _recipe.Description;
            set
            {
                _recipe.Description = value;
                OnPropertyChanged();
            }
        }

        // Ingredients property
        public decimal Amount
        {
            get => _ingredient.Amount;
            set
            {
                _ingredient.Amount = value;
                OnPropertyChanged();
            }
        }

        public Units Unit
        {
            get => _ingredient.Unit;
            set
            {
                _ingredient.Unit = value;
                OnPropertyChanged();
            }
        }

        public string IngredientName
        {
            get => _ingredient.Name;
            set
            {
                _ingredient.Name = value;
                OnPropertyChanged();
            }
        }

        // Instructions property
        public string CurrentInstruction
        {
            get => _currentInstruction;
            set
            {
                _currentInstruction = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddButtonText));
            }
        }

        // Steps for TextBox field
        public int NextStepNumber
        {
            get => Instructions.Count + 1;
        }
        public bool IsLastStep(int stepNumber)
        {
            return stepNumber == Instructions.Count;
        }


        public ObservableCollection<Ingredient> Ingredients { get; set; }
        public ObservableCollection<Instruction> Instructions { get; set; }

        #endregion

        #region Commands

        // Base
        public ICommand SelectImageCommand { get; private set; }

        // Ingredient
        public ICommand AddIngredientCommand { get; private set; }
        public ICommand RemoveIngredientCommand { get; private set; }

        // Add/Edit/Cancel/Delete instruction
        public ICommand AddInstructionCommand { get; private set; }
        public ICommand RemoveInstructionCommand { get; private set; }
        public ICommand EditInstructionCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }

        // Save recipe
        public ICommand SaveRecipeCommand { get; private set; }
        // Update recipe
        public ICommand EditRecipeCommand { get; private set; }

        #endregion

        #region Constructor

        public RecipeViewModel(ApplicationDbContext context)
        {
            _context = context;
            _recipe = new Recipe();
            _ingredient = new Ingredient();

            Ingredients = new ObservableCollection<Ingredient>();
            Instructions = new ObservableCollection<Instruction>();

            Instructions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(NextStepNumber));


            InitializeCommands();
        }


        #endregion

        #region Load
        public void LoadRecipeForEdit(Recipe recipe)
        {
            _recipe = recipe;
            Ingredients = new ObservableCollection<Ingredient>(recipe.Ingredients);
            Instructions = new ObservableCollection<Instruction>(recipe.Instructions);
            IsEditMode = true;  // Set edit mode for recipe

            // Notify all properties
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(CookingTime));
            OnPropertyChanged(nameof(Servings));
            OnPropertyChanged(nameof(MealType));
            OnPropertyChanged(nameof(MainIngredient));
            OnPropertyChanged(nameof(ImagePath));
        }



        #endregion

        #region Image

        private void ExecuteSelectImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png",
                Title = "Välj en bild"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _recipe.ImagePath = openFileDialog.FileName;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        private string SaveImage(string sourcePath)
        {
            // Base folders setup
            string imagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            string recipeImagesFolder = Path.Combine(imagesFolder, "Recept");
            string recipeFolder = Path.Combine(recipeImagesFolder, _recipe.Id.ToString());

            // Create necessary directories
            Directory.CreateDirectory(imagesFolder);
            Directory.CreateDirectory(recipeImagesFolder);
            Directory.CreateDirectory(recipeFolder);

            // Delete all existing images in recipe folder
            foreach (string file in Directory.GetFiles(recipeFolder, "image.*"))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(file);
            }

            // Handle only the image file
            string fileName = $"image{Path.GetExtension(sourcePath)}";
            string destinationPath = Path.Combine(recipeFolder, fileName);

            // Save new image using FileStream
            using (FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            using (FileStream destStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                sourceStream.CopyTo(destStream);
            }

            // Return relative path for database
            return Path.Combine("Images", "Recept", _recipe.Id.ToString(), fileName);
        }

        #endregion

        #region Add

        private void ExecuteAddIngredient()
        {
            var ingredient = new Ingredient
            {
                Amount = Amount,
                Unit = Unit,
                Name = IngredientName
            };

            Ingredients.Add(ingredient);

            // Reset fields
            Amount = 0;
            Unit = Units.st;
            IngredientName = string.Empty;

            // Update validation message
            OnPropertyChanged(nameof(IngredientsValidationMessage));
        }

        private void ExecuteAddInstruction()
        {
            if (IsEditing)
            {
                _selectedInstruction.Description = CurrentInstruction;
                IsEditing = false;

                // Force UI update
                var index = Instructions.IndexOf(_selectedInstruction);
                Instructions.RemoveAt(index);
                Instructions.Insert(index, _selectedInstruction);
            }
            else
            {
                var instruction = new Instruction
                {
                    StepNumber = Instructions.Count + 1,
                    Description = CurrentInstruction
                };
                Instructions.Add(instruction);
            }
            CurrentInstruction = string.Empty;
            OnPropertyChanged(nameof(InstructionsValidationMessage));
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new ApplicationDbContext())
            {
                var recipeVM = new RecipeViewModel(context);
                recipeVM.RecipeAdded += () =>
                {
                    var recipeBookVM = DataContext as RecipeBookViewModel;
                    recipeBookVM?.LoadRecipes();
                };

                var window = new Window
                {
                    Content = new RecipeDetailsView { DataContext = recipeVM },
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Width = 550,
                    SizeToContent = SizeToContent.Height
                };

                window.ShowDialog();
            }
        }

        #endregion

        #region Remove

        private void ExecuteRemoveIngredient(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
            OnPropertyChanged(nameof(IngredientsValidationMessage));
        }

        private void ExecuteRemoveInstruction(Instruction instruction)
        {
            int removedIndex = instruction.StepNumber - 1;
            Instructions.Remove(instruction);

            // Renumber remaining steps
            for (int i = 0; i < Instructions.Count; i++)
            {
                Instructions[i].StepNumber = i + 1;
            }

            // Update Error UI 
            OnPropertyChanged(nameof(InstructionsValidationMessage));
        }

        #endregion

        #region Edit Instruction

        private void ExecuteEditInstruction(Instruction instruction)
        {
            if (instruction != null)
            {
                _selectedInstruction = instruction;
                CurrentInstruction = instruction.Description ?? string.Empty;
                IsEditing = true;
            }
        }

        #endregion

        #region Cancel

        private void ExecuteCancelEdit(Instruction instruction)
        {
            CurrentInstruction = string.Empty;
            IsEditing = false;
            _selectedInstruction = null;
        }

        #endregion

        #region Save

        private void ExecuteSaveRecipe()
        {
            // Validate all required fields
            ValidateProperty(nameof(Name));
            ValidateProperty(nameof(CookingTime));
            ValidateProperty(nameof(Servings));
            ValidateProperty(nameof(Instructions));
            ValidateProperty(nameof(Ingredients));

            // Check if any validation messages exist
            if (!string.IsNullOrEmpty(NameValidationMessage) ||
                !string.IsNullOrEmpty(CookingTimeValidationMessage) ||
                !string.IsNullOrEmpty(ServingsValidationMessage) ||
                !string.IsNullOrEmpty(InstructionsValidationMessage) ||
                !string.IsNullOrEmpty(IngredientsValidationMessage))
            {
                return;
            }

            if (IsEditMode)
            {
                UpdateRecipe();
            }
            else
            {
                _recipe.Ingredients = Ingredients.ToList();
                _recipe.Instructions = Instructions.ToList();

                using (var transaction = _context.Database.BeginTransaction())
                {
                    string imageFolder = null;
                    try
                    {
                        // First save racipe to get id
                        _context.Recipes.Add(_recipe);
                        _context.SaveChanges();

                        // Now save image with correct ID if one is selected
                        if (!string.IsNullOrEmpty(ImagePath))
                        {
                            _recipe.ImagePath = SaveImage(ImagePath);
                            imageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Recept", _recipe.Id.ToString());

                            // Update recipe with image path
                            _context.SaveChanges();
                        }
                        else
                        {
                            _recipe.ImagePath = Path.Combine("Images", "Recept", "recept.png");
                            _context.SaveChanges();
                        }

                        transaction.Commit();

                        RecipeAdded?.Invoke();

                        // Close window after successful save
                        Application.Current.Windows.OfType<Window>()
                            .FirstOrDefault(w => w.IsActive)?.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();

                        // Clean up created image folder if save failed
                        if (imageFolder != null && Directory.Exists(imageFolder))
                        {
                            Directory.Delete(imageFolder, true);
                        }

                        System.Windows.MessageBox.Show($"Kunde inte spara receptet: {ex.InnerException?.Message ?? ex.Message}");
                    }
                }
            }
 
        }

        #endregion

        #region Update

        private void UpdateRecipe()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var existingRecipe = _context.Recipes.Find(_recipe.Id);
                    string currentImagePath = existingRecipe.ImagePath;  // Save current image path

                    // Update recipe data
                    _recipe.Ingredients = Ingredients.ToList();
                    _recipe.Instructions = Instructions.ToList();

                    if (existingRecipe != null)
                    {
                        _context.Entry(existingRecipe).CurrentValues.SetValues(_recipe);

                        // Restore current image path if no new image selected
                        if (string.IsNullOrEmpty(ImagePath))
                        {
                            existingRecipe.ImagePath = currentImagePath;
                        }
                        _context.SaveChanges();
                    }

                    // Handle new image only if selected
                    if (!string.IsNullOrEmpty(ImagePath))
                    {
                        _recipe.ImagePath = SaveImage(ImagePath);
                        _context.SaveChanges();
                    }

                    transaction.Commit();
                    RecipeAdded?.Invoke();

                    Application.Current.Windows.OfType<Window>()
                        .FirstOrDefault(w => w.IsActive)?.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Windows.MessageBox.Show($"Kunde inte uppdatera receptet: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }


        private void ExecuteEditRecipe(Recipe recipe)
        {
            // Close details window
            Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.IsActive)?.Close();

            // Open edit window
            var mainWindow = Application.Current.MainWindow;
            var recipeBookVM = mainWindow.DataContext as RecipeBookViewModel;
            var addRecipeWindow = new AddRecipeWindow(recipeBookVM, recipe);
            addRecipeWindow.ShowDialog();
        }

        #endregion

        #region Validation

        private string ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Name):
                    return string.IsNullOrWhiteSpace(Name) ? "Receptnamn krävs" : string.Empty;

                case nameof(CookingTime):
                    return CookingTime <= 0 ? "Måste vara större än 0" : string.Empty;

                case nameof(Servings):
                    return Servings <= 0 ? "Måste vara större än 0" : string.Empty;

                case nameof(Ingredients):
                    return Ingredients.Any() ? string.Empty : "Minst en ingrediens krävs";

                case nameof(Instructions):
                    return Instructions.Any() ? string.Empty : "Minst en instruktion krävs";
            }
            return string.Empty;
        }

        public string NameValidationMessage => ValidateProperty(nameof(Name));
        public string CookingTimeValidationMessage => ValidateProperty(nameof(CookingTime));
        public string ServingsValidationMessage => ValidateProperty(nameof(Servings));
        public string IngredientsValidationMessage => ValidateProperty(nameof(Ingredients));
        public string InstructionsValidationMessage => ValidateProperty(nameof(Instructions));

        #endregion
    }
}
