using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Models;
using ClinicManagement.PatientManager.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.PatientManager.Repository
{
    /// <summary>
    /// File-based implementation of the patient repository
    /// </summary>
    public class FilePatientRepository : IPatientRepository
    {
        private readonly string _filePath;
        private readonly ILogger<FilePatientRepository> _logger;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Constructor with file path
        /// </summary>
        /// <param name="filePath">Path to the patients file</param>
        /// <param name="logger">Logger instance</param>
        public FilePatientRepository(string filePath, ILogger<FilePatientRepository> logger)
        {
            _filePath = filePath;
            _logger = logger;
            
            // Create the directory if it doesn't exist
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            // Create the file if it doesn't exist
            if (!File.Exists(filePath))
            {
                File.Create(filePath).Close();
            }
        }

        /// <inheritdoc/>
        public async Task<List<Patient>> GetAllAsync()
        {
            try
            {
                var patients = new List<Patient>();
                string[] lines = await File.ReadAllLinesAsync(_filePath);
                
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            patients.Add(Patient.FromCSV(line));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error parsing patient line: {Line}", line);
                        }
                    }
                }
                
                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading patients from file");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Patient> GetByIdAsync(string ci)
        {
            try
            {
                var patients = await GetAllAsync();
                return patients.FirstOrDefault(p => p.CI == ci);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by CI: {CI}", ci);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Patient> CreateAsync(Patient patient)
        {
            try
            {
                var patients = await GetAllAsync();
                
                // Check if patient with the same CI already exists
                if (patients.Any(p => p.CI == patient.CI))
                {
                    _logger.LogWarning("Patient with CI {CI} already exists", patient.CI);
                    throw new InvalidOperationException($"Patient with CI {patient.CI} already exists");
                }
                
                // Add the new patient and save all
                patients.Add(patient);
                await SaveAllPatientsAsync(patients);
                
                return patient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient: {CI}", patient.CI);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Patient> UpdateAsync(string ci, Patient updatedPatient)
        {
            try
            {
                var patients = await GetAllAsync();
                var existingPatient = patients.FirstOrDefault(p => p.CI == ci);
                
                if (existingPatient == null)
                {
                    _logger.LogWarning("Patient with CI {CI} not found for update", ci);
                    return null;
                }
                
                // Update the patient properties
                existingPatient.Name = updatedPatient.Name;
                existingPatient.LastName = updatedPatient.LastName;
                
                // Save all patients
                await SaveAllPatientsAsync(patients);
                
                return existingPatient;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient: {CI}", ci);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(string ci)
        {
            try
            {
                var patients = await GetAllAsync();
                var patient = patients.FirstOrDefault(p => p.CI == ci);
                
                if (patient == null)
                {
                    _logger.LogWarning("Patient with CI {CI} not found for deletion", ci);
                    return false;
                }
                
                patients.Remove(patient);
                await SaveAllPatientsAsync(patients);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient: {CI}", ci);
                throw;
            }
        }

        /// <summary>
        /// Saves all patients to the file
        /// </summary>
        /// <param name="patients">List of patients to save</param>
        private async Task SaveAllPatientsAsync(List<Patient> patients)
        {
            lock (_lockObject)
            {
                try
                {
                    var lines = patients.Select(p => p.ToCSV()).ToArray();
                    File.WriteAllLines(_filePath, lines);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving patients to file");
                    throw;
                }
            }
            
            await Task.CompletedTask;
        }
    }
}