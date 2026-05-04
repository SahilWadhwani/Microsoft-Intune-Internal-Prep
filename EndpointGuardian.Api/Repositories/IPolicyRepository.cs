using System.ComponentModel.Design.Serialization;
using EndpointGuardian.Api.Models;
namespace EndpointGuardian.Api.Repositories;



public interface IPolicyRepository

{

    Task<List<CompliancePolicy>> GetAllAsync();

    Task<CompliancePolicy?> GetByIdAsync(string id);

    Task AddAsync(CompliancePolicy policy);

    Task UpdateAsync(CompliancePolicy policy);

}