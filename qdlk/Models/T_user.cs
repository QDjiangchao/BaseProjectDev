using System;
namespace qdlk.Models
{
    public class T_user
    {
        public Guid id { get; set; }
        public string cusercode { get; set; }
        public string cusername { get; set; }
        public string cpwd { get; set; }
        public string cmemo { get; set; }
        public int isex { get; set; }
        public DateTime dcreatedate { get; set; }
        public DateTime dupdatedate { get; set; }
        public int idel { get; set; }
    }
}
