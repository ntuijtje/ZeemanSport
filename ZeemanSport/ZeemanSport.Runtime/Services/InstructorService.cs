using ZeemanSport.Core.Contracts.Instructor;
using ZeemanSport.Core.Instructor;

namespace ZeemanSport.Runtime.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly IInstructorRepository _instructorRepository;

        public InstructorService(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }

        public async Task<IReadOnlyCollection<InstructorResponse>> GetAllAsync()
        {
            IReadOnlyCollection<Instructor> instructors = await _instructorRepository.GetAllAsync();

            return instructors.Select(mapToResponse).ToArray();
        }

        public async Task<InstructorResponse?> GetByIdAsync(int id)
        {
            Instructor? instructor = await _instructorRepository.GetByIdAsync(id);

            if (instructor == null)
                return null;

            return mapToResponse(instructor);
        }

        public async Task<InstructorResponse> CreateAsync(CreateInstructorRequest request)
        {
            Instructor instructor = new Instructor
            {
                Name = request.Name,
                PhotoUrl = request.PhotoUrl,
                IsActive = true
            };

            Instructor savedInstructor = await _instructorRepository.SaveAsync(instructor);

            return mapToResponse(savedInstructor);
        }

        public async Task<InstructorResponse?> UpdateAsync(int id, UpdateInstructorRequest request)
        {
            Instructor? instructor = await _instructorRepository.GetByIdAsync(id);

            if (instructor == null)
                return null;

            instructor.Name = request.Name;
            instructor.PhotoUrl = request.PhotoUrl;
            instructor.IsActive = request.IsActive;

            Instructor savedInstructor = await _instructorRepository.SaveAsync(instructor);

            return mapToResponse(savedInstructor);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _instructorRepository.DeleteAsync(id);
        }

        private static InstructorResponse mapToResponse(Instructor instructor)
        {
            return new InstructorResponse
            {
                Id = instructor.Id,
                Name = instructor.Name,
                PhotoUrl = instructor.PhotoUrl,
                IsActive = instructor.IsActive
            };
        }
    }
}
