using BuyAndSellAppWeb.Models;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BuyAndSellAppWeb.Repository
{
    public class BuyAndSellRepository : IBuyAndSellRepository
    {
        public string ANNONSLISTA { get; set; } = "AnnonsLista";
        public string ANNONSLISTA_ID { get; set; } = "e65de9f2-b880-446b-8596-710c84173e2a";
        User _user = null;
        //TERMSTORE
        public string TERMGROUPNAME { get; set; } = "AnnonsKategorier";
        public string TERMSETNAME { get; set; } = "Kategorier";
        public string TERM_ELEKTRONIK { get; set; } = "Elektronik";
        public string TERM_FORDON { get; set; } = "Fordon";
        public string TERM_FRITIDOCHHOBBY { get; set; } = "Fritid & hobby";
        public string TERM_HYGIENARTIKLAR { get; set; } = "Hygienartiklar";
        public string TERM_MÖBLER { get; set; } = "Möbler";
        //TERMSTORE


        //RetrivespecificFields
        public ListItemCollection RetriveList2(ClientContext context, string listTitle)
        {
            Web web = context.Web;
            List list = web.Lists.GetByTitle(listTitle);
            var query = new CamlQuery()
            {

                ViewXml = "<View><Query /><ViewFields>" +
                                              "<FieldRef Name='ID'/>" +
                                              "<FieldRef Name='Title'/>" +
                                              "<FieldRef Name='ubza'/>" +
                                              "<FieldRef Name='Pris'/>" +
                                              "<FieldRef Name='Created'/>" +
                                              "<FieldRef Name='Kategorier'/>" +
                                              "<FieldRef Name='S_x00e4_ljare'/>" +
                                              "<FieldRef Name='Bild'/>"+
                                              "</ViewFields></View>"
            };
            var items = list.GetItems(query);
            context.Load(items);
            context.ExecuteQuery();

            return items;
        }
        //RetrivespecificFields
        //RetriveallFields
        public ListItemCollection RetriveList(ClientContext context, string listTitle)
        {
            List retrivesList = context.Web.Lists.GetByTitle(listTitle);
            CamlQuery query = CamlQuery.CreateAllItemsQuery();
            ListItemCollection items = retrivesList.GetItems(query);

            context.Load(items);
            context.ExecuteQuery();

            return items;
        }
        //RetriveallFields
        //GetItems
        public List<Advertisment> GetItems(ClientContext context, string listTitle)
        {
            ListItemCollection listItems = RetriveList2(context, listTitle);
            List<Advertisment> products = new List<Advertisment>();
            foreach (var items in listItems)
            {
                TaxonomyFieldValue taxFieldValue = items["Kategorier"] as TaxonomyFieldValue;
                FieldUserValue user = (FieldUserValue)items["S_x00e4_ljare"];
                Advertisment product = new Advertisment
                {
                    ID = Convert.ToInt32(items["ID"]),
                    ProductTitle = items["Title"].ToString(),
                    Description = Convert.ToString(items["ubza"]),
                    Created = Convert.ToDateTime(items["Created"]),
                    Price = Convert.ToDecimal(items["Pris"]),
                    Category = taxFieldValue.Label,
                    Seller = user.Email,
                    SellerToken = VerifyUser(user),
                };
                products.Add(product);
            }

            return products;
        }
        //GetItems
        //GetTaxanomy
        public List<SelectListItem> GetTaxanomy(ClientContext context)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            TermCollection terms = null;
            TaxonomySession taxonomy = TaxonomySession.GetTaxonomySession(context);
            if (taxonomy != null)
            {
                TermStore termStore = taxonomy.GetDefaultKeywordsTermStore();
                if (termStore != null)
                {
                    TermGroup termGroup = termStore.Groups.GetByName(TERMGROUPNAME);
                    TermSet termSet = termGroup.TermSets.GetByName(TERMSETNAME);
                    terms = termSet.GetAllTerms();

                    context.Load(terms);
                    context.ExecuteQuery();

                    foreach (var term in terms)
                    {
                        items.Add(new SelectListItem { Text = term.Name, Value = term.Id.ToString() });
                    }

                }
            }
            return items;
        }
        //GetTaxanomy
        //AddToSharePoinList
        public void AddToSharePoinList(ClientContext context, Advertisment formData, string listTitle)
        {
            _user = GetUser();
            List bookedRooms = context.Web.Lists.GetByTitle(listTitle);
            ListItemCreationInformation SVitemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = bookedRooms.AddItem(SVitemCreateInfo);

            Web Web = context.Web;
            context.Load(Web);
            context.ExecuteQuery();

            User newUser = Web.EnsureUser(_user.LoginName);
            context.Load(newUser);
            context.ExecuteQuery();

            FieldUserValue userValue = new FieldUserValue
            {
                LookupId = newUser.Id
            };

            newItem["Title"] = formData.ProductTitle;
            newItem["ubza"] = formData.Description;
            newItem["Pris"] = formData.Price;
            newItem["Kategorier"] = formData.Category;
            newItem["Bild"] = formData.ImageUrl;
            newItem["S_x00e4_ljare"] = userValue;

            newItem.Update();
            context.ExecuteQuery();

        }
        //AddToSharePoinList
        //DeleteItem
        public void DeleteItem(ClientContext SPcontext, string listName, int id)
        {
            List listToDelete = SPcontext.Web.Lists.GetByTitle(listName);
            ListItem listItem = listToDelete.GetItemById(id);
            listItem.DeleteObject();

            SPcontext.ExecuteQuery();
        }
        //DeleteItem
        //GetListItem
        public Advertisment GetListItem(ClientContext context, string listTitle, int id)
        {
            Web web = context.Web;
            List list = web.Lists.GetByTitle(listTitle);

            ListItem r = list.GetItemById(id);
            context.Load(r);
            context.ExecuteQuery();

            TaxonomyFieldValue taxFieldValue = r["Kategorier"] as TaxonomyFieldValue;

            Advertisment enProdukt = new Advertisment
            {
                ID = Convert.ToInt32(r["ID"]),
                ProductTitle = Convert.ToString(r["Title"]),
                Description = Convert.ToString(r["ubza"]),
                Price = Convert.ToDecimal(r["Pris"]),
                ImageUrl = ((FieldUrlValue)(r["Bild"])).Url,
                Category = taxFieldValue.Label

            };
            return enProdukt;
        }
        //GetListItem
        //ModifyItem
        public void ModifyItem(ClientContext context, string listTitle, Advertisment formData)
        {
            List list = context.Web.Lists.GetByTitle(listTitle);
            ListItem newItem = list.GetItemById(formData.ID);

            newItem["Title"] = formData.ProductTitle;
            newItem["ubza"] = formData.Description;
            newItem["Pris"] = formData.Price;
            newItem["Bild"] = formData.ImageUrl;
            newItem["Kategorier"] = formData.Category;

            newItem.Update();
            context.ExecuteQuery();
        }
        //ModifyItem
        //CreateTermSet
        public void CreateTermSet(ClientContext context)
        {

            //English(Default English Locale ID)            
            int lcid = 1033;
            //Get termSession
            TaxonomySession taxonomy = TaxonomySession.GetTaxonomySession(context);
            if (taxonomy != null)
            {
                //Get TermStore
                TermStore termStore = taxonomy.GetDefaultKeywordsTermStore();
                if (termStore != null)
                {
                    //Set TermGroup
                    TermGroup termGroup = termStore.CreateGroup(TERMGROUPNAME, Guid.NewGuid());
                    //Set TermSet
                    TermSet termSetCollection = termGroup.CreateTermSet(TERMSETNAME, Guid.NewGuid(), lcid);
                    //Set Terms
                    termSetCollection.CreateTerm(TERM_ELEKTRONIK, lcid, Guid.NewGuid());
                    termSetCollection.CreateTerm(TERM_FORDON, lcid, Guid.NewGuid());
                    termSetCollection.CreateTerm(TERM_FRITIDOCHHOBBY, lcid, Guid.NewGuid());
                    termSetCollection.CreateTerm(TERM_HYGIENARTIKLAR, lcid, Guid.NewGuid());
                    termSetCollection.CreateTerm(TERM_MÖBLER, lcid, Guid.NewGuid());

                    context.ExecuteQuery();
                }
            }
        }
        //CreateTermSet
        //CheckTermGroupName
        public bool CheckTermGroupName(ClientContext context)
        {
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(context);
            TermStore termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            context.Load(termStore,
                   store => store.Name,
                   store => store.Groups.Include(
                       group => group.Name));
            context.ExecuteQuery();
            if (taxonomySession != null)
            {
                if (termStore != null)
                {
                    foreach (TermGroup group in termStore.Groups)
                    {
                        if (group.Name == TERMGROUPNAME)
                        {
                            return true;

                        }
                    }
                }
            }
            return false;
        }
        //CheckTermGroupName
        //GetDeatils
        public Advertisment GetDeatils(ClientContext context, string listTitle, int id)
        {
            Advertisment adDetails = new Advertisment();
            if (context != null)
            {
                List list = context.Web.Lists.GetByTitle(listTitle);
                ListItem newItem = list.GetItemById(id);
                context.Load(newItem);
                context.ExecuteQuery();

                TaxonomyFieldValue taxFieldValue = newItem["Kategorier"] as TaxonomyFieldValue;
                FieldUserValue user = (FieldUserValue)newItem["S_x00e4_ljare"];
                adDetails = new Advertisment
                {
                    ID = Convert.ToInt32(newItem["ID"]),
                    ProductTitle = newItem["Title"].ToString(),
                    Description = Convert.ToString(newItem["ubza"]),
                    Created = Convert.ToDateTime(newItem["Created"]),
                    Price = Convert.ToDecimal(newItem["Pris"]),
                    Category = taxFieldValue.Label,
                    ImageUrl = ((FieldUrlValue)(newItem["Bild"])).Url,
                    Seller = user.Email,
                    SellerToken = VerifyUser(user),
                };
            }

            return adDetails;
        }
        //GetDeatils
        //VerifyUser
        public bool VerifyUser(FieldUserValue inputUser)
        {
            User _User = GetUser();
            if (inputUser.LookupId == _user.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //VerifyUser
        //GetUser
        public User GetUser()
        {
            var spContext = MySession.Current.spcontext;
            using (var userContext = spContext.CreateUserClientContextForSPHost())
            {
                _user = userContext.Web.CurrentUser;
                try
                {
                    userContext.Load(_user);
                    userContext.ExecuteQuery();
                }
                catch (Exception e)
                {
                    throw new HttpException(400, "Error: " + e.Message);
                }
            }
            return _user;
        }
        //GetUser
    }
    //Interface
    public interface IBuyAndSellRepository
    {
        //Functions------------------------------------------------------------------------
        ListItemCollection RetriveList2(ClientContext context, string listTitle);
        ListItemCollection RetriveList(ClientContext context, string listTitle);
        List<Advertisment> GetItems(ClientContext context, string listTitle);
        List<SelectListItem> GetTaxanomy(ClientContext context);
        void AddToSharePoinList(ClientContext context, Advertisment formData, string listTitle);
        void DeleteItem(ClientContext SPcontext, string listName, int id);
        Advertisment GetListItem(ClientContext context, string listTitle, int id);
        void ModifyItem(ClientContext context, string listTitle, Advertisment formData);
        bool VerifyUser(FieldUserValue inputUser);
        User GetUser();
        void CreateTermSet(ClientContext context);
        bool CheckTermGroupName(ClientContext context);
        Advertisment GetDeatils(ClientContext context, string listTitle, int id);

        //Functions-------------------------------------------------------------------------
        //Properties-----------------------
        string ANNONSLISTA { get; set; }
        string ANNONSLISTA_ID { get; set; }
        //Taxonomy
        //TermGroup
        string TERMGROUPNAME { get; set; }
        //TermSet
        string TERMSETNAME { get; set; }
        //Terms
        string TERM_ELEKTRONIK { get; set; }
        string TERM_FORDON { get; set; }
        string TERM_FRITIDOCHHOBBY { get; set; }
        string TERM_HYGIENARTIKLAR { get; set; }
        string TERM_MÖBLER { get; set; }
        //Taxonomy

        //--------------------------------
    }
    //Interface
}