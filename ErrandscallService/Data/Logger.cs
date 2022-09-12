using ErrandscallDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrandscallService.Data
{
    public class Logger
    {

        public bool Log(ServiceMessage serviceMessage)
        {
            ErrandscallEntities db = new ErrandscallEntities();

            try
            {
                serviceMessage.AddedOnDateTime = DateTime.Now;
                db.ServiceMessage.Add(serviceMessage);
                db.SaveChanges();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
