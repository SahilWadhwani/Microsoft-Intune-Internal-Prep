using EndpointGuardian.Api.Models;
namespace EndpointGuardian.Api.Repositories;



public interface IPolicyRepository

{

    List<CompliancePolicy> GetAll();

    CompliancePolicy? GetById(string id);

    void Add(CompliancePolicy policy);

}