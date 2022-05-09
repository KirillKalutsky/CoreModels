using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels.DBModels
{
    public enum IncidentCategory
    {
        Fire,

    }

    public static class IncidentCategoryExtensions
    {           
        public static string GetName(this IncidentCategory incigentCategory)
        {
            switch (incigentCategory)
            {
                case IncidentCategory.Fire:
                    return "Пожар";
                default:
                    throw new ArgumentException($"Нет реализации для: {incigentCategory}");
            }
        }
    }
}
