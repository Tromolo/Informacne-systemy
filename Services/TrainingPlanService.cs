using System.Collections.Generic;
using VIS_projekt.Domain; 

namespace VIS_projekt.Services
{
    public class TrainingPlanService
    {
        public TrainingPlanService()
        {

        }

        public void AddTrainingPlan(int userId, string description, bool active = true)
        {
            var trainingPlan = new TrainingPlan(userId, description, active);
            trainingPlan.Save();
        }

        public List<TrainingPlan> GetTrainingPlansByUserId(int userId)
        {
            return TrainingPlan.FindByUserId(userId);
        }

        public List<TrainingPlan> GetTrainingPlansByTrainerId(int trainerId)
        {
            return TrainingPlan.FindByTrainerId(trainerId);
        }

        public TrainingPlan? GetTrainingPlanById(int id)
        {
            return TrainingPlan.FindById(id);
        }

        public void UpdateTrainingPlan(TrainingPlan plan)
        {
            plan.Save();
        }

        public void DeleteTrainingPlan(int id)
        {
            var plan = TrainingPlan.FindById(id);
            if (plan != null)
            {
                plan.Delete();
            }
        }
    }
}
