using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Errandscall.Models
{
    public class AlertTypes
    {
        public const string Success = "success";
        public const string Info = "info";
        public const string Warning = "warning";
        public const string Danger = "danger";
    }

    public enum Roles
    {
        Administrotor = 1,
        Client = 2,
        Employee = 3,
    }

    public enum PlanType
    {
        Bronze = 1,
        Silver,
        Gold,
        Platinum,
        Diamond,
    }
    
    public enum Language
    {
        English = 1,
        Afrikaans,
    }

}

