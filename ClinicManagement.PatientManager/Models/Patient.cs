using System;
using System.Text.Json.Serialization;

namespace ClinicManagement.PatientManager.Models
{
    /// <summary>
    /// Represents a patient in the clinic
    /// </summary>
    public class Patient
    {
        /// <summary>
        /// Patient's first name
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Patient's last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Patient's identification number
        /// </summary>
        public string CI { get; set; }
        
        /// <summary>
        /// Patient's blood group
        /// </summary>
        public string BloodGroup { get; set; }

        /// <summary>
        /// Default constructor required for deserialization
        /// </summary>
        public Patient()
        {
        }

        /// <summary>
        /// Constructor with patient information
        /// </summary>
        public Patient(string name, string lastName, string ci)
        {
            Name = name;
            LastName = lastName;
            CI = ci;
            BloodGroup = GenerateRandomBloodGroup();
        }

        /// <summary>
        /// Generates a random blood group for the patient
        /// </summary>
        /// <returns>A random blood group string (A+, A-, B+, B-, AB+, AB-, O+, O-)</returns>
        private static string GenerateRandomBloodGroup()
        {
            string[] bloodGroups = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
            return bloodGroups[new Random().Next(bloodGroups.Length)];
        }
        
        /// <summary>
        /// Converts the patient to a CSV string format
        /// </summary>
        /// <returns>CSV formatted string</returns>
        public string ToCSV()
        {
            return $"{Name},{LastName},{CI},{BloodGroup}";
        }
        
        /// <summary>
        /// Creates a patient object from a CSV string
        /// </summary>
        /// <param name="csvLine">CSV formatted string</param>
        /// <returns>Patient object</returns>
        public static Patient FromCSV(string csvLine)
        {
            string[] parts = csvLine.Split(',');
            if (parts.Length != 4)
            {
                throw new FormatException("Invalid CSV format for Patient");
            }
            
            return new Patient
            {
                Name = parts[0].Trim(),
                LastName = parts[1].Trim(),
                CI = parts[2].Trim(),
                BloodGroup = parts[3].Trim()
            };
        }
    }

    /// <summary>
    /// DTO for patient creation
    /// </summary>
    public class CreatePatientRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string CI { get; set; }
    }

    /// <summary>
    /// DTO for patient update
    /// </summary>
    public class UpdatePatientRequest
    {
        public string Name { get; set; }
        public string LastName { get; set; }
    }
}
