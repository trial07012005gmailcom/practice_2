using System.Collections.Generic;
using System.Threading.Tasks;
using ClinicManagement.PatientManager.Models;

namespace ClinicManagement.PatientManager.Repository.Interfaces
{
    /// <summary>
    /// Interface for patient repository operations
    /// </summary>
    public interface IPatientRepository
    {
        /// <summary>
        /// Gets all patients
        /// </summary>
        /// <returns>List of all patients</returns>
        Task<List<Patient>> GetAllAsync();
        
        /// <summary>
        /// Gets a patient by CI
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <returns>Patient if found, null otherwise</returns>
        Task<Patient> GetByIdAsync(string ci);
        
        /// <summary>
        /// Creates a new patient
        /// </summary>
        /// <param name="patient">Patient to create</param>
        /// <returns>Created patient</returns>
        Task<Patient> CreateAsync(Patient patient);
        
        /// <summary>
        /// Updates an existing patient
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <param name="patient">Updated patient information</param>
        /// <returns>Updated patient if successful, null if not found</returns>
        Task<Patient> UpdateAsync(string ci, Patient patient);
        
        /// <summary>
        /// Deletes a patient
        /// </summary>
        /// <param name="ci">Patient's CI</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(string ci);
    }
}
