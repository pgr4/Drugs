using Drugs.Library.Models;
using Drugs.Library.Repository;

namespace Drugs.Library
{
    public class Service
    (
        DrugSqliteRepository drugRepository, 
        SideEffectSqliteRepository sideEffectRepository, 
        CategorySqliteRepository categoryRepository, 
        DrugSideEffectLinkSqliteRepository drugSideEffectLinkRepository,
        DrugCategoryLinkSqliteRepository drugCategoryLinkRepository
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

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await categoryRepository.GetAllAsync();
        }

        public async Task<IEnumerable<DrugFullInformation>> GetAllDrugsAsync(IEnumerable<Category> categories = null)
        {
            if (categories == null || !categories.Any())
            {
                var drugs = await drugRepository.GetAllAsync();
                return await Task.WhenAll(drugs.Select(drug => GetDrugInformationAsync(drug)));
            }
            else
            {
                var drugCategoryLinks = await Task.WhenAll(categories.Select(category => drugCategoryLinkRepository.GetDrugCategoryLinksByCategoryIdAsync(category.CategoryId)));
                var drugs = await Task.WhenAll(drugCategoryLinks.SelectMany(t => t.Select(drugCategoryLink => drugCategoryLink.DrugId)).Distinct().Select(drugId => drugRepository.GetDrugByIdAsync(drugId)));
                return await Task.WhenAll(drugs.Select(drug => GetDrugInformationAsync(drug)));
            }
        }

        public async Task CreateDrugAsync(Drug drug)
        {
            await drugRepository.AddAsync(drug);
        }
        
        public async Task CreateSideEffectAsync(SideEffect sideEffect)
        {
            await sideEffectRepository.AddAsync(sideEffect);
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await categoryRepository.AddAsync(category);
        }

        public async Task CreateDrugSideEffectLinkAsync(DrugSideEffectLink drugSideEffectLink)
        {
            await drugSideEffectLinkRepository.AddAsync(drugSideEffectLink);
        }

        public async Task CreateDrugCategoryLinkAsync(DrugCategoryLink drugCategoryLink)
        {
            await drugCategoryLinkRepository.AddAsync(drugCategoryLink);
        }

        private async Task<DrugFullInformation> GetDrugInformationAsync(Drug drug)
        {
            var drugSideEffectLinks = await drugSideEffectLinkRepository.GetDrugSideEffectLinksByDrugIdAsync(drug.DrugId);

            var sideEffects = await Task.WhenAll(drugSideEffectLinks.Select(async drugSideEffectLink =>
            {
                return await sideEffectRepository.GetSideEffectByIdAsync(drugSideEffectLink.SideEffectId);
            }));

            var drugCategoryLinks = await drugCategoryLinkRepository.GetDrugCategoryLinksByDrugIdAsync(drug.DrugId);

            var categories = await Task.WhenAll(drugCategoryLinks.Select(async drugCategoryLink =>
            {
                return await categoryRepository.GetCategoryByIdAsync(drugCategoryLink.CategoryId);
            }));

            return new DrugFullInformation(drug, sideEffects, categories);
        }
    }
}