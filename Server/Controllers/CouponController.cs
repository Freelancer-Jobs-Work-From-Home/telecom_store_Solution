using BussinessObject.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.ViewModel.Coupon;
using Services.IServices;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var coupons = _couponService.GetAll().Select(c => new CouponViewModel
            {
                CouponID = c.CouponID,
                Code = c.Code,
                DiscountPercentage = c.DiscountPercentage,
                ExpiryDate = c.ExpiryDate
            }).ToList();

            return Ok(coupons);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var coupon = _couponService.GetById(id);
            if (coupon == null) return NotFound();

            var model = new CouponViewModel
            {
                CouponID = coupon.CouponID,
                Code = coupon.Code,
                DiscountPercentage = coupon.DiscountPercentage,
                ExpiryDate = coupon.ExpiryDate
            };

            return Ok(model);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var coupon = new Coupon
            {
                CouponID = Guid.NewGuid(),
                Code = model.Code,
                DiscountPercentage = model.DiscountPercentage,
                ExpiryDate = model.ExpiryDate,
                CreatedAt = DateTime.UtcNow
            };

            _couponService.Add(coupon);
            return Ok(new { message = "Tạo mã giảm giá thành công." });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody] CouponViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var coupon = _couponService.GetById(id);
            if (coupon == null) return NotFound();

            coupon.Code = model.Code;
            coupon.DiscountPercentage = model.DiscountPercentage;
            coupon.ExpiryDate = model.ExpiryDate;

            _couponService.Update(coupon);
            return Ok(new { message = "Cập nhật mã giảm giá thành công." });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var coupon = _couponService.GetById(id);
            if (coupon == null) return NotFound();

            _couponService.Delete(id);
            return Ok(new { message = "Xóa mã giảm giá thành công." });
        }
    }

}
