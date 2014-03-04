using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System;
using System.Web;
using System.Web.Caching;
using Astra.CompositeRepository;
using Astra.Controllers.Shared;
using Astra.Helper;
using Astra.Misc;
using Astra.Models;
using Astra.Models.ViewModels;
using Astra.Repositories;
using Astra.Helper;
using MTUtil.List;
using MTUtil.TypeManagement;

namespace Astra.Controllers
{    
    public class KeyWordController : BaseController
    {
        #region VIEW_ACTIONS
        [NoCache]
        [HttpGet]
        public ViewResult Index()
        {
          ViewBag.Title = "Index";

          using (var repository = new ScopedCompositeRepository().Repositories.KeyWordLinksRepository)
          {
            ViewBag.Tallies = repository.GetKeywordLinkTallies();
          }

          return View(this.GetKeyWords());
        }

        [ChildActionOnly]
#if !DEBUG
        [OutputCache(Duration = 86400)]
#endif
        public PartialViewResult KeyWordCloud()
        {
          ViewBag.Title = "Keyword Cloud";

          //get list of keywords
          List<KeyWord> astraKeyWordList = GetKeyWords().ToList();

          //remove any keywords with parentencies - these are generic ones that are not useful here
          for (int i = 0; i < astraKeyWordList.Count(); i++)
          {
            if (astraKeyWordList[i].Word[0] == '(')
            {
              astraKeyWordList.RemoveAt(i); 
            }
          }

          //get resource list
          IEnumerable<Resource> astraResourceList = null;
          using (var repository = new ScopedCompositeRepository().Repositories.ResourceRepository)
          {
            astraResourceList = repository.AllIncluding();
          }
          
          //output KeyWordPercentageList
          List<KeyWordPercentage> astraKeyWordPercentageList = new List<KeyWordPercentage>();

          //get list of keywords w/ the number of times they are used
          foreach (KeyWord astraKeyWord in astraKeyWordList)
          {
            KeyWordPercentage wordPercentage = new KeyWordPercentage();
            wordPercentage.NumberOfTimesUsed = 0; //start of w/ zero keyword usage count
            wordPercentage.Word = astraKeyWord.Word;

            //count the number of times each keyword is used
            foreach (Resource resourceItem in astraResourceList)
            {
              if (resourceItem.KeyWords.Contains(wordPercentage.Word))
              {
                wordPercentage.NumberOfTimesUsed++;
              }
              else
              {
                //split the title on punctuation & most symbols, but ignore any empty strings produced
                string[] titleWords = resourceItem.Title.Split(new string[] { " ", ",", ".", ":", ";", "!", "?", "(", ")", "&", "%", "$", "@" }, StringSplitOptions.RemoveEmptyEntries); //don't split "C#" !!!!

                if (titleWords.Contains(wordPercentage.Word))
                { 
                  wordPercentage.NumberOfTimesUsed++; 
                }
              }
            }
            astraKeyWordPercentageList.Add(wordPercentage);
          }

          //sort based on IComparable function of KeyWordPercentage class
          astraKeyWordPercentageList.Sort();

          KeyWordPercentageDataBank astraKeyWordPercentageDataBank = new KeyWordPercentageDataBank(astraKeyWordPercentageList);

          astraKeyWordPercentageDataBank.CalculateSizesFromStandardDeviation();

          return PartialView(astraKeyWordPercentageDataBank);
        }


        [Authorize(Roles = MembershipHelper.ROLE_LIBRARIAN + "," + MembershipHelper.ROLE_ADMIN)]
        public ActionResult Delete(int id)
        {
          DeleteKeyWord(this.GetKeyWord(id));
          return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
          return View(new KeyWord());
        }

        [HttpPost]
        public ActionResult Create(KeyWord keyWord)
        {
          if (ModelState.IsValid)
          {
            using (var repository = new ScopedCompositeRepository().Repositories.KeywordRepository)
            {
              repository.CreateKeyword(keyWord);
            }
          }

          return RedirectToAction("Index");
        }

        public ActionResult Merge(int id)
        {
          KeyWord targetKeyWord = this.GetKeyWord(id);
          ViewBag.TargetKeyWord = targetKeyWord;
          
          List<SelectListItem> keyWordList = new List<SelectListItem>();

          SelectListItem item = new SelectListItem();
          item.Text = "(pick one)";
          item.Value = string.Empty;
          keyWordList.Add(item);

          foreach (KeyWord keyWord in this.GetKeyWords())
          {
            if (keyWord.KeyWordID == targetKeyWord.KeyWordID)
              continue;

            item = new SelectListItem();
            item.Text = keyWord.Word;
            item.Value = keyWord.KeyWordID.ToString();
            keyWordList.Add(item);
          }
          ViewBag.KeyWordList = keyWordList;

          return View(targetKeyWord);
        }

        [HttpPost]
        public ActionResult Merge()
        {
          int targetKeyWordId = TypeUtils.ToInt(Request.Form["TargetKeyWordId"]);
          if (targetKeyWordId == 0)
            return View(RedirectToAction("Index"));

          int mergeIntoId = TypeUtils.ToInt(Request.Form["MergeIntoKeyWordList"]);
          if (mergeIntoId == 0)
            return View(targetKeyWordId);

          KeyWord targetKeyWord = this.GetKeyWord(targetKeyWordId);

          using (var repositories = new ScopedCompositeRepository())
          {
            IEnumerable<ResourceToKeyWordLink> linksToClone = repositories.Repositories.KeyWordLinksRepository.GetLinksByKeyword(mergeIntoId);
            foreach (ResourceToKeyWordLink linkToClone in linksToClone)
            {
              if (repositories.Repositories.KeyWordLinksRepository.AllIncluding(kwl => kwl.KeyWordID == targetKeyWordId 
                  && kwl.ResourceID == linkToClone.ResourceID).Count() > 0)
                continue;

              Resource resource = repositories.Repositories.ResourceRepository.Find<Resource>(linkToClone.ResourceID);
              resource.KeyWords.Add(targetKeyWord.Word);
              repositories.Repositories.ResourceRepository.InsertOrUpdate(resource);
            }

            repositories.Repositories.ResourceRepository.Save();
          }

          return RedirectToAction("Index");
        }
        #endregion

        #region MISC_FUNCTIONS
        public IEnumerable<KeyWord> GetKeyWords()
        {
          using (var repository = new ScopedCompositeRepository())
          {
            return repository.Repositories.KeywordRepository.All.OrderBy(x => x.Word);
          }
        }
        
        public KeyWord GetKeyWord(int id)
        {
          using (var repository = new ScopedCompositeRepository())
          {
            KeyWord keyword = repository.Repositories.KeywordRepository.Find(id);
            return keyword;
          }
        }        

				public List<KeyWord> GetKeyWordsSorted()
				{
          using (var repository = new ScopedCompositeRepository())
          {
            return repository.Repositories.KeywordRepository.All.OrderBy(kw => kw.Word).ToList();
          }
				}

        public bool DeleteKeyWord(KeyWord keyWord)
        {
          // first delete all the links
          using (var keywordRepository = new ScopedCompositeRepository().Repositories.KeywordRepository)
          {
            keywordRepository.DeleteKeyWord(keyWord.KeyWordID);
          }

          return true;
        }

				public MultiSelectList KeyWordMultiSelectList(List<KeyWord> selectedWords)
				{
					List<SelectListItem> items = new List<SelectListItem>();

          List<KeyWord> keywords = this.GetKeyWordsSorted();
          keywords.Insert(0, new KeyWord { Word = "(Any)" });
					foreach (KeyWord keyword in keywords)
					{
						SelectListItem item = new SelectListItem();
						item.Text = keyword.Word;
						item.Value = keyword.Word;
						item.Selected = true;
						items.Add(item);
					}
          if (selectedWords==null)
          {
            selectedWords = new List<KeyWord>();
          }
					List<string> selectedItems = new List<string>();
					foreach (KeyWord keyword in selectedWords)
					{
						selectedItems.Add(keyword.Word);
					}

					MultiSelectList m = new MultiSelectList(items, "Value", "Text", selectedItems);
					return m;
				}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
      #endregion
    }
}