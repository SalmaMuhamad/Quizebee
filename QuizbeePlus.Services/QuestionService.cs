using QuizbeePlus.Data;
using QuizbeePlus.Entities;
using QuizbeePlus.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizbeePlus.Services
{
    public class QuestionService
    {
        #region Define as Singleton
        private static QuestionService _Instance;

        public static QuestionService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new QuestionService();
                }

                return (_Instance);
            }
        }

        private QuestionService()
        {
        }
        #endregion
        public List<Question> GetAllQuestions()
        {
            using (var context = new QuizbeeContext())
            {
                return context.Questions
                                        .OrderBy(x => x.Title)
                                        .ToList();
            }
        }

        public QuestionsSearch GetQuestions(string searchTerm, int pageNo, int pageSize)
        {
            using (var context = new QuizbeeContext())
            {
                var search = new QuestionsSearch();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    search.Questions = context.Questions
                                        .Include(u => u.Category)
                                        .OrderBy(x => x.Title)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.Users.Count();
                }
                else
                {
                    search.Questions = context.Questions
                                        .Where(u => u.Title.ToLower().Contains(searchTerm.ToLower()))
                                        .Include(u => u.Category)
                                        .OrderBy(x => x.Title)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.Categories
                                        .Where(u => u.Name.ToLower().Contains(searchTerm.ToLower())).Count();
                }

                return search;
            }
        }

        public async Task<bool> NewQuestion(Question question)
        {
            using (var context = new QuizbeeContext())
            {
                context.Questions.Add(question);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> SaveNewQuestion(Question question)
        {
            using (var context = new QuizbeeContext())
            {
                context.Questions.Add(question);

                return await context.SaveChangesAsync() > 0;
            }
        }

    }
}
