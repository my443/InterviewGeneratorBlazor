using InterviewGeneratorBlazor.Data;
using InterviewGeneratorBlazor.Models;

namespace InterviewGeneratorBlazor.ViewModels
{
    public class QuestionViewModel
    {
        private readonly AppDbContextFactory _contextFactory;
        private readonly int _categoryId;

        public QuestionViewModel(AppDbContextFactory contextFactory, int categoryId)
        {
            _contextFactory = contextFactory;
            _categoryId = categoryId;
            LoadQuestions();
        }

        public List<Question> Questions { get; set; } = new();
        public Question QuestionModel { get; set; } = new();
        public bool IsEditMode { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public void LoadQuestions()
        {
            using var db = _contextFactory.CreateDbContext();
            Questions = db.Questions
                .Where(q => q.CategoryId == _categoryId)
                .ToList();
        }

        public void EditQuestion(Question question)
        {
            IsEditMode = true;
            QuestionModel = new Question
            {
                Id = question.Id,
                Text = question.Text,
                Answer = question.Answer,
                CategoryId = question.CategoryId
            };
        }

        public void DeleteQuestion(int id)
        {
            using var db = _contextFactory.CreateDbContext();
            var q = db.Questions.Find(id);
            if (q != null)
            {
                db.Questions.Remove(q);
                db.SaveChanges();
                LoadQuestions();
            }
            if (IsEditMode && QuestionModel.Id == id)
            {
                ResetForm();
            }
        }

        public void SaveQuestion()
        {
            ErrorMessage = null;
            using var db = _contextFactory.CreateDbContext();
            if (string.IsNullOrWhiteSpace(QuestionModel.Text))
            {
                ErrorMessage = "Question text is required.";
                return;
            }
            if (string.IsNullOrWhiteSpace(QuestionModel.Answer))
            {
                ErrorMessage = "Answer is required.";
                return;
            }

            if (IsEditMode)
            {
                var q = db.Questions.Find(QuestionModel.Id);
                if (q != null)
                {
                    q.Text = QuestionModel.Text;
                    q.Answer = QuestionModel.Answer;
                    db.SaveChanges();
                    LoadQuestions();
                }
            }
            else
            {
                var newQuestion = new Question
                {
                    Text = QuestionModel.Text,
                    Answer = QuestionModel.Answer,
                    CategoryId = _categoryId
                };
                db.Questions.Add(newQuestion);
                db.SaveChanges();
                LoadQuestions();
            }
            ResetForm();
        }

        public void ResetForm()
        {
            QuestionModel = new Question { CategoryId = _categoryId };
            IsEditMode = false;
            ErrorMessage = null;
        }
    }
}
