using PokemonReviewApp.Data;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class CountryRepository : ICountryRepository
    {
        private readonly DataContext _context;
        public CountryRepository(DataContext context)
        {
            _context = context;
        }

        public bool CountryExist(int id)
        {
            return _context.Countries.Any(c => c.Id == id);
        }

        public ICollection<Country> GetCountries()
        {
            return _context.Countries.OrderBy(c => c.Id).ToList();
        }

        public Country GetCountry(int id)
        {
            return _context.Countries.Where(c => c.Id == id).FirstOrDefault();
        }

        public Country GetCountryByOwner(int ownerId)
        {
            return _context.Owners.Where(o => o.Id == ownerId).Select(s => s.Country).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnersByCountry(int countryId)
        {
            return _context.Owners.Where(c => c.Country.Id == countryId).ToList();
        }

        // CREATE
        public bool CreateCountry(Country country)
        {
            _context.Add(country);
            return Save();
        }

        // UPDATE
        public bool UpdateCountry(Country country)
        {
            _context.Update(country);
            return Save();
        }

        // Delete
        public bool DeleteCountry(Country country)
        {
            _context.Remove(country);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
