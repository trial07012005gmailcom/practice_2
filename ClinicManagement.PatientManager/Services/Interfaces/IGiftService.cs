using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClinicManagement.PatientManager.Services.Interfaces
{
    /// <summary>
    /// Interface for the gift service
    /// </summary>
    public class Gift
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    /// <summary>
    /// Interface for the gift service
    /// </summary>
    public interface IGiftService
    {
        /// <summary>
        /// Gets gifts from the external API
        /// </summary>
        /// <returns>List of gifts</returns>
        Task<List<Gift>> GetGiftsAsync();
    }
}