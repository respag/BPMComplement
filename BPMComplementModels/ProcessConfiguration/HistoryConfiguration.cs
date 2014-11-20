using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ultimus.ComponentManager.Models
{
    [Serializable]
    //[Table("HistoryConfiguration", Schema = "Process")]
    public class HistoryConfiguration
    {
        #region IdConfiguration

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdConfiguration { get; set; }
        
        #endregion

        #region IdProcess -> Foreing Key

        [DisplayName("Process")]
        [Range(1, 100, ErrorMessage = "Please choose a Process")]
        public int IdProcess { get; set; }
        public virtual CatProcesses CatProcesses { get; set; }
        
        #endregion

        [Required]
        [Display(Name = "Enable Completed Task Deleted Event")]
        public bool CompletedTaskDeleted { get; set; }

        [Required]
        [Display(Name = "Enable Incident Aborted Event")]
        public bool IncidentAborted { get; set; }

        [Required]
        [Display(Name = "Enable Incident Completed Event")]
        public bool IncidentCompleted { get; set; }

        [Required]
        [Display(Name = "Enable Incident Initiated Event")]
        public bool IncidentInitiated { get; set; }

        [Required]
        [Display(Name = "Enable Step Aborted Event")]
        public bool StepAborted { get; set; }

        [Required]
        [Display(Name = "Enable Task Activated Event")]
        public bool TaskActivated { get; set; }

        [Required]
        [Display(Name = "Enable Task Assigned Event")]
        public bool TaskAssigned { get; set; }

        [Required]
        [Display(Name = "Enable Task Completed Event")]
        public bool TaskCompleted { get; set; }

        [Required]
        [Display(Name = "Enable Task Conferred Event")]
        public bool TaskConferred { get; set; }

        [Required]
        [Display(Name = "Enable Task Delayed Event")]
        public bool TaskDelayed { get; set; }

        [Required]
        [Display(Name = "Enable Task Late Event")]
        public bool TaskLate { get; set; }

        [Required]
        [Display(Name = "Enable Task Resubmitted Event")]
        public bool TaskResubmitted { get; set; }

        [Required]
        [Display(Name = "Enable Task Returned Event")]
        public bool TaskReturned { get; set; }

        [Required]
        [Display(Name = "Enable Task Submit Failed Event")]
        public bool TaskSubmitFailed { get; set; }

        [Required]
        [Display(Name = "Enable Tasks Per Day Threshold Reached Event")]
        public bool TasksPerDayThresholdReached { get; set; }

        [Required]
        [Display(Name = "Enable Queue Task Activated Event")]
        public bool QueueTaskActivated { get; set; }

        [Required]
        [Display(Name = "Enable Task Deleted On Min Response Complete Event")]
        public bool TaskDeletedOnMinResponseComplete { get; set; }
    }
}