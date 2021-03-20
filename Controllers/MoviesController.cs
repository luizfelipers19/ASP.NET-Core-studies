using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        // instanciação de um DbContext, que é responsável por trabalhar com o Banco de dados
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        // retorna todos os filmes do Banco de Dados
        // GET: api/<MoviesController>
        [HttpGet]
        public IActionResult Get()
        {
            //  return _dbContext.Movies;
            // código 200 - Sucesso
            return Ok(_dbContext.Movies);
            //código 400 - Bad request
            //return BadRequest();

            //código 404 - Not found
            //return NotFound();
            
            // retorna qualquer tipo de status code, a partir do objeto StatusCodes
         //   return StatusCode(StatusCodes.);
        }

        // retorna um filme específico, a partir do ID passado
        // GET api/<MoviesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            // caso não tenha encontrado um filme, retorna um NotFound
            if(movie == null)
            {
                return NotFound("Hmmm, não achei nenhum filme com esse número de ID/registro. Dá uma conferida aí ou tenta outros números de identificação.");
            }
            else
            {
                return Ok(movie);
            }

            
        }
        [HttpGet("[action]/{id}")]
        public int Test(int id)
        {
            return id;
        }

        //// POST api/<MoviesController>
        //[HttpPost]
        //public IActionResult Post([FromBody] Movie movieObj)
        //{
        //    _dbContext.Movies.Add(movieObj);
        //    _dbContext.SaveChanges();
        //    return StatusCode(StatusCodes.Status201Created);
        //}



        // POST api/<MoviesController>
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            //    _dbContext.Movies.Add(movieObj);
            //    _dbContext.SaveChanges();
            //    return StatusCode(StatusCodes.Status201Created);
            //
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot",guid +".jpg");
            
            if(movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }

            movieObj.ImageUrl = filePath.Remove(0,7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
            
            
            //return Ok();
        
        
        
        }

       



        // PUT api/<MoviesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
          var movie =   _dbContext.Movies.Find(id);
            if(movie == null)
            {

                return NotFound("Nenhum registro encontrado nesse ID");
            }
            else
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");

                if (movieObj.Image != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    movieObj.Image.CopyTo(fileStream);
                    movie.ImageUrl = filePath.Remove(0, 7);
                }



                movie.Name = movieObj.Name;
                            movie.Language = movieObj.Language;
                movie.Rating = movieObj.Rating;

                            _dbContext.SaveChanges();

                            return Ok("Registro atualizado com sucesso!");

            }
           


        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

        var movie =   _dbContext.Movies.Find(id);

        // caso o objeto não exista/ não tenha sido encontrado pelo Id passado na requisição
        if(movie == null)
            {
                return NotFound("Mano, tu tá tentando deletar um negócio de um registro que nem existe. Tenta ver certinho aí o número do Id que vc tá passando, porque aqui no Servidor não tem nada com esse número");
            }

            else// caso o registro exista
            {
                _dbContext.Movies.Remove(movie);
            _dbContext.SaveChanges();

            return Ok("Registro Deletado mlk");


            }

            
        }
    }
}
