using Common.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mfcu.Repositories.AppFramework
{
    public class FileUploadRepo : RepoBase<DocumentUpload>
    {
        public FileUploadRepo(DbContext context)
            : base(context)
        {

        }

        public string GetFormerArchivePath()
        {
            var results = ExecuteScalarFunc<string>("DocumentUdfGetAttachmentDirectory");

            return results.First();
        }
    }
}
