using Project_With_Domain_Login.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project_With_Domain_Login.Classes
{
    /// <summary>
    /// A bunch of methods to save entities
    /// </summary>
    public class Database
    {
        private DBEntityFramework db = null;
        private String currentUser = "";

        public Database(DBEntityFramework db, String currentUser)
        {
            this.db = db;
            this.currentUser = currentUser;
        }

        #region PT
        public void Save(ProviderTracking audit)
        {
            // Set properties
            audit.LastModified = DateTime.Now;
            audit.LastModifiedBy = currentUser;// GetCurrentUser();

            // Save it
            if (audit.Id == 0)
            {
                //audit.CreationDate = DateTime.Now;
                audit.DateAdded = DateTime.Now;
                Add(audit);
            }
            else
            {
                Update(audit);
            }
        }

        public void Add(ProviderTracking audit)
        {
            db.ProviderTrackings.Add(audit);
            db.SaveChanges();
        }

        public void Update(ProviderTracking audit)
        {
            db.ProviderTrackings.Attach(audit);
            db.Entry(audit).State = EntityState.Modified;
            db.SaveChanges();
        }
        #endregion

        #region Check
        public void SaveList(List<Checklist> checklists)
        {
            foreach (Checklist c in checklists)
            {
                Save(c);
            }
        }

        public void Save(Checklist c)
        {
            if (c.Id == 0)
            {
                Add(c);
            }
            else
            {
                Update(c);
            }
        }

        public void Update(Checklist c)
        {
            db.Checklists.Attach(c);
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Add(Checklist c)
        {
            db.Checklists.Add(c);
            db.SaveChanges();
        }

        public void Delete(Checklist c)
        {
            db.Checklists.Attach(c);
            db.Checklists.Remove(c);
            db.SaveChanges();
        }

        /// <summary>
        /// Deletes checklists in a list that are marked for deletion
        /// </summary>
        /// <param name="checklists"></param>
        public void DeleteList(List<Checklist> checklists)
        {
            foreach(Checklist c in checklists)
            {
                if(c.Delete)
                {
                    Delete(c);
                }
            }
        }
        #endregion

    }
}