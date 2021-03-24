using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {

        private CinemaDbContext _dbContext;
        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;

        }


        [Authorize]
        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort, int? pageNumber, int? pageSize)
        {

            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;


           var movies =  from movie in _dbContext.Movies
            select new
            {
                Id = movie.Id,
                Name = movie.Name,
                Genre = movie.Genre,
                Rating = movie.Rating,
                Language = movie.Language,
                Duration = movie.Duration,
                ImageUrl = movie.ImageUrl
            };

            switch (sort)
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));

                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));

                default:
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize) );
            }




            return Ok(movies);


        }


        //api/movies/moviedetail/1
        [Authorize]
        [HttpGet("[action]/{id}")]
        public IActionResult MovieDetail(int id)
        {
            
            
            var movie = _dbContext.Movies.Find(id);

            if(movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }







        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObj)
        {
            //    _dbContext.Movies.Add(movieObj);
            //    _dbContext.SaveChanges();
            //    return StatusCode(StatusCodes.Status201Created);
            //
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            if (movieObj.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create);
                movieObj.Image.CopyTo(fileStream);
            }

            movieObj.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movieObj);
            _dbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);


            //return Ok();



        }

        // PUT api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObj)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
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
                movie.Description = movieObj.Description;
                movie.Language = movieObj.Language;
                movie.PlayingDate = movieObj.PlayingDate;
                movie.PlayingTime = movieObj.PlayingTime;
                movie.TicketPrice = movieObj.TicketPrice;
                movie.Rating = movieObj.Rating;
                movie.Genre = movieObj.Genre;
                movie.TrailerUrl = movieObj.TrailerUrl;
                movie.Image = movieObj.Image;

                _dbContext.SaveChanges();

                return Ok("Registro atualizado com sucesso!");

            }



        }

        // DELETE api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            var movie = _dbContext.Movies.Find(id);

            // caso o objeto não exista/ não tenha sido encontrado pelo Id passado na requisição
            if (movie == null)
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
