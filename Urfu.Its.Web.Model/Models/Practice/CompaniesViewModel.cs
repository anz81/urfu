using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urfu.Its.Web.DataContext;

namespace Urfu.Its.Web.Model.Models
{
    public class CompaniesViewModelRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string INN { get; set; }
        public string Address { get; set; }
        public string Director { get; set; }
        public string DirectorInitials { get; set; }
        public string DirectorGenitive { get; set; }
        public string PostOfDirector { get; set; }
        public string PostOfDirectorGenitive { get; set; }
        public string PersonInCharge { get; set; }
        public string PersonInChargeInitials { get; set; }
        public string PostOfPersonInCharge { get; set; }
        public string Phone { get; set; }
        public string CompanyPhoneNumber { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public List<string> Contracts { get; set; }
        public string DocumentName { get; set; }
        public string Country { get; set; }
        public int? CountryId { get; set; }
        public string Region { get; set; }
        public int? RegionId { get; set; }
        public string City { get; set; }
        public int? CityId { get; set; }
        public bool IsConfirmed { get; set; }

        //public string OwnershipType { get; set; }

        //public int? OwnershipTypeId { get; set; }
        
    }

    public class CompaniesViewModel
    {
        public IQueryable<CompaniesViewModelRow> Rows { get; set; }
        public CompaniesViewModel(List<Company> companies, IQueryable<Contract> contracts, IQueryable<CompanyLocation> locations)
        {
            var rows = new List<CompaniesViewModelRow>();

            foreach (var company in companies)
            {
                CompaniesViewModelRow row = new CompaniesViewModelRow();

                row.Name = company.Name;
                row.ShortName = company.ShortName;
                row.INN = company.INN;
                row.PersonInCharge = company.PersonInCharge;
                row.PersonInChargeInitials = company.PersonInChargeInitials;
                row.PostOfPersonInCharge = company.PostOfPersonInCharge;
                row.Director = company.Director;
                row.DirectorInitials = company.DirectorInitials;
                row.DirectorGenitive = company.DirectorGenitive;
                row.PostOfDirector = company.PostOfDirector;
                row.PostOfDirectorGenitive = company.PostOfDirectorGenitive; 
                row.Address = company.Address;
                row.Email = company.Email;
                row.Site = company.Site;
                row.Phone = company.PhoneNumber;
                row.CompanyPhoneNumber = company.CompanyPhoneNumber;
                row.Id = company.Id;                
                row.DocumentName = company.FileStorageId!= null ? company.FileStorage.FileNameForUser: "";
                row.IsConfirmed = company.IsConfirmed;
                //row.OwnershipTypeId = company.OwnershipTypeId;
                //row.OwnershipType = company.OwnershipType?.ShortName;

                var compContracts = contracts.Where(c => c.CompanyId == company.Id);
                row.Contracts = new List<string>();

                foreach (var c in compContracts)
                {
                    string info = "";
                    info += c.Number ?? "";
                    info += (c.ContractDate != null) ? " от " + c.ContractDate.Value.ToShortDateString() : "";
                    info += (c.StartDate != null) ? " c " + c.StartDate.Value.ToShortDateString() : "";
                    info += (c.FinishDate != null) ? " по " + c.FinishDate.Value.ToShortDateString() : "";

                    row.Contracts.Add(info);
                }

                if (company.Location != null)
                {
                    var location = company.Location;
                    switch (location.Level)
                    {
                        case 1: // указана страна
                            row.Country = location.Name;
                            row.CountryId = location.Id;
                            break;
                        case 2: // указан населенный пункт
                            var country = locations.FirstOrDefault(l => l.Id == location.ParentId && l.Level == 1);
                            row.Country = country.Name;
                            row.CountryId = country.Id;
                            row.Region = location.Name;
                            row.RegionId = location.Id;
                            break;
                        case 3: // указан город
                            row.City = location.Name;
                            row.CityId = location.Id;
                            var _region = locations.FirstOrDefault(l => l.Id == location.ParentId && l.Level == 2);
                            row.Region = _region.Name;
                            row.RegionId = _region.Id;
                            var _country = locations.FirstOrDefault(l => l.Id == _region.ParentId && l.Level == 1);
                            row.Country = _country.Name;
                            row.CountryId = _country.Id;
                            break;
                    }
                }

                rows.Add(row);
            }
            Rows = rows.AsQueryable();
        }
    }
}
