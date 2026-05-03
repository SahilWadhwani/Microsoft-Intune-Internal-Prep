using EndpointGuardian.Api.Models;

namespace EndpointGuardian.Api.Services;

public interface IAccessDecisionService

{

    AccessDecisionResponse? CreateDecision(CreateAccessDecisionRequest request);

}