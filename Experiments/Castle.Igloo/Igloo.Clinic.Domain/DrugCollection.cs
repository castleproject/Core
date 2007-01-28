using System.Collections.Generic;

namespace Igloo.Clinic.Domain
{
    public class DrugCollection : List<Drug>
    {
        public void Remove(long id)
        {
            Drug drug = this.FindById(id);
            if (drug != null)
            {
                base.Remove(drug);
            }
        }

        public Drug FindById(long id)
        {
            foreach (Drug drug in this)
            {
                if (drug.Id.Equals(id))
                {
                    return drug;
                }
            }
            return null;
        }
    }

}
