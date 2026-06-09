using System;
using System.Collections.Generic;
using System.Text;
using ZeemanSport.Core.Contracts.Workout;
using ZeemanSport.Core.Workout;

namespace ZeemanSport.Runtime.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly IWorkoutRepository _workoutRepository;

        public WorkoutService(IWorkoutRepository workoutRepository)
        {
            _workoutRepository = workoutRepository;
        }

        public async Task<IReadOnlyCollection<WorkoutResponse>> GetAllAsync()
        {
            IReadOnlyCollection<Workout> workouts = await _workoutRepository.GetAllAsync();

            return workouts.Select(mapToResponse).ToArray();
        }

        public async Task<WorkoutResponse?> GetByIdAsync(int id)
        {
            Workout? workout = await _workoutRepository.GetByIdAsync(id);

            if (workout == null)
                return null;

            return mapToResponse(workout);
        }

        public async Task<WorkoutResponse> CreateAsync(CreateWorkoutRequest request)
        {
            Workout workout = new Workout
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };

            Workout savedWorkout = await _workoutRepository.SaveAsync(workout);

            return mapToResponse(savedWorkout);
        }

        public async Task<WorkoutResponse?> UpdateAsync(int id, UpdateWorkoutRequest request)
        {
            Workout? workout = await _workoutRepository.GetByIdAsync(id);

            if (workout == null)
                return null;

            workout.Name = request.Name;
            workout.Description = request.Description;
            workout.IsActive = request.IsActive;

            Workout savedWorkout = await _workoutRepository.SaveAsync(workout);

            return mapToResponse(savedWorkout);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _workoutRepository.DeleteAsync(id);
        }

        private static WorkoutResponse mapToResponse(Workout workout)
        {
            return new WorkoutResponse
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                IsActive = workout.IsActive
            };
        }
    }
}
