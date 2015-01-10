using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranger.TestConsole
{
    public class CandidateRepo
    {
        private DavinciTsDbContext _dbContext = null;

        public CandidateRepo() 
        {
            this._dbContext = new DavinciTsDbContext();
        }

        public CandidateRepo(string ConnectionString)
        {
            this._dbContext = new DavinciTsDbContext(ConnectionString);
        }

        public void Insert(Candidate Candidate)
        {
            this._dbContext.Candidates.Create(Candidate);
        }

        public void Update(Candidate Candidate)
        {
            this._dbContext.Candidates.Update(Candidate);
        }

        public void Delete(Candidate Candidate)
        {
            this._dbContext.Candidates.Delete(Candidate);
        }

        public Candidate GetById(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return null;

            var query = this._dbContext.Candidates.Where(a => a.Id == Id).Select();

            if (query.Any())
                return query.First();
            else
                return null;
        }

        public Candidate GetByTestTakerNumber(string TakerNumber)
        {
            var query = this._dbContext.Candidates.Where(a => a.TakerNumber == TakerNumber).Select();

            if (query.Any())
                return query.First();
            else
                return null;
        }

        public Candidate GetByEmail(string Email)
        {
            var query = this._dbContext.Candidates.Where(a => a.Email == Email).Select();

            if (query.Any())
                return query.First();
            else
                return null;
        }

        public IEnumerable<Candidate> List()
        {
            return this._dbContext.Candidates.Select();
        }

        public IEnumerable<Candidate> ListByFilter(string NameFilter, string PhoneFilter, string EmailFilter)
        {
            return this._dbContext.Candidates.Where(c => 
                (c.NameCht.Contains(NameFilter) || c.NameEng.Contains(NameFilter)) && 
                 c.Phone.Contains(PhoneFilter) && 
                 c.Email.Contains(EmailFilter)).Select();
        }
    }
}
