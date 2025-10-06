using InterviewGeneratorBlazor.Data;
using InterviewGeneratorBlazor.Models;
using Microsoft.EntityFrameworkCore;

namespace InterviewGeneratorBlazor.ViewModels
{
    public class CategoryViewModel
    {
        private readonly AppDbContextFactory _contextFactory;
        private readonly string _sqliteDbPath;

        public CategoryViewModel(AppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            LoadCategories();
        }

        public List<Category> Categories { get; set; } = new();
        public Category CategoryModel { get; set; } = new();
        public bool IsEditMode { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public void LoadCategories()
        {
            using var db = _contextFactory.CreateDbContext();
            Categories = db.Categories.Include(c => c.Questions).ToList();
        }

        public void EditCategory(Category category)
        {
            IsEditMode = true;
            CategoryModel = new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Questions = category.Questions.ToList()
            };
        }

        public void DeleteCategory(int id)
        {
            using var db = _contextFactory.CreateDbContext();
            var cat = db.Categories.Find(id);
            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                LoadCategories();
            }
            if (IsEditMode && CategoryModel.Id == id)
            {
                ResetForm();
            }
        }

        public void SaveCategory()
        {
            ErrorMessage = null;
            using var db = _contextFactory.CreateDbContext();
            if (string.IsNullOrWhiteSpace(CategoryModel.Name))
            {
                ErrorMessage = "Name is required.";
                return;
            }

            if (IsEditMode)
            {
                var cat = db.Categories.Find(CategoryModel.Id);
                if (cat != null)
                {
                    cat.Name = CategoryModel.Name;
                    cat.Description = CategoryModel.Description;
                    db.SaveChanges();
                    LoadCategories();
                }
            }
            else
            {
                var newCategory = new Category
                {
                    Name = CategoryModel.Name,
                    Description = CategoryModel.Description,
                    Questions = new List<Question>()
                };
                db.Categories.Add(newCategory);
                db.SaveChanges();
                LoadCategories();
            }
            ResetForm();
        }

        public void ResetForm()
        {
            CategoryModel = new Category();
            IsEditMode = false;
            ErrorMessage = null;
        }
    }
}
