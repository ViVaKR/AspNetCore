using ContosoPizza.Data;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza;

[ApiController]
[Route("[controller]")]
public class CouponController : ControllerBase
{
    PromotionsContext _contex;

    public CouponController(PromotionsContext context)
    {
        _contex = context;
    }

    [HttpGet]
    public IEnumerable<Coupon> Get()
    {
        return _contex.Coupons.AsNoTracking().ToList();
    }
}
