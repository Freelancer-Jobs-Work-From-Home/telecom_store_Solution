using BussinessObject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.Feedback;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IUserService _userService;

        public FeedbackController(IFeedbackService feedbackService, IUserService userService)
        {
            _feedbackService = feedbackService;
            _userService = userService;
        }

        // POST: api/feedback (Customer gửi feedback)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public IActionResult SubmitFeedback([FromBody] FeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var feedback = new Feedback
            {
                CreatedAt = DateTime.UtcNow,
                Message = model.Message,
                UserID = model.UserID,
                FeedbackID = Guid.NewGuid()
            };

            _feedbackService.Add(feedback);
            return Ok(new { message = "Cảm ơn bạn đã gửi phản hồi!" });
        }

        // GET: api/feedback (Admin xem toàn bộ feedback)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            var feedbacks = _feedbackService.GetAll()
                .Select(f => new FeedbackViewModel
                {
                    FeedbackID = f.FeedbackID,
                    FullName = _userService.GetById(f.UserID)?.FullName,
                    Email = _userService.GetById(f.UserID)?.Email,
                    Message = f.Message,
                    UserID = f.UserID,
                    CreatedAt = f.CreatedAt
                }).ToList();

            return Ok(feedbacks);
        }

        // DELETE: api/feedback/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            var feedback = _feedbackService.GetById(id);
            if (feedback == null) return NotFound();

            _feedbackService.Delete(id);
            return Ok(new { message = "Xóa phản hồi thành công." });
        }
    }

}
