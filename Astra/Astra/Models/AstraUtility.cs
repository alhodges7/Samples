using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.CompositeRepository;
using Astra.Models.ViewModels;

namespace Astra.Models
{
    public class AstraUtility
    {
        private const int ADDED_LESS_THAN_DAYS = 30;

        public static List<Resource> GetAllSearch()
        {
            using (var composite = new ScopedCompositeRepository())
            {
                var searchResult = composite.Repositories.ResourceRepository.AllCommittedIncluding(null, "CoverImage").OrderBy(x => x.Title);
                return (List<Resource>)searchResult.ToList();
            }
        }

        public static List<Resource> DoKeyWordSearch(List<string> keyWords = null)
        {
            using (var repository = new ScopedCompositeRepository().Repositories.ResourceRepository)
            {
                var searchResult = repository.FindByKeyWord(keyWords).OrderBy(x => x.Title);
                return (List<Resource>)searchResult.ToList();
            }
        }

        public static List<Resource> DoAdvancedSearch(Resource searchValues = null)
        {
            List<Resource> searchResult = new List<Resource>();
            using (var repository = new ScopedCompositeRepository().Repositories)
            {
                searchResult = repository.ResourceRepository.AdvancedSearch(searchValues);
                return searchResult;
            }
        }

        public static List<Resource> GetRecentlyAddedItems()
        {
            using (var composite = new ScopedCompositeRepository())
            {
                var searchResult = composite.Repositories.ResourceRepository.AllCommittedIncluding(null, "CoverImage").OrderBy(x => x.Title);
                var result = from d in searchResult
                             where DateTime.Now.Subtract(d.CreatedOn).Days <= ADDED_LESS_THAN_DAYS
                             select d;
                return (List<Resource>)searchResult.ToList();
            }
        }

        public static List<Resource> GetRecommendations(List<Resource> recentlyViewed)
        {
            List<string> keyWords = new List<string>();
            foreach(Resource resource in recentlyViewed)
            {
                List<string> resourceKeyWords = new List<string>();
                resourceKeyWords =  resource.KeyWords;
                foreach (string word in resourceKeyWords)
                {
                    if (!keyWords.Contains(word))
                        keyWords.Add(word);
                }
            }
            List<Resource> recommendedList = new List<Resource>();
            recommendedList = DoKeyWordSearch(keyWords).OrderByDescending(x => x.Rating).ToList();
            recommendedList = RemoveDuplicates(recentlyViewed, recommendedList);
            return recommendedList;
        }

        public static List<Resource> RemoveDuplicates(List<Resource> list_A, List<Resource> list_B)
        {
            foreach (Resource resource in list_A)
            {
                for (int i = 0; i < list_B.Count(); i++)
                {
                    if (resource.ResourceID == list_B.ElementAt(i).ResourceID)
                    {
                        list_B.RemoveAt(i);
                       
                    }
                }
            }
            return list_B;
        }

    }
}