using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace PL.Controllers
{
    public class PokemonController : Controller
    {
        string URL_API = "https://pokeapi.co/api/v2/";
        string image = "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/";

        [HttpGet]
        public ActionResult GetAll(string pag)
        {
            ML.Pokemon pokemon = new ML.Pokemon();
            pokemon.Pokemons = new List<object>();
            pag = GetPagination(pag);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL_API);
                var responseTask = client.GetAsync(pag);
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    dynamic json = JObject.Parse(readTask.Result.ToString());

                    pokemon.Next = json.next;
                    pokemon.Previus = json.previous;

                    //Busqueda por tipo
                    if (pag.Length < 10)
                    {
                        foreach (var item in json.pokemon)
                        {
                            ML.Pokemon listPokemon = new ML.Pokemon();

                            listPokemon.Nombre = item.pokemon.name;
                            listPokemon.URL = item.pokemon.url;
                            listPokemon.Id = GetId(listPokemon.URL);
                            listPokemon.Image = image + listPokemon.Id + ".png";

                            pokemon.Pokemons.Add(listPokemon);
                        }
                    }
                    //Realiza el ciclo de GetAll
                    else
                    {
                        foreach (var item in json.results)
                        {
                            ML.Pokemon listPokemon = new ML.Pokemon();

                            listPokemon.Nombre = item.name;
                            listPokemon.Nombre = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(listPokemon.Nombre);
                            listPokemon.URL = item.url;
                            listPokemon.Id = GetId(listPokemon.URL);
                            listPokemon.Image = image + listPokemon.Id + ".png";

                            pokemon.Pokemons.Add(listPokemon);
                        }
                    }
                }
            }
            return View(pokemon);
        }
        [HttpGet]
        public ActionResult GetById(int Id)
        {
            ML.Result resultTipo = new ML.Result();
            resultTipo.results = new List<object>();

            ML.Result resultHabilidades = new ML.Result();
            resultHabilidades.results = new List<object>();

            ML.Pokemon pokemon = new ML.Pokemon();
            pokemon.Pokemons = new List<object>();

            pokemon.Detalles = new ML.Detalles();
            pokemon.Detalles.Tipo = new ML.Tipo();
            pokemon.Detalles.Habilidades = new ML.Habilidad();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL_API);
                var responseTask = client.GetAsync("pokemon/" + Id);
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsStringAsync();
                    dynamic json = JObject.Parse(readTask.Result.ToString());

                    pokemon.Id = json.id;
                    pokemon.Nombre = json.name;
                    pokemon.Image = image + pokemon.Id + ".png";

                    //Detalles
                    pokemon.Detalles.Tamaño = json.height;
                    pokemon.Detalles.Tamaño = pokemon.Detalles.Tamaño / 10;
                    pokemon.Detalles.Peso = json.weight;
                    pokemon.Detalles.Peso = pokemon.Detalles.Peso / 10;

                    //Tipo
                    foreach (var item in json.types)
                    {
                        ML.Pokemon pokemonitem = new ML.Pokemon();
                        pokemonitem.Detalles = new ML.Detalles();
                        pokemonitem.Detalles.Tipo = new ML.Tipo();

                        pokemonitem.Detalles.Tipo.Nombre = item.type.name;
                        pokemonitem.Detalles.Tipo.url = item.type.url;
                        pokemonitem.Detalles.Tipo.Id = GetIdTipo(pokemonitem.Detalles.Tipo.url);

                        resultTipo.results.Add(pokemonitem);
                    }
                    pokemon.Detalles.Tipo.Tipos = resultTipo.results;

                    //Habilidades
                    foreach (var item in json.stats)
                    {
                        ML.Pokemon pokemonItemStatus = new ML.Pokemon();
                        pokemonItemStatus.Detalles = new ML.Detalles();
                        pokemonItemStatus.Detalles.Habilidades = new ML.Habilidad();

                        pokemonItemStatus.Detalles.Habilidades.Name = item.stat.name;
                        pokemonItemStatus.Detalles.Habilidades.Value = item.base_stat;

                        resultHabilidades.results.Add(pokemonItemStatus);
                    }
                    pokemon.Detalles.Habilidades.Habilidades = resultHabilidades.results;
                }
            }
            return View(pokemon);
        }
        public int GetId(string URL)
        {
            string Id;
            Id = URL.Substring(34);
            Id = Id[..^1];
            return int.Parse(Id);
        }
        public int GetIdTipo(string URL)
        {
            string Id;
            Id = URL.Substring(31);
            Id = Id[..^1];
            return int.Parse(Id);
        }
        public string GetPagination(string URL)
        {
            if (URL == null)
            {
                URL = "pokemon?limit=20&offset=0";
            }
            else
            {
                if (URL.Length > 10)
                {
                    URL = URL.Substring(26);
                }
            }
            return URL;
        }

    }
}
