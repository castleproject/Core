
using System;
using System.Collections.Generic;
using Castle.Igloo;
using Castle.Igloo.Attributes;
using Castle.Igloo.Controllers;
using Igloo.Clinic.Domain;

namespace Igloo.Clinic.Application
{
    public class DrugController : BaseController
    {
        private DrugCollection _drugs;
        
        [Inject(Scope = ScopeType.Session)]
        public DrugCollection Drugs
        {
            set { _drugs = value; }
        }

        [SkipNavigation]
        public virtual IList<Drug> GetDrugs()
        {
            return _drugs;
        }

        [SkipNavigation]
        public virtual void Create(string name, string description)
        {
            Drug drug = new Drug(DateTime.Now.Ticks, name, description);
            _drugs.Add(drug);
        }

        [SkipNavigation]
        public virtual void Update(long id, string name, string description)
        {
            Drug drug = _drugs.FindById(id);
            drug.Name = name;
            drug.Description = description;           
        }
        
        [SkipNavigation]
        public virtual void Delete(long id)
        {
            _drugs.Remove(id);
        }

        [SkipNavigation]
        public virtual void Delete(long id, int j)
        {
        }
    }
}
