using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QAndA.Data;
using QAndA.Data.Models;
using QAndA.Hubs;

namespace QAndA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IHubContext<QuestionsHub> _questionHubContext;

        public QuestionsController(IDataRepository dataRepository, IHubContext<QuestionsHub> questionHubContext)
        {
            _dataRepository = dataRepository;
            _questionHubContext = questionHubContext;
        }

        [HttpGet]
        public IEnumerable<QuestionGetManyResponse> GetQuestions(string search)
        {
            return string.IsNullOrEmpty(search)
                ? _dataRepository.GetQuestions()
                : _dataRepository.GetQuestionsBySearch(search);
        }

        [HttpGet("unanswered")]
        public IEnumerable<QuestionGetManyResponse> GetUnansweredQuestions()
        {
            return _dataRepository.GetUnansweredQuestions();
        }

        [HttpGet("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        [HttpPost]
        public ActionResult<QuestionGetSingleResponse> PostQuestion(QuestionPostRequest questionPostRequest)
        {
            var savedQuestion = _dataRepository.PostQuestion(new QuestionPostFullRequest
                {
                    Title = questionPostRequest.Title,
                    Content = questionPostRequest.Content,
                    UserId = "1",
                    UserName = "bob.test@test.com",
                    Created = DateTime.UtcNow
                });
            return CreatedAtAction(nameof(GetQuestion), new {questionId = savedQuestion.QuestionId}, savedQuestion);
        }

        [HttpPut("{questionId}")]
        public ActionResult<QuestionGetSingleResponse> PutQuestion(int questionId,
            QuestionPutRequest questionPutRequest)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }

            questionPutRequest.Title =
                string.IsNullOrEmpty(questionPutRequest.Title) ? question.Title : questionPutRequest.Title;
            questionPutRequest.Content =
                string.IsNullOrEmpty(questionPutRequest.Content) ? question.Content : questionPutRequest.Content;
            var savedQuestion =
                _dataRepository.PutQuestion(questionId,
                    questionPutRequest);
            return savedQuestion;
        }

        [HttpDelete("{questionId}")]
        public ActionResult DeleteQuestion(int questionId)
        {
            var question = _dataRepository.GetQuestion(questionId);
            if (question == null)
            {
                return NotFound();
            }

            _dataRepository.DeleteQuestion(questionId);
            return NoContent();
        }
        
        [HttpPost("answer")]
        public ActionResult<AnswerGetResponse> PostAnswer(AnswerPostRequest answerPostRequest)
        {
            var questionExists = _dataRepository.QuestionExists(answerPostRequest.QuestionId.Value);
            if (!questionExists)
            {
                return NotFound();
            }
            var savedAnswer = _dataRepository.PostAnswer(new AnswerPostFullRequest
                    {
                        QuestionId = answerPostRequest.QuestionId.Value,
                        Content = answerPostRequest.Content,
                        UserId = "1",
                        UserName = "bob.test@test.com",
                        Created = DateTime.UtcNow
                    }
                );
            
            _questionHubContext.Clients.Group($"Question-{answerPostRequest.QuestionId.Value}")
                .SendAsync("ReceiveQuestion", _dataRepository.GetQuestion(answerPostRequest.QuestionId.Value));
            
            return savedAnswer;
        }
    }
}