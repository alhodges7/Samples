using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.CompositeRepository;

namespace Astra.Models
{
    public class CustomProfile
    {
        #region Declarations
        private const int QUEUE_SIZE = 5;
        private System.Web.Script.Serialization.JavaScriptSerializer JsonSerializer;
        #endregion

        #region Properties
        private UserProfile userProfile;
        private string StrJson { get; set; }
        private List<int> _recentlyViewed = new List<int>(QUEUE_SIZE);

        private int numResultsPerPage;
        public int NumResultsPerPage
        {
            get
            {
                return numResultsPerPage;
            }

            set
            {
                numResultsPerPage = value;
            }
        }
        private string defaultSort;
        public string DefaultSort 
        {
            get
            {
                return defaultSort;
            }
            set
            {
                defaultSort = value;
            }
        }

        private string keyWords;
        public string KeyWords
        {
            get
            {
                return keyWords;
            }
            set
            {
                keyWords = value;
            }
        }
        public List<int> RecentlyViewed
        {
          get {return _recentlyViewed; }
          set { _recentlyViewed = value; }
        }
        #endregion

        #region Consructors
        public CustomProfile()
        {

        }

        public CustomProfile(UserProfile userProfile)
        {
            this.userProfile = userProfile;
            DecodeJsonString(userProfile.Preferences);
        }

        public CustomProfile(string mid)
        {
            using (var repositories = new ScopedCompositeRepository())
            {
                userProfile = repositories.Repositories.UserProfileRepository.FindProfileByMID(mid);
                if(userProfile != null)
                    DecodeJsonString(userProfile.Preferences);
            }
        }
        #endregion

        #region Public Methods
        public int AddToRecentlyViewed(int Viewed)
        {
            if (!this._recentlyViewed.Contains(Viewed))
            {
                if (this._recentlyViewed.Count() >= QUEUE_SIZE)
                    this._recentlyViewed.RemoveAt(0);
            
                this._recentlyViewed.Add(Viewed);
            }
            
            return this._recentlyViewed.Count();
        }
        public CustomProfile SaveCustomProfile()
        {
            MakeJsonString();
            using (var repositories = new ScopedCompositeRepository())
            {
                userProfile = repositories.Repositories.UserProfileRepository.Update(userProfile);
            }
            return this;
        }
        #endregion

        #region Private Methods
        private string MakeJsonString()
        {
            JsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            StrJson = JsonSerializer.Serialize(this);
            userProfile.Preferences = StrJson;
            return StrJson;
        }
        private void DecodeJsonString(string preferences)
        {
            if(!string.IsNullOrEmpty(preferences))
            {
                JsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                CustomProfile cp = JsonSerializer.Deserialize<CustomProfile>(preferences);
                this.NumResultsPerPage = cp.NumResultsPerPage;
                this.DefaultSort = cp.DefaultSort;
                this.KeyWords = cp.KeyWords;
                this._recentlyViewed = cp._recentlyViewed;
            }
        }
        #endregion
    }
}