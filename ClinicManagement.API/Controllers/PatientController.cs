using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Models;
using ClinicManagement.PatientManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(IPatientService patientService, ILogger<PatientsController> logger)
        {
            _patientService = patientService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all patients
        /// </summary>
        /// <returns>List of all patients</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Patient>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPatients()
        {
            _logger.LogInformation("Getting all patients");
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        /// <summary>
        /// Gets a patient by CI
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <returns>Patient if found</returns>
        [HttpGet("{ci}")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatientByCI(string ci)
        {
            _logger.LogInformation("Getting patient by CI: {CI}", ci);
            var patient = await _patientService.GetPatientByCIAsync(ci);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found" });
            }
            
            return Ok(patient);
        }

        /// <summary>
        /// Creates a new patient
        /// </summary>
        /// <param name="request">Patient creation request</param>
        /// <returns>Created patient</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            try
            {
                _logger.LogInformation("Creating patient with CI: {CI}", request.CI);
                var patient = await _patientService.CreatePatientAsync(request);
                return CreatedAtAction(nameof(GetPatientByCI), new { ci = patient.CI }, patient);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for patient creation");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict in patient creation");
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates a patient
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <param name="request">Patient update request</param>
        /// <returns>Updated patient</returns>
        [HttpPut("{ci}")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePatient(string ci, [FromBody] UpdatePatientRequest request)
        {
            try
            {
                _logger.LogInformation("Updating patient with CI: {CI}", ci);
                var patient = await _patientService.UpdatePatientAsync(ci, request);
                
                if (patient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }
                
                return Ok(patient);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid input for patient update");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a patient
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <returns>No content</returns>
        [HttpDelete("{ci}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient(string ci)
        {
            _logger.LogInformation("Deleting patient with CI: {CI}", ci);
            var result = await _patientService.DeletePatientAsync(ci);
            
            if (!result)
            {
                return NotFound(new { message = "Patient not found" });
            }
            
            return NoContent();
        }
    }
}
