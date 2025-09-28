using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace generator_web.Models
{
    public class user_data
    {
        [Key]
        public int Id { get; set; }
        //[ForeignKey("UserId")]
        //public int UserId { get; set; }
        public string command{ get; set; }
        public DateTime DateTime{ get; set; }
        //public string status {get;set;} Kabul edildi mi, edilmedi mi?(saved or unsaved)
    }
}
//dayı bir şey dicem bizim şuanki db'deki user_data tablosunda şöyle bir durumumuz var: kullanıcı tablosu oluşturmalıyız,
//rol de katarız sonra o kullanıcı id'si ile hangi kullanıcının o komutu yaptığına erişebiliriz. 