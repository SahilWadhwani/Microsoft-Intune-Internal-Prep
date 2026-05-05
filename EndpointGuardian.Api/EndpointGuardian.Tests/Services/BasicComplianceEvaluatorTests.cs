using EndpointGuardian.Api.Models;
using EndpointGuardian.Api.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace EndpointGuardian.Tests.Services;

public class BasicComplianceEvaluatorTests
{
    private readonly BasicComplianceEvaluator _evaluator;

    public BasicComplianceEvaluatorTests()
    {
        _evaluator = new BasicComplianceEvaluator(
            NullLogger<BasicComplianceEvaluator>.Instance);
    }

    private static CompliancePolicy CreatePolicy(
        int minOsVersion = 13,
        bool requireEncryption = true,
        bool requirePassword = true,
        bool requireDefender = true,
        int maxCheckInAgeHours = 24)
    {
        return new CompliancePolicy(

        Guid.NewGuid().ToString(),

        "Corporate Baseline",

        minimumOsVersion,

        requireEncryption,

        requirePassword,

        requireDefender,

        maxCheckInAgeHours);
    }


    private static ManagedDevice CreateDevice(

        int osVersion = 14,

        bool? isEncrypted = true,

        bool? hasPassword = true,

        bool? defenderEnabled = true)

    {

        var device = new ManagedDevice(

            Guid.NewGuid().ToString(),

            "Test Device",

            DevicePlatform.Windows);

        device.UpdateOsVersion(osVersion);

        device.CheckIn(isEncrypted, hasPassword, defenderEnabled);

        return device;

    }

    [Fact]
    public void EvaluatePolicy_WhenDeviceSatisfiesPolicy_ReturnsCompliant()
    {
        // Arrange
        var device = CreateDevice(

        osVersion: 14,

        isEncrypted: true,

        hasPassword: true,

        defenderEnabled: true);

        var policy = CreatePolicy(minOsVersion: 13);

        // Act
        var result = _evaluator.EvaluatePolicy(device, policy);

        // Assert
        result.Should().Be(ComplianceStatus.Compliant);
        result.FailreReasons.Should().BeEmpty();
    }

    [Fact]

public void EvaluatePolicy_WhenOsVersionTooLow_ReturnsNonCompliantWithReason()

{

    var device = CreateDevice(osVersion: 12);

    var policy = CreatePolicy(minimumOsVersion: 13);

    var result = _evaluator.EvaluatePolicy(device, policy);

    result.Status.Should().Be(ComplianceStatus.NonCompliant);

    result.FailureReasons.Should()

        .Contain(r => r.Code == "OS_VERSION_TOO_LOW");

}
}