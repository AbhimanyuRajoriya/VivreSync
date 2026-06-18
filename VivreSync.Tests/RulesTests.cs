using VivreSync.Model.Enums;
using Xunit;

namespace VivreSync.Tests
{
    public class BusinessRuleTests
    {
        private bool IsValidUtilization(int utilization)
        {
            return utilization > 0 && utilization <= 100;
        }

        private bool IsValidDateRange(DateOnly startDate, DateOnly endDate)
        {
            return startDate <= endDate;
        }

        private bool CanAllocate(List<TestAllocation> existingAllocations, DateOnly newStartDate, DateOnly newEndDate, int newUtilization)
        {
            for (var date = newStartDate; date <= newEndDate; date = date.AddDays(1))
            {
                var allocationOnThisDay = existingAllocations
                    .Where(a => a.StartDate <= date && a.EndDate >= date).Sum(a => a.Utilization);

                if (allocationOnThisDay + newUtilization > 100)
                    return false;
            }

            return true;
        }

        private bool IsMilestoneOverdue(DateOnly dueDate, DateOnly today, MilestoneStatus status)
        {
            return dueDate < today && status != MilestoneStatus.Completed;
        }

        private bool IsActiveEmployeeAllowed(bool employeeIsActive, bool userIsActive)
        {
            return employeeIsActive && userIsActive;
        }

        [Fact]
        public void Allocation_Fail_When_Utilization_Above_100()
        {
            var result = IsValidUtilization(120);
            Assert.False(result);
        }

        [Fact]
        public void Allocation_Pass_When_Utilization_Valid()
        {
            var result = IsValidUtilization(60);
            Assert.True(result);
        }

        [Fact]
        public void Allocation_Fail_When_StartDate_After_EndDate()
        {
            var startDate = new DateOnly(2026, 7, 1);
            var endDate = new DateOnly(2026, 6, 30);

            var result = IsValidDateRange(startDate, endDate);
            Assert.False(result);
        }

        [Fact]
        public void Allocation_Fail_When_Employee_OverAllocated()
        {
            var existingAllocations = new List<TestAllocation>
            {
                new TestAllocation
                {
                    StartDate = new DateOnly(2026, 6, 10),
                    EndDate = new DateOnly(2026, 6, 30),
                    Utilization = 60
                },
                new TestAllocation
                {
                    StartDate = new DateOnly(2026, 6, 18),
                    EndDate = new DateOnly(2026, 6, 25),
                    Utilization = 40
                }
            };

            var result = CanAllocate(existingAllocations, new DateOnly(2026, 6, 20), new DateOnly(2026, 6, 22), 10);
            Assert.False(result);
        }

        [Fact]
        public void Allocation_Pass_When_Date_Ranges_Dont_Overlap()
        {
            var existingAllocations = new List<TestAllocation>
            {
                new TestAllocation
                {
                    StartDate = new DateOnly(2026, 6, 10),
                    EndDate = new DateOnly(2026, 6, 30),
                    Utilization = 100
                }
            };

            var result = CanAllocate(
                existingAllocations,
                new DateOnly(2026, 7, 1),
                new DateOnly(2026, 7, 10),
                100);

            Assert.True(result);
        }

        [Fact]
        public void Project_Status_Accept_Lowercase_Input()
        {
            var inputStatus = "active";

            var result = Enum.TryParse<ProjectStatus>(inputStatus, ignoreCase: true, out var parsedStatus);

            Assert.True(result);
            Assert.Equal(ProjectStatus.Active, parsedStatus);
        }

        [Fact]
        public void Milestone_Overdue_When_DueDate_Passed_Or_Not_Completed()
        {
            var today = new DateOnly(2026, 6, 18);
            var dueDate = new DateOnly(2026, 6, 15);
            var status = MilestoneStatus.OnTrack;

            var result = IsMilestoneOverdue(dueDate, today, status);
            Assert.True(result);
        }

        [Fact]
        public void Employee_Not_Allowed_When_User_Inactive()
        {
            var employeeIsActive = true;
            var userIsActive = false;

            var result = IsActiveEmployeeAllowed(employeeIsActive, userIsActive);
            Assert.False(result);
        }

        private class TestAllocation
        {
            public DateOnly StartDate { get; set; }
            public DateOnly EndDate { get; set; }
            public int Utilization { get; set; }
        }
    }
}