using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Models;

namespace ClinicManagement.PatientManager.Services.Interfaces
{
    /// <summary>
    /// Interface for patient management services
    /// </summary>
    public interface IPatientService
    {
        /// <summary>
        /// Gets all patients
        /// </summary>
        /// <returns>List of all patients</returns>
        Task<List<Patient>> GetAllPatientsAsync();
        
        /// <summary>
        /// Gets a patient by CI
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <returns>Patient if found, null otherwise</returns>
        Task<Patient> GetPatientByCIAsync(string ci);
        
        /// <summary>
        /// Creates a new patient
        /// </summary>
        /// <param name="request">Patient creation request</param>
        /// <returns>Created patient</returns>
        Task<Patient> CreatePatientAsync(CreatePatientRequest request);
        
        /// <summary>
        /// Updates an existing patient
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <param name="request">Patient update request</param>
        /// <returns>Updated patient</returns>
        Task<Patient> UpdatePatientAsync(string ci, UpdatePatientRequest request);
        
        /// <summary>
        /// Deletes a patient
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeletePatientAsync(string ci);
    }
}
