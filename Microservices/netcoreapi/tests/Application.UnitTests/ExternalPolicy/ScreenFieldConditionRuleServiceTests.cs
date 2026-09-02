using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.ExternalPolicy.Models;
using Application.ExternalPolicy.Rules;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ValidationException = Application.Common.Exceptions.ValidationException;

namespace Application.UnitTests.ExternalPolicy
{
    [TestFixture]
    public class ScreenFieldConditionRuleServiceTests
    {
        private IScreenFieldConditionRuleService _service;

        [SetUp]
        public void SetUp()
        {
            var configuration = new ConfigurationBuilder().Build();
            _service = new ScreenFieldConditionRuleService(
                new Application.Common.RuleEngine.RuleEngine(),
                configuration,
                NullLogger<ScreenFieldConditionRuleService>.Instance);
        }

        private static PolicyDataTable Table(string tableName, params Dictionary<string, object>[] rows)
            => new PolicyDataTable { TableName = tableName, TableValue = new List<Dictionary<string, object>>(rows) };

        [Test]
        public async Task Validate_ShouldNotThrow_WhenPolicyDataIsEmpty()
        {
            await _service.Validate(new List<PolicyDataTable>());
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenVehicleCoveragesMatchPostJuly2025Options()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "UMBLMD", "250/500" },
                    { "UNBLMD", "250/500" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenBodilyInjuryIsNotValidForPostJuly2025EffectiveDate()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "001" },
                    { "BILLMD", "20/40" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenBodilyInjuryIsValidForPreJuly2025EffectiveDate()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20250101" },
                    { "LOCNUM", "001" },
                    { "BILLMD", "20/40" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenHiredCarClassCodeIs661900WithBlankExcessHired()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "502" },
                    { "VEHNAM", "HIRED CAR" },
                    { "CLASX", "661900" },
                    { "EXHIRED", "0" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenHiredCarClassCodeIsWrongForPositiveExcessHired()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "502" },
                    { "VEHNAM", "HIRED CAR" },
                    { "CLASX", "661900" },
                    { "EXHIRED", "5000" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenNonOwnedClassCodeDoesNotMatchEmployeeCount()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "501" },
                    { "VEHNAM", "NONOWN-EMPL" },
                    { "CLASX", "660100" },
                    { "NEMPL", "30" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenNonOwnedClassCodeMatchesEmployeeCount()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "501" },
                    { "VEHNAM", "NONOWN-EMPL" },
                    { "CLASX", "660200" },
                    { "NEMPL", "30" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenNonMaDriverHasNoRegStatus()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP131", new Dictionary<string, object>
                {
                    { "DRVNUM", "001" },
                    { "LICNUM", "S40440542" },
                    { "LICSTE", "NH" },
                    { "LICSTA", "" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenMaDriverHasNoRegStatus()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP131", new Dictionary<string, object>
                {
                    { "DRVNUM", "001" },
                    { "LICNUM", "S40440542" },
                    { "LICSTE", "MA" },
                    { "LICSTA", "" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenDeliveryServiceIsYesWithoutExplanation()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP155", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004012" },
                    { "DELIVER", "Y" },
                    { "DELIVERD", "" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenIccPucFilingsIsYes()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP155", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004012" },
                    { "FILING", "Y" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenUnderwriterQuestionAnswersAreNo()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP155", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004012" },
                    { "FILING", "N" },
                    { "HAZARD", "N" },
                    { "SNOWPLOW", "N" },
                    { "DELIVER", "N" },
                    { "DELCON", "N" },
                    { "DELAUTO", "N" },
                    { "CANCNON", "N" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenIrpmFactorIsWithinNonFleetRange()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBPRATPY", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004012" },
                    { "FLEET", "N" },
                    { "IRPM1", "1.00" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenIrpmFactorIsAboveNonFleetRange()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBPRATPY", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004012" },
                    { "FLEET", "N" },
                    { "IRPM1", "1.10" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenIrpmFactorIsWithinFleetRange()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBPRATPY", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004012" },
                    { "FLEET", "Y" },
                    { "IRPM1", "1.10" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldNotThrow_ForMinimalPolicyRow()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DWXP110PY", new Dictionary<string, object>
                {
                    { "POLICY", "MCAQ004015" },
                    { "EFFDTE", "20240101" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenTowingIsSetOnNonPrivatePassengerVehicle()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PU" },
                    { "TOWLPD", "30" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenTowingIsSetOnPrivatePassengerVehicle()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PP" },
                    { "BILLMD", "25/50" },
                    { "TOWLPD", "50" },
                    { "ENHCOV", "N" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenRentalIsSelectedWithAutoPlusEndorsementOn()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PU" },
                    { "RRECOV", "Y" },
                    { "ENHCOV", "Y" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenRentalIsSelectedWithAutoPlusEndorsementOff()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PU" },
                    { "RRECOV", "Y" },
                    { "ENHCOV", "N" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenTruckClassCodeIsNotAPrivatePassengerDefault()
        {
            // VHTYPE="PP" from DB2 but Class Code (014990) doesn't start with 7, so this is
            // really a Truck-type vehicle, not Private Passenger - the PP class code rule
            // must not fire just because VHTYPE says PP.
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "002" },
                    { "VHTYPE", "PP" },
                    { "FLEET", "N" },
                    { "CLASX", "014990" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenPrivatePassengerClassCodeConflictsWithFleetStatus()
        {
            // Class Code 739800 (the Fleet=Yes default) assigned while Fleet Vehicles=No.
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PP" },
                    { "FLEET", "N" },
                    { "CLASX", "739800" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenPrivatePassengerClassCodeMatchesFleetStatus()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PP" },
                    { "FLEET", "N" },
                    { "CLASX", "739100" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenCollisionAndLimitedCollisionAreBothPopulated()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "COLDED", "4000" },
                    { "LCLDED", "500" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenOptionalBodilyInjuryIsSelectedWithoutBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "No Coverage" },
                    { "OBILMD", "100,000" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenUninsuredIsLowerThanBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "UMBLMD", "20/40" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenUninsuredMatchesBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "UMBLMD", "25/50" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenUnderinsuredIsLowerThanBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "UNBLMD", "20/40" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenUnderinsuredMatchesBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "UNBLMD", "25/50" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenUninsuredExceedsOptionalBodilyInjury()
        {
            // Screenshot scenario: BI 25/50, OBI 25/50 (no excess purchased), Uninsured
            // 250/500 -> exceeds the allowable maximum set by Optional BI.
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "OBILMD", "25/50" },
                    { "UMBLMD", "250/500" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenUninsuredMatchesOptionalBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "OBILMD", "250/500" },
                    { "UMBLMD", "250/500" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenUnderinsuredExceedsOptionalBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "OBILMD", "25/50" },
                    { "UNBLMD", "250/500" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenUnderinsuredMatchesOptionalBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "OBILMD", "250/500" },
                    { "UNBLMD", "250/500" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenPropertyDamageIsIncludedWithSplitLimitOptionalBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "OBILMD", "25/50" },
                    { "PDLLMD", "Included" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenPropertyDamageIsIncludedWithOptionalBodilyInjuryNoCoverage()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "OBILMD", "No Coverage" },
                    { "PDLLMD", "Included" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenPropertyDamageHasDollarLimitWithSplitLimitOptionalBodilyInjury()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "OBILMD", "25/50" },
                    { "PDLLMD", "50,000" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenMedicalPaymentsExceeds5000ForTruck()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PU" },
                    { "MEDLMD", "10,000" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenMedicalPaymentsIs5000ForTruck()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PU" },
                    { "BILLMD", "25/50" },
                    { "MEDLMD", "5,000" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenMedicalPaymentsExceeds5000ForNonTruck()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "PP" },
                    { "FLEET", "N" },
                    { "CLASX", "739100" },
                    { "BILLMD", "25/50" },
                    { "MEDLMD", "25,000" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenMedicalPaymentsIsNotAValidOption()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "MEDLMD", "12,500" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenDriveOtherCarIsMissingNumberOfIndividuals()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "500" },
                    { "VEHNAM", "DRIVE OTHER CAR" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenDriveOtherCarHasNumberOfIndividuals()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "500" },
                    { "VEHNAM", "DRIVE OTHER CAR" },
                    { "NINDIV", "2" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenLoanLeaseIsYesWithoutComprehensiveAndCollision()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "LNLSCOV", "Y" },
                    { "CMPDED", "500" },
                    { "COLDED", "No Coverage" },
                    { "LCLDED", "No Coverage" },
                    { "NEWCST", "25000" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenLoanLeaseIsYesWithComprehensiveAndCollision()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "PIPLM1", "8000" },
                    { "LNLSCOV", "Y" },
                    { "CMPDED", "500" },
                    { "COLDED", "500" },
                    { "NEWCST", "25000" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenWaiverOfDeductibleIsSetWithoutCollision()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "COLDWV", "Y" },
                    { "COLDED", "No Coverage" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenWaiverOfDeductibleIsSetOnVirtualVehicleWithoutCollision()
        {
            // Virtual coverage rows carry COLDWV as a sticky default with no deductible of
            // their own; only a real vehicle's waiver requires Collision to be selected.
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "501" },
                    { "VEHNAM", "NONOWN-EMPL" },
                    { "CLASX", "660100" },
                    { "NEMPL", "0" },
                    { "COLDWV", "Y" },
                    { "COLDED", "" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenWaiverOfDeductibleIsSetWithCollision()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "BILLMD", "25/50" },
                    { "PIPLM1", "8000" },
                    { "COLDWV", "Y" },
                    { "COLDED", "500" },
                    { "NEWCST", "25000" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenStatedAmountIsEnteredWithoutPhysicalDamage()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "SRTCST", "18000" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenStatedAmountIsZeroWithoutPhysicalDamage()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "SRTCST", "0" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenCostNewIsZeroWithPhysicalDamageSelected()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "COLDED", "500" },
                    { "NEWCST", "0" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenCostNewIsZeroForVirtualVehicleWithCollision()
        {
            // Drive Other Car is a policy-level exposure, not a real car - it carries
            // Collision for rating but has no Cost New to report.
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "500" },
                    { "VEHNAM", "DRIVE OTHER CAR" },
                    { "NINDIV", "2" },
                    { "COLDED", "500" },
                    { "NEWCST", "0" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenPlateNumberContainsEmbeddedSpace()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "PLATEN", "AB 1234" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenPlateNumberHasNoEmbeddedSpace()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "PLATEN", "  AB1234  " }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenDuplicateVinAcrossVehicles()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P",
                    new Dictionary<string, object>
                    {
                        { "LOCNUM", "001" },
                        { "VEHID", "YV126MFK0F1338518" }
                    },
                    new Dictionary<string, object>
                    {
                        { "LOCNUM", "002" },
                        { "VEHID", "YV126MFK0F1338518" }
                    })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenDuplicatePlateNumberAcrossVehicles()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P",
                    new Dictionary<string, object>
                    {
                        { "LOCNUM", "001" },
                        { "VEHID", "YV126MFK0F1338518" },
                        { "PLATEN", "ABC123" }
                    },
                    new Dictionary<string, object>
                    {
                        { "LOCNUM", "002" },
                        { "VEHID", "1HGCM82633A004352" },
                        { "PLATEN", "ABC123" }
                    })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenVirtualVehiclesShareNotApplicablePlaceholderIds()
        {
            // Non-Owned / Hired / Drive Other Car rows all carry "N/A" as the VIN - that
            // repetition is expected and must not be reported as a duplicate vehicle.
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P",
                    new Dictionary<string, object>
                    {
                        { "LOCNUM", "500" },
                        { "VEHNAM", "DRIVE OTHER CAR" },
                        { "VEHID", "N/A" },
                        { "NINDIV", "2" }
                    },
                    new Dictionary<string, object>
                    {
                        { "LOCNUM", "501" },
                        { "VEHNAM", "NONOWN-EMPL" },
                        { "VEHID", "N/A" },
                        { "CLASX", "660100" },
                        { "NEMPL", "0" }
                    })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenDuplicateDriverLicenseNumber()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP131",
                    new Dictionary<string, object>
                    {
                        { "DRVNUM", "001" },
                        { "LICNUM", "S18671182" },
                        { "LICSTE", "MA" }
                    },
                    new Dictionary<string, object>
                    {
                        { "DRVNUM", "002" },
                        { "LICNUM", "S18671182" },
                        { "LICSTE", "MA" }
                    })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenDriverLicenseNumbersAreDistinct()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP131",
                    new Dictionary<string, object>
                    {
                        { "DRVNUM", "001" },
                        { "LICNUM", "S18671182" },
                        { "LICSTE", "MA" }
                    },
                    new Dictionary<string, object>
                    {
                        { "DRVNUM", "002" },
                        { "LICNUM", "S40440542" },
                        { "LICSTE", "MA" }
                    })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenOemPartsSelectedOnVehicleOlderThanTenYears()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "001" },
                    { "MODLYR", "2010" },
                    { "OEMCOL", "Y" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenOemPartsSelectedOnVehicleWithinTenYears()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "EFFDTE", "20260121" },
                    { "LOCNUM", "001" },
                    { "MODLYR", "2020" },
                    { "OEMCOL", "Y" }
                })
            };

            await _service.Validate(tables);
        }

        [Test]
        public async Task Validate_ShouldThrow_WhenAntiTheftCodeIsNotAllowedForMotorcycle()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "MC" },
                    { "ANTTFT", "3" }
                })
            };

            var act = async () => await _service.Validate(tables);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Test]
        public async Task Validate_ShouldNotThrow_WhenAntiTheftCodeIsAllowedForMotorcycle()
        {
            var tables = new List<PolicyDataTable>
            {
                Table("DMBP130P", new Dictionary<string, object>
                {
                    { "LOCNUM", "001" },
                    { "VHTYPE", "MC" },
                    { "ANTTFT", "4" }
                })
            };

            await _service.Validate(tables);
        }
    }
}
