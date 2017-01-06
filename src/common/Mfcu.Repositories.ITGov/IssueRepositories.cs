using Common.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mfcu.Repositories.ITGov
{
    public class DepartmentRepo : RepoBase<Department>
    {
        public DepartmentRepo(DbContext context) : base(context) { }

        public Task<List<Department>> GetActiveDepts()
        {
            return Entities.Where(d => d.boolIsActive == true).ToListAsync<Department>();
        }
    }

    public class CoreSystemRepo : RepoBase<CoreSystem>
    {
        public CoreSystemRepo(DbContext context) : base(context) { }

        public async Task<List<CoreSystem>> GetAllAsync()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            var items = await GetAsync(t => t.boolIsActive == true, t =>  t.OrderBy(i => i.SystemDescr));

            return items.ToList();
        }

        public async Task<List<CoreSystem>> GetVendorSystemsAsync(int intVendorID)
        {
            Context.Configuration.LazyLoadingEnabled = false;
            var items = await GetAsync(t => t.boolIsActive == true && (t.VendorID == intVendorID || t.VendorID == null), t => t.OrderBy(i => i.SystemDescr));

            return items.ToList();
        }
    }

    public class SystemUserRepo : RepoBase<SystemUser>
    {
        public SystemUserRepo(DbContext context) : base(context) { }

        public async Task<List<SystemUser>> GetAllAsync()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            var items = await GetAsync(t => t.boolIsActive == true && t.boolIsIncidentOwner == true, t => t.OrderBy(i => i.UserName));

            return items.ToList();
        }
    }

    public class VendorRepo : RepoBase<Vendor>
    {
        public VendorRepo(DbContext context) : base(context) { }

        public async Task<List<Vendor>> GetAllAsync()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            var items = await GetAsync(t => t.boolIsActive == true, t => t.OrderBy(i => i.VendorDescr));

            return items.ToList();
        }
    }

    public class IssueRepo : RepoBase<Tracking>
    {
        public IssueRepo(DbContext context) : base(context) { }
    }

    public class IssueViewRepo : RepoBase<vwTracking>
    {
        public IssueViewRepo(DbContext context) : base(context) { }

        public Task<List<vwTracking>> GetAllAsync()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            return Entities.ToListAsync<vwTracking>();
        }

        public Task<List<vwTracking>> GetIssuesAsync(bool boolIsResolved)
        {
            Context.Configuration.LazyLoadingEnabled = false;
            if (boolIsResolved)
            {
                return Entities.ToListAsync<vwTracking>();
            }
            else
            {
                return Entities.Where(i => i.ResolvedDate == null).ToListAsync<vwTracking>();
            }
        }

        public Task<vwTracking> GetAsync(int intIncidentTrackingID)
        {
            return Entities.Where(i => i.IncidentTrackingID == intIncidentTrackingID).FirstOrDefaultAsync();
        }
    }

    public class CommentRepo : RepoBase<Comment>
    {
        public CommentRepo(DbContext context) : base(context) { }

        public async Task<List<CommentDTO>> GetIssueCommentsAsync(int intIncidentTrackingID, string strDomainLogin)
        {
            Context.Configuration.LazyLoadingEnabled = false;

            var comments = await Entities.Where(i => i.boolDeleted == false && i.IncidentTrackingID == intIncidentTrackingID).Select(c => new CommentDTO()
            {
                CommentID = c.CommentID,
                Comment = c.Comment1,
                IncidentTrackingID = c.IncidentTrackingID,
                created_by = c.created_by,
                created_dt = c.created_dt,
                modified_by = c.modified_by,
                modified_dt = c.modified_dt,
                boolUpdatable = (c.created_by.ToLower().Equals(strDomainLogin) ? true : false)
            }).ToListAsync();

            return comments;
        }
    }

    public class IssueDeptRepo : RepoBase<TrackingDepartment>
    {
        public IssueDeptRepo(DbContext context) : base(context) { }

        public async Task<List<IssueDeptDTO>> GetIssueDepartmentsAsync(int intIncidentTrackingID)
        {
            var departments = await Entities.Where(i => i.IncidentTrackingID == intIncidentTrackingID).Select(i => new IssueDeptDTO
            {
                DepartmentID = i.DepartmentID,
                CostCenterNumber = i.Department.CostCenterNumber,
                DepartmentCode = i.Department.DepartmentCode,
                DepartmentDescr = i.Department.DepartmentDescr,
            }).ToListAsync();

            return departments;
        }

        public override async Task CreateAsync(TrackingDepartment entity)
        {
            TrackingDepartment existedDept = await Entities.Where(i => i.IncidentTrackingID == entity.IncidentTrackingID &&
                                                                              i.DepartmentID == entity.DepartmentID).FirstOrDefaultAsync();

            if (existedDept != null)
            {
                throw new Exception("Can't add this issue department which already existed.");
            }

            await base.CreateAsync(entity);
        }

        public override async Task DeleteAsync(TrackingDepartment entity)
        {
            TrackingDepartment existingDept = await Entities.Where(i => i.IncidentTrackingID == entity.IncidentTrackingID &&
                                                                                 i.DepartmentID == entity.DepartmentID).FirstOrDefaultAsync();
            if (existingDept == null)
            {
                throw new Exception("This issue department doesn't exist.");
            }

            await base.DeleteAsync(existingDept);
        }
    }

    public class AttachmentRepo : RepoBase<Attachment>
    {
        public AttachmentRepo(DbContext context) : base(context) { }

        public async Task<List<Attachment>> GetIssueAttachmentsAsync(int intIncidentTrackingID)
        {
            Context.Configuration.LazyLoadingEnabled = false;
            return await (Entities.Where(i => i.IncidentTrackingID == intIncidentTrackingID).ToListAsync());
        }

        public Task<usp_get_AttachmentFileName_Result> GetIssueAttachmentFileName(int? intSurrogateID, string strSystemType, string strSourceFile)
        {
            var intSurrogateIDParameter = intSurrogateID.HasValue ?
                new ObjectParameter("intSurrogateID", intSurrogateID) :
                new ObjectParameter("intSurrogateID", typeof(int));

            var strSystemTypeParameter = strSystemType != null ?
                new ObjectParameter("strSystemType", strSystemType) :
                new ObjectParameter("strSystemType", typeof(string));

            var strSourceFileParameter = strSourceFile != null ?
                new ObjectParameter("strSourceFile", strSourceFile) :
                new ObjectParameter("strSourceFile", typeof(string));

            var result = ExecuteSproc<usp_get_AttachmentFileName_Result>("usp_get_AttachmentFileName", intSurrogateIDParameter,
                                                                                                       strSystemTypeParameter,
                                                                                                       strSourceFileParameter).FirstOrDefault();

            return Task.FromResult<usp_get_AttachmentFileName_Result>(result);
        }

        public override async Task CreateAsync(Attachment attachment)
        {
            Attachment existingAttachment = await Entities.Where(i => i.IncidentTrackingID == attachment.IncidentTrackingID && i.DocumentID == attachment.DocumentID).FirstOrDefaultAsync();

            if (existingAttachment != null)
            {
                throw new Exception("Can't add. This issue attachment already existed.");
            }

            await base.CreateAsync(attachment);
        }

        public override async Task DeleteAsync(Attachment attachment)
        {
            Attachment existingAttachment = await Entities.Where(i => i.IncidentTrackingID == attachment.IncidentTrackingID && i.DocumentID == attachment.DocumentID).FirstOrDefaultAsync();

            if (existingAttachment == null)
            {
                throw new Exception("This issue document doesn't exist.");
            }

            await base.DeleteAsync(existingAttachment);
        }
    }

    public class IncidentStatusRepo : RepoBase<IncidentStatus>
    {
        public IncidentStatusRepo(DbContext context) : base(context) { }

        public async Task<List<IncidentStatus>> GetAllAsync()
        {
            Context.Configuration.LazyLoadingEnabled = false;
            var items = await GetAsync(s => s.boolIsActive == true, s => s.OrderBy(i => i.StatusDescr));

            return items.ToList();
        }
    }
}
