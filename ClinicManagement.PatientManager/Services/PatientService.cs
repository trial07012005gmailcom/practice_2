using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Models;
using ClinicManagement.PatientManager.Repository.Interfaces;
using ClinicManagement.PatientManager.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.PatientManager.Services
{
    /// <summary>
    /// Implementation of the patient service
    /// </summary>
    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly ILogger<PatientService> _logger;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="patientRepository">Patient repository</param>
        /// <param name="logger">Logger instance</param>
        public PatientService(IPatientRepository patientRepository, ILogger<PatientService> logger)
        {
            _patientRepository = patientRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all patients");
                return await _patientRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all patients");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Patient> GetPatientByCIAsync(string ci)
        {
            try
            {
                _logger.LogInformation("Getting patient by CI: {CI}", ci);
                var patient = await _patientRepository.GetByIdAsync(ci);
                
                if (patient == null)
                {
                    _logger.LogWarning("Patient with CI {CI} not found", ci);
                }
                
                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by CI: {CI}", ci);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Patient> CreatePatientAsync(CreatePatientRequest request)
        {
            try
            {
                _logger.LogInformation("Creating patient with CI: {CI}", request.CI);
                
                // Input validation
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    throw new ArgumentException("Name is required", nameof(request.Name));
                }
                
                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    throw new ArgumentException("Last name is required", nameof(request.LastName));
                }
                
                if (string.IsNullOrWhiteSpace(request.CI))
                {
                    throw new ArgumentException("CI is required", nameof(request.CI));
                }
                
                // Create the patient with randomly assigned blood group
                var patient = new Patient(request.Name, request.LastName, request.CI);
                
                // Add the patient to the repository
                return await _patientRepository.CreateAsync(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient with CI: {CI}", request.CI);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Patient> UpdatePatientAsync(string ci, UpdatePatientRequest request)
        {
            try
            {
                _logger.LogInformation("Updating patient with CI: {CI}", ci);
                
                // Input validation
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    throw new ArgumentException("Name is required", nameof(request.Name));
                }
                
                if (string.IsNullOrWhiteSpace(request.LastName))
                {
                    throw new ArgumentException("Last name is required", nameof(request.LastName));
                }
                
                // Check if patient exists
                var existingPatient = await _patientRepository.GetByIdAsync(ci);
                if (existingPatient == null)
                {
                    _logger.LogWarning("Patient with CI {CI} not found for update", ci);
                    return null;
                }
                
                // Update patient properties
                existingPatient.Name = request.Name;
                existingPatient.LastName = request.LastName;
                
                // Update the patient in the repository
                return await _patientRepository.UpdateAsync(ci, existingPatient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient with CI: {CI}", ci);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeletePatientAsync(string ci)
        {
            try
            {
                _logger.LogInformation("Deleting patient with CI: {CI}", ci);
                return await _patientRepository.DeleteAsync(ci);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient with CI: {CI}", ci);
                throw;
            }
        }
    }
}
