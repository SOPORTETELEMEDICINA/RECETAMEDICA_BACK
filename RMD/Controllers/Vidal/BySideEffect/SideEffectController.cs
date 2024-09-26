using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RMD.Extensions;
using RMD.Interface.Vidal;

namespace RMD.Controllers.Vidal.BySideEffect
{
    [ApiController]
    [Authorize]
    [ServiceFilter(typeof(ValidateTokenFilter))]
    [Route("api/[controller]")]
    public class SideEffectController : ControllerBase
    {
        private readonly ISideEffectService _sideEffectService;

        public SideEffectController(ISideEffectService sideEffectService)
        {
            _sideEffectService = sideEffectService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSideEffect(int id)
        {
            var sideEffect = await _sideEffectService.GetSideEffectByIdAsync(id);
            if (sideEffect == null)
            {
                return NotFound();
            }

            return Ok(sideEffect);
        }
    }

}
