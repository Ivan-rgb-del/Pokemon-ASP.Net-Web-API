using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IMapper _mapper;
        private readonly IReviewRepository _reviewRepository;

        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper, IReviewRepository reviewRepository)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _reviewRepository = reviewRepository;
        }

        // get all pokemons(list)
        [HttpGet]
        [ProducesResponseType (200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // this enables us by IActionresult
            }
            return Ok(pokemons); // this enables us by IActionresult
        }

        // get specific pokemon bt Id
        [HttpGet("{pokemonId}")]
        [ProducesResponseType (200, Type = typeof(Pokemon))]
        [ProducesResponseType (400)]
        public IActionResult GetPokemon(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExist(pokemonId))
            {
                return NotFound(); // this enables us by IActionresult
            }

            var specificPokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokemonId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // this enables us by IActionresult
            }
            return Ok(specificPokemon); // this enables us by IActionresult
        }

        // CREATE
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
            {
                return BadRequest(ModelState);
            }

            var pokemons = _pokemonRepository.GetPokemons()
                .Where(p => p.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon allready exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving!");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }

        // Update
        [HttpPut("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokemonId, [FromQuery]int ownerId, [FromQuery]int categoryId, [FromBody] PokemonDto updatePokemon)
        {
            if (updatePokemon == null)
                return BadRequest(ModelState);

            if (pokemonId != updatePokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExist(pokemonId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(updatePokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating pokemon!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        // Delete
        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExist(pokemonId))
            {
                return NotFound();
            }

            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokemonId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong when deleting the reviews!");
            }

            if (_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting pokemon!");
            }

            return NoContent();
        }
    }
}
