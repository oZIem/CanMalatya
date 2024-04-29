namespace WebApiii.DTO
{
    public class JobPostingDTO
    {
        public int JobId { get; set; }
        public bool JobIsActive { get; set; }
       
        public string Title { get; set; }
        public string PersonName { get; set; }
        public string Phone { get; set; } 
        public string Address { get; set; } 
        public string JobDescription { get; set; } 
        public string EmploymentType { get; set; }
  

    }
}
