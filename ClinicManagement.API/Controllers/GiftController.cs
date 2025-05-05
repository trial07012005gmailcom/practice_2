using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GiftsController : ControllerBase
    {
        private readonly IGiftService _giftService;
        private readonly ILogger<GiftsController> _logger;

        public GiftsController(IGiftService giftService, ILogger<GiftsController> logger)
        {
            _giftService = giftService;
            _logger = logger;
        }

        /// <summary>
        /// Gets gifts for patients
        /// </summary>
        /// <returns>List of gifts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Gift>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGifts()
        {
            try
            {
                _logger.LogInformation("Getting gifts for patients");
                var gifts = await _giftService.GetGiftsAsync();
                return Ok(gifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gifts");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error retrieving gifts from external service" });
            }
        }
    }
}
