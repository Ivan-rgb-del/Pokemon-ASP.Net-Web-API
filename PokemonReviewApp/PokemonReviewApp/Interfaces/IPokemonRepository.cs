using PokemonReviewApp.Models;

namespace PokemonReviewApp.Interfaces
{
    public interface IPokemonRepository
    {
        // show list
        ICollection<Pokemon> GetPokemons();
        // for these two when user click on special pokemon(Pikachu for example)
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        bool PokemonExist(int pokemonId);
        bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon);
        bool DeletePokemon(Pokemon pokemon);
        bool Save();
    }
}
