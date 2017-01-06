using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mfcu.Repositories.ITGov
{
    public class SystemUserDTO
    {
        public int SystemUserID { get; set; }
        public string DomainLogin { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public bool boolIsCobitOwner { get; set; }
        public bool boolIsActive { get; set; }
    }

    public class StatusDTO
    {
        public int StatusID { get; set; }
        public string StatusType { get; set; }
        public string StatusDescr { get; set; }
        public bool boolIsActive { get; set; }
    }

    public class ProcessDTO
    {
        public int ProcessID { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDescr { get; set; }
        public string ProcessPurposeStatement { get; set; }
        public int DomainID { get; set; }
        public string DomainDescr { get; set; }
        public bool boolIsActive { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }

        public string CreatedDateVw
        {
            get
            {
                return created_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string ModifiedDateVw
        {
            get
            {
                return modified_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }

    public class PriorityDTO
    {
        public int PriorityID { get; set; }
        public string PriorityDescr { get; set; }
        public bool boolIsActive { get; set; }
    }

    public class PracticeDTO
    {
        // table
        public int PracticeID { get; set; }
        public string PracticeCode { get; set; }
        public string PracticeName { get; set; }
        public string PracticeDescr { get; set; }
        public string Certification { get; set; }
        public int ProcessID { get; set; }
        public int? OwnerID { get; set; }
        public int? PriorityID { get; set; }
        public int? StatusID { get; set; }
        public bool boolIsActive { get; set; }
        public DateTime? TargetDate { get; set; }
        public string TargetDateVw
        {
            get
            {
                return ((TargetDate == null) ? string.Empty : Convert.ToDateTime(TargetDate).ToShortDateString());
            }
        }
        public string strTargetDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string CompletionDateVw
        {
            get
            {
                return ((CompletionDate == null) ? string.Empty : Convert.ToDateTime(CompletionDate).ToShortDateString());
            }
        }
        public string strCompletionDate { get; set; }
        public DateTime? CompletionDateTimeStamp { get; set; }

        // view
        public int AreaID { get; set; }
        public int DomainID { get; set; }
        public string AreaDescr { get; set; }
        public string DomainDescr { get; set; }
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDescr { get; set; }
        public string ProcessPurposeStatement { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmailAddress { get; set; }
        public string PriorityDescr { get; set; }
        public string StatusDescr { get; set; }
        public string AssignedSystemUser { get; set; }
        public string AssignedSystemUserID { get; set; }
        public bool boolHasAttachment { get; set; }
        public bool boolHasComment { get; set; }
        public bool boolHasDocLink { get; set; }
    }

    public class LinkDTO
    {
        public int LinkID { get; set; }
        public string LinkDescr { get; set; }
        public string LinkName { get; set; }
        public string LinkPath { get; set; }
        public int CobitPracticeID { get; set; }
        public bool boolDeleted { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }

        public string CreatedDateVw
        {
            get
            {
                return created_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string ModifiedDateVw
        {
            get
            {
                return modified_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }

    public class IssueDeptDTO
    {
        public int DepartmentID { get; set; }
        public int? CostCenterNumber { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentDescr { get; set; }
    }

    public class DocumentDTO
    {
        public string laterFilePath { get; set; }
        public string description { get; set; }
        public string modifiedBy { get; set; }
    }

    public class IssueDocument
    {
        public int documentId { get; set; }
        public bool isDeleted { get; set; }
    }

    public class DomainDTO
    {
        public int DomainID { get; set; }
        public int AreaID { get; set; }
        public string AreaDescr { get; set; }
        public string DomainDescr { get; set; }
        public bool boolIsActive { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }

        public string CreatedDateVw
        {
            get
            {
                return created_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string ModifiedDateVw
        {
            get
            {
                return modified_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }

    public class CommentDTO
    {
        public int CommentID { get; set; }
        public string Comment { get; set; }
        public int? CobitPracticeID { get; set; }
        public int? IncidentTrackingID { get; set; }
        public bool boolDeleted { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }
        public bool boolUpdatable { get; set; }

        public string CreatedDateVw
        {
            get
            {
                return created_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string ModifiedDateVw
        {
            get
            {
                return modified_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }

    public class AuditLogDTO
    {
        public int AuditLogID { get; set; }
        public int SurrogateID { get; set; }
        public string SurrogateType { get; set; }
        public string AuditInfo { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangeBy { get; set; }
        public DateTime ChangeDate { get; set; }

        public string ChangeDateVw
        {
            get
            {
                return ChangeDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
    }

    public class AttachmentDTO
    {
        public int AttachmentID { get; set; }
        public string AttachmentDescr { get; set; }
        public string AttachmentFileName { get; set; }
        public int CobitPracticeID { get; set; }
        public bool boolDeleted { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }

        public string CreatedDateVw
        {
            get
            {
                return created_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string ModifiedDateVw
        {
            get
            {
                return modified_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }

    public class AreaDTO
    {
        public int AreaID { get; set; }
        public string AreaDescr { get; set; }
        public bool boolIsActive { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }

        public string CreatedDateVw
        {
            get
            {
                return created_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }

        public string ModifiedDateVw
        {
            get
            {
                return modified_dt.ToString("yyyy-MM-dd HH:mm");
            }
        }
    }
}
