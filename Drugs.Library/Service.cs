using Drugs.Library.Models;
using Drugs.Library.Repository;

namespace Drugs.Library
{
    public class Service
    (
        DrugSqliteRepository drugRepository, 
        SideEffectSqliteRepository sideEffectRepository, 
        DrugSideEffectLinkSqliteRepository drugSideEffectLinkRepository
)
    {
        /// <summary>
        /// This is a test method to check if the database is working.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> HasDrugsAsync()
        {
            var result = await drugRepository.GetDrugByIdAsync(1);
            return result != null;
        }

        public async Task CreateDrugAsync(Drug drug)
        {
            await drugRepository.AddAsync(drug);
        }
        
        public async Task CreateSideEffectAsync(SideEffect sideEffect)
        {
            await sideEffectRepository.AddAsync(sideEffect);
        }

        public async Task CreateDrugSideEffectLinkAsync(DrugSideEffectLink drugSideEffectLink)
        {
            await drugSideEffectLinkRepository.AddAsync(drugSideEffectLink);
        }

        public async Task<IEnumerable<Drug>> GetDrugsBySimilarNameAsync(string name)
        {
            return await drugRepository.GetDrugsBySimilarNameAsync(name);
        }

        public async Task<DrugFullInformation> GetDrugInformationAsync(Drug drug)
        {
            var drugSideEffectLinks = await drugSideEffectLinkRepository.GetDrugSideEffectLinksByDrugIdAsync(drug.DrugId);

            var sideEffects = await Task.WhenAll(drugSideEffectLinks.Select(async drugSideEffectLink =>
            {
            return await sideEffectRepository.GetSideEffectByIdAsync(drugSideEffectLink.SideEffectId);
            }));

            return new DrugFullInformation(drug, sideEffects);
        }
    }
}