using System;

namespace _2.RichDomain
{
    public class ApproveStep
    {
        public Employee Approver { get; private set; }

        public ApproveStep NexStep { get; set; }

        public ApproveStep(Employee approver)
        {
            this.Approver = approver;
        }
    }

    public class LeaveEntry
    {
        private ApproveStep currentApproveStep;
        private DateTime endDate;
        private DateTime startDate;
        private readonly Employee employee;
        private LeaveType leaveType; //change

        public Employee CurrentApprover
        {
            get
            {
                return this.currentApproveStep.Approver;
            }
        }

        public bool IsApproved
        {
            get
            {
                return this.currentApproveStep == null;
            }
        }

        public virtual bool CanFinalize //change
        {
            get
            {
                return IsApproved && this.leaveType.CanFinalize(); //change
            }
        }

        public LeaveEntry(Employee employee, DateTime startDate, DateTime endDate, LeaveType leaveType)
        {
            this.startDate = startDate;
            this.endDate = endDate;

            this.IsFinalized = false;
            this.employee = employee;

            this.SetLeaveType(leaveType);
        }

        public void SetLeaveType(LeaveType type)
        {
            this.leaveType = type; //change
            this.currentApproveStep = leaveType.CreateApproveSequence(this.employee);
        }

        public void Escalate()
        {
            if(this.currentApproveStep.NexStep == null)
            {
                throw new Exception("Cannot Excalate");
            }

            this.currentApproveStep = this.currentApproveStep.NexStep;
        }

        public void Approve(Employee approver)
        {
            if(this.IsApproved)
            {
                return;
            }

            if(approver == this.currentApproveStep.Approver)
            {
                this.currentApproveStep = null;
            }

            throw new Exception("Not Allowed #YOLO");
        }

        public void FinalizeLeave()
        {
            if(!this.CanFinalize)
            {
                throw new Exception("Not Allowed #YOLO");
            }
            this.IsFinalized = true;
        }

        public bool IsFinalized { get; private set; }
    }

    public abstract class LeaveType
    {
        public abstract ApproveStep CreateApproveSequence(Employee employee);

        public abstract bool CanFinalize(); //change
    }

    public class AnualLeave : LeaveType 
    {
        public override ApproveStep CreateApproveSequence(Employee employee)
        {
            throw new NotImplementedException();
             //(Team Lead -> Account Manager -> SignOff)
        }

        public override bool CanFinalize()
        {
            return true;
        }
    }

    public class StudyLeave : LeaveType 
    {
        public override ApproveStep CreateApproveSequence(Employee employee)
        {
            throw new NotImplementedException();
            //(Account Manager -> SignOff)
        }

        public override bool CanFinalize()
        {
            return true;
        }
    }

    public class SickLeave : LeaveType  //change
    {
        public override ApproveStep CreateApproveSequence(Employee employee)//change
        {
            throw new NotImplementedException();//change
            //(Team Lead -> Account Manager -> SignOff)
        }
        public byte[] SickNote { get; set; } //change
        
        public override bool CanFinalize()
        {
            return this.SickNote != null;
        }
    }

    public class LeaveService
    {
        private readonly IRepository repository;
        private readonly ILeaveTypeFactory leaveTypeFactory;

        public LeaveService(IRepository repository, ILeaveTypeFactory leaveTypeFactory)
        {
            this.repository = repository;
            this.leaveTypeFactory = leaveTypeFactory;
        }

        public void CreateLeave(LeaveInputModel input)
        {
            var leaveType = this.leaveTypeFactory.Make(input.LeaveType);
            var employee = this.repository.FindEmployeeById(input.EmployeeId);
            var leave = new LeaveEntry(employee, input.StartDate, input.EndDate, leaveType);
            this.repository.InsertLeave(leave);
        }

        public void Escalate(int leaveId)
        {
            LeaveEntry leave = this.repository.FindLeaveById(leaveId);
            leave.Escalate();
            this.repository.UpdateLeave(leave);
        }

        public void FinalizeLeave(int leaveId)
        {
            LeaveEntry leave = this.repository.FindLeaveById(leaveId);
            leave.FinalizeLeave();
            this.repository.UpdateLeave(leave);
        }

        public void Approve(int leaveId, int approverId)
        {
            var leave = this.repository.FindLeaveById(leaveId);
            var approver = this.repository.FindEmployeeById(approverId);
            leave.Approve(approver);
            this.repository.UpdateLeave(leave);
        }

        public void ChangeLeaveType(int leaveId, string leaveType)
        {
            var leave = this.repository.FindLeaveById(leaveId);
            var type = this.leaveTypeFactory.Make(leaveType);
            leave.SetLeaveType(type);
            this.repository.UpdateLeave(leave);
        }
    }

    public interface ILeaveTypeFactory
    {
        LeaveType Make(string leaveType); //Construct LeaveType. Either Switch on the type or the name or Activator.CreateInstance Based on Some Convention
    }

    public interface IRepository
    {
        void InsertLeave(LeaveEntry leave);

        LeaveEntry FindLeaveById(int leaveId);

        void UpdateLeave(LeaveEntry leave);

        Employee FindEmployeeById(int employeeId);
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

    public class LeaveInputModel
    {
        public int EmployeeId { get; set; }

        public string LeaveType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
