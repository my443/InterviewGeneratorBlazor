using InterviewGeneratorBlazor.Data;
using InterviewGeneratorBlazor.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace InterviewGeneratorBlazor.ViewModels
{
    public class InterviewViewModel
    {
        private readonly AppDbContextFactory _contextFactory;

        public List<Category> Categories { get; set; } = new();
        public List<Question> AvailableQuestions { get; set; } = new();
        public List<Question> InterviewQuestions { get; set; } = new();

        public int? SelectedCategoryId { get; set; }
        public int? SelectedQuestionId { get; set; }
        public string InterviewName { get; set; } = string.Empty;
        public DateTime InterviewDate { get; set; } = DateTime.Today;

        public InterviewViewModel(AppDbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
            LoadCategories();
        }

        public void LoadCategories()
        {
            using var db = _contextFactory.CreateDbContext();
            Categories = db.Categories.ToList();
        }

        public void LoadQuestionsForCategory()
        {
            if (SelectedCategoryId == null) return;
            using var db = _contextFactory.CreateDbContext();
            AvailableQuestions = db.Questions
                .Where(q => q.CategoryId == SelectedCategoryId)
                .ToList();
        }

        public void AddQuestionToInterview()
        {
            if (SelectedQuestionId == null) return;
            var question = AvailableQuestions.FirstOrDefault(q => q.Id == SelectedQuestionId);
            if (question != null && !InterviewQuestions.Any(q => q.Id == question.Id))
            {
                InterviewQuestions.Add(question);
            }
        }

        public void SaveInterview()
        {
            using var db = _contextFactory.CreateDbContext();
            var interview = new Interview
            {
                InterviewName = InterviewName,
                DateCreated = InterviewDate,
                Questions = new List<Question>()
            };

            // Attach existing questions by their IDs
            foreach (var q in InterviewQuestions)
            {
                var question = db.Questions.Find(q.Id);
                if (question != null)
                {
                    interview.Questions.Add(question);
                }
            }

            db.Interviews.Add(interview);
            db.SaveChanges();
        }
        public void LoadInterviewById(int interviewId)
        {
            using var context = _contextFactory.CreateDbContext();
            var interview = context.Interviews
                .Include(i => i.Questions)
                .FirstOrDefault(i => i.Id == interviewId);

            if (interview != null)
            {
                InterviewName = interview.InterviewName;
                InterviewDate = interview.DateCreated;
                InterviewQuestions = interview.Questions.ToList();
                // Optionally, set other properties as needed
            }
        }
    }
}