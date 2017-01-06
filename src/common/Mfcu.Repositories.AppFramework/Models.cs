using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mfcu.Repositories.AppFramework
{
    public class DocumentModel
    {
        public string laterFilePath { get; set; }
        public string description { get; set; }
        public string modifiedBy { get; set; }
    }

    public class DeleteDocumentModel
    {
        public bool IsDeleted { get; set; }
    }

    public class FilesStatus
    {
        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public long size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }
        public int documentId { get; set; }
        public string description { get; set; }
        public string path { get; set; }
        public string created_by { get; set; }
        public DateTime created_dt { get; set; }
        public string modified_by { get; set; }
        public DateTime modified_dt { get; set; }

        public FilesStatus() { }

        public bool isDeleted { get; set; }
    }

    public class RestoreFromArchiveModel
    {
        public int DocumentId { get; set; }
        public string OfflineArchiveFilePath { get; set; }
        public string OfflineFilePath { get; set; }
        public string OnlineFilePath { get; set; }
    }
}
