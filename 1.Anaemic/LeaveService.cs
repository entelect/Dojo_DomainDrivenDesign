using System;
using System.Collections.Generic;
using System.Linq;

namespace _1.Anaemic
{
    public class LeaveService
    {
        private readonly ILeaveRepository leaveRepository;

        public LeaveService(ILeaveRepository leaveRepository)
        {
            this.leaveRepository = leaveRepository;
        }

        public void CreateLeave(LeaveInputModel input)
        {
            LeaveEntry leave = new LeaveEntry
                                   {
                                       EmployeeId = input.EmployeeId,
                                       StartDate = input.StartDate,
                                       EndDate = input.EndDate,
                                       LeaveType = input.LeaveType,
                                       LeaveStatus = LeaveStatus.Pending,
                                    };
            var escalationList = this.GetApprovers(input.EmployeeId, input.LeaveType);
            leave.Approvers.AddRange(escalationList);
            leave.CurrentApprover = leave.Approvers.First();
            this.leaveRepository.Insert(leave);
        }

        private IEnumerable<Employee> GetApprovers(int employeeId, LeaveType leaveType)
        {
            switch(leaveType)
            {
                case LeaveType.Annual: //(Team Lead -> Account Manager -> SignOff)
                    throw new NotImplementedException();
                case LeaveType.Study: //(Account Manager -> SignOff)
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("leaveType");
            }
        }

        public void Escalate(int leaveId)
        {
            var leave = this.leaveRepository.FindById(leaveId);

            var approverIndex = leave.Approvers.FindIndex(x => x.EmployeeId == leave.CurrentApprover.EmployeeId);
            if(approverIndex < leave.Approvers.Count - 1) //todo make better if time left
            {
                leave.CurrentApprover = leave.Approvers.ElementAt(approverIndex + 1);
            }
            else
            {
                throw new Exception("Cannot Excalate");
            }
            this.leaveRepository.Update(leave);
        }

        public void UpdateStatus(int leaveId, LeaveStatus leaveStatus, Employee actionEmployee)
        {
            var leave = this.leaveRepository.FindById(leaveId);
            if(this.CanChangeLeaveStatus(leave, leaveStatus, actionEmployee))
            {
                leave.LeaveStatus = leaveStatus;
            }
            else
            {
                throw new Exception("Not Allowed #YOLO");
            }
            this.leaveRepository.Update(leave);
        }

        private bool CanChangeLeaveStatus(LeaveEntry leave, LeaveStatus leaveStatus, Employee actionEmployee)
        {
            switch(leaveStatus)
            {
                case LeaveStatus.Pending:
                    return true;
                case LeaveStatus.Approved:
                    return leave.CurrentApprover.EmployeeId == actionEmployee.EmployeeId;
                case LeaveStatus.Finalized:
                    if(leave.LeaveStatus != LeaveStatus.Approved)
                    {
                        throw new ArgumentException("Cannot Finalize Leave");
                    }
                    return actionEmployee.IsManager();
                default:
                    throw new ArgumentOutOfRangeException("leaveStatus");
            }
        }

        public void ChangeLeaveType(int leaveId, LeaveType leaveType)
        {
            var leave = this.leaveRepository.FindById(leaveId);
            var escalationList = this.GetApprovers(leave.EmployeeId, leaveType);
            leave.Approvers.Clear();
            leave.Approvers.AddRange(escalationList);
            leave.CurrentApprover = leave.Approvers.First();
            this.leaveRepository.Update(leave);
        }
    }

    public interface ILeaveRepository 
    {
        void Insert(LeaveEntry leave);

        LeaveEntry FindById(int leaveId);

        void Update(LeaveEntry leave);
    }

    public class LeaveEntry
    {
        public LeaveEntry()
        {
            this.Approvers = new List<Employee>();
        }

        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public LeaveType LeaveType { get; set; }

        public LeaveStatus LeaveStatus { get; set; }

        public List<Employee> Approvers { get; private set; }

        public Employee CurrentApprover { get; set; }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public bool IsManager()
        {
            throw new NotImplementedException();
        }
    }

    public enum LeaveType
    {
        Annual,
        Study
    }

    public enum LeaveStatus
    {
        Pending,
        Approved,
        Finalized
    }

    public class LeaveInputModel
    {
        public int EmployeeId { get; set; }

        public LeaveType LeaveType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
