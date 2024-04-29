using Microsoft.AspNetCore.Identity;

namespace WebApiii.Models
{
    public class AppUser:IdentityUser<int>
    {
        public string FullName { get; set; } = null!;  //
        public DateTime DateAdded {  get; set; }    //kullanıcının ne zamn oluşturulduğu bilgisi
    }
}
