using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiii.DTO;
using WebApiii.Models;


namespace WebApiii.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostingsController : ControllerBase
    {
        private readonly JobPostingsContext _context;

        public JobPostingsController(JobPostingsContext context)
        {
            _context = context;
        }

        private static JobPostingDTO JobPostingToDTO(JobPosting jp)
        {
            var entity = new JobPostingDTO();

            if (jp != null)
            {
                entity.JobId = jp.JobId;
                entity.Title = jp.Title;
                entity.PersonName = jp.PersonName;
                entity.Phone = jp.Phone;
                entity.Address = jp.Address;
                entity.JobDescription = jp.JobDescription;
                entity.EmploymentType = jp.EmploymentType;
                entity.JobIsActive = jp.JobIsActive;

            }
            return entity;
        }


        // GET: api/JobPostings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPostingDTO>>> GetJobPostings()
        {
            var jobPostings = await _context.JobPostings
                .Where(jp => jp.JobIsActive) // Örneğin, sadece aktif iş ilanlarını getir
                .Select(jp => JobPostingToDTO(jp))
                .ToListAsync();

            return Ok(jobPostings);
        }

        // GET: api/JobPostings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobPostingDTO>> GetJobPosting(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return JobPostingToDTO(jobPosting);
        }

        // POST: api/JobPostings
        [HttpPost]
        public async Task<IActionResult> CreateJobPosting(JobPostingDTO jobPostingDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = new JobPosting
            {
                Title = jobPostingDTO.Title,
                PersonName = jobPostingDTO.PersonName,
                Phone = jobPostingDTO.Phone,
                Address = jobPostingDTO.Address,
                JobDescription = jobPostingDTO.JobDescription,
                EmploymentType = jobPostingDTO.EmploymentType,
                JobIsActive = jobPostingDTO.JobIsActive
            };

            _context.JobPostings.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetJobPostings), new { id = entity.JobId }, JobPostingToDTO(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJobPosting(int id, JobPostingDTO jobPostingDTO)
        {
            if (id != jobPostingDTO.JobId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            _context.Entry(jobPosting).CurrentValues.SetValues(jobPostingDTO);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!JobPostingExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }


        // DELETE: api/JobPostings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobPosting(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            _context.JobPostings.Remove(jobPosting);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobPostingExists(int id)
        {
            return _context.JobPostings.Any(jp => jp.JobId == id);
        }

    }
}
